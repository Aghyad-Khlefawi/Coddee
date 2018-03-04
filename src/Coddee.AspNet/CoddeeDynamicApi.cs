// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using Coddee.Attributes;
using Coddee.Data;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Coddee.AspNet
{
    /// <summary>
    /// An AspNetCode middle-ware that provides an interface for the application repositories.
    /// </summary>
    public class CoddeeDynamicApi
    {
        private readonly RequestDelegate _next;
        private readonly IRepositoryManager _repositoryManager;
        private readonly Func<IIdentity, object> _setContext;

        /// <inheritdoc />
        public CoddeeDynamicApi(RequestDelegate next,
            IRepositoryManager repositoryManager,
            CoddeeControllersManager controllersManager,
                                Func<IIdentity, object> setContext)
        {
            _next = next;
            _repositoryManager = repositoryManager;
            _setContext = setContext;
            _apiActions = controllersManager.GetRegisteredActions();
        }

        private readonly Dictionary<string, IApiAction> _apiActions;

        /// <summary>
        /// Returns the requested repository either by the class name
        /// or by using <see cref="NameAttribute"/>
        /// </summary>
        IRepository GetRepositoryByName(string name)
        {
            return _repositoryManager.GetRepository(name);
        }

        /// <summary>
        /// This method is called when a valid request is received by AspNetCore
        /// </summary>
        public async Task Invoke(HttpContext context)
        {
            var req = context.Request;
            var handled = await HandleRequest(context, req);
            if (!handled)
                await _next(context);
        }

        /// <summary>
        /// Process the request parameters and return the value from the required repository.
        /// <returns>True if the request was handled</returns>
        /// </summary>
        private async Task<bool> HandleRequest(HttpContext context, HttpRequest req)
        {
            if (req.Path.HasValue)
            {
                //  Split the path
                //  It should be like the following formula
                //  host/dapi/Repository/Action
                var pathParts = req.Path.Value.ToLower().Split('/').Skip(2);
                var repositoryName = pathParts.ElementAt(0);
                var actionName = pathParts.ElementAt(1);
                var path = $"{repositoryName}/{actionName}";

                //  Check if this path was not called before
                //  then create an action for it
                if (!_apiActions.TryGetValue(path, out var action))
                    action = CreateAction(repositoryName, actionName, path);

                if (action == null)
                {
                    // Action not found
                    await context.Response.WriteAsync("Action not found.");
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    return true;
                }

                // Check for authentication and authorization
                if (action.RequiredAuthentication)
                {

                    var authoized = string.IsNullOrEmpty(action.Claim) || context.User.Identity.IsAuthenticated && context.User.Identity is ClaimsIdentity identity && identity.Claims.Any(e => e.Value == action.Claim);

                    if (!authoized)
                    {
                        context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                        await context.Response.WriteAsync("Unauthorized.");
                        return true;
                    }
                }

                IEnumerable<object> args = null;
                if (action.ParametersInfo.Count > 0)
                {
                    try
                    {
                        args = await ParseParameters(req, action.ParametersInfo);
                    }
                    catch (APIException ex)
                    {
                        context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                        await context.Response.WriteAsync(ex.Message);
                        return true;
                    }
                }

                try
                {
                    object res = null;
                    if (action.Owner == null)
                    {
                        var repository = GetRepositoryByName(action.RepositoryName);
                        if (_setContext != null)
                            repository.SetContext(_setContext(context.User.Identity));
                        res = await action.Invoke(repository, args);
                    }
                    else
                        res = await action.Invoke(args);

                    context.Response.StatusCode = (int)HttpStatusCode.OK;
                    if (action.RetrunsValue)
                    {
                        context.Response.ContentType = "application/json";
                        await context.Response.WriteAsync(JsonConvert.SerializeObject(res));
                    }
                }
                catch (Exception e)
                {


                    if (e.InnerException is DBException dbexc)
                    {
                        var apiEx = new APIException(dbexc);
                        var ex = JsonConvert.SerializeObject(apiEx);
                        context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                        await context.Response.WriteAsync(ex);
                    }
                    else if (e.InnerException is AggregateException aggregate && aggregate.InnerExceptions.First() is DBException adbexc)
                    {
                        var apiEx = new APIException(adbexc);
                        var ex = JsonConvert.SerializeObject(apiEx);
                        context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                        await context.Response.WriteAsync(ex);
                    }
                    else
                    {
                        var ex = JsonConvert.SerializeObject(new APIException(e));
                        context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                        await context.Response.WriteAsync(ex);
                    }

                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// Returns an action that wraps a repository call 
        /// to be used easily in future requests.
        /// </summary>
        /// <param name="repositoryName">The name of the requested repository.</param>
        /// <param name="actionName">The requested action in the repository.</param>
        /// <param name="path">The path of the request.</param>
        /// <returns></returns>
        private IApiAction CreateAction(string repositoryName, string actionName, string path)
        {
            IApiAction action = null;

            // Get the target repository
            var repository = GetRepositoryByName(repositoryName);
            if (repository != null)
            {
                if (actionName.ToLower() == "getitem")
                    actionName = "get_item";

                // find the requested method info
                var method = repository
                    .GetType()
                    .GetMethods()
                    .FirstOrDefault(e => e.Name.Equals(actionName, StringComparison.InvariantCultureIgnoreCase));

                var interfaceMethod = repository.ImplementedInterface
                                                .GetMethods()
                                                .FirstOrDefault(e => e.Name.Equals(actionName, StringComparison.InvariantCultureIgnoreCase));

                // Create a delegate object for the action to improve dynamic calls performance
                action = DelegateAction.CreateDelegateAction(repositoryName, method, path);
                if (action == null)
                    return null;

                if (interfaceMethod != null)
                {
                    var authAttr = interfaceMethod.GetCustomAttribute<AuthorizeAttribute>();
                    if (authAttr != null)
                    {
                        action.RequiredAuthentication = true;
                        action.Claim = authAttr.Claim;
                    }
                }
                _apiActions.Add(path, action);
            }

            return action;
        }

        /// <summary>
        /// Parse the parameters from the HttpRequest
        /// </summary>
        private async Task<IEnumerable<object>> ParseParameters(HttpRequest req, IEnumerable<ActionParameter> param)
        {
            var args = new List<object>();
            if (param.Any())
            {
                if (req.Method == HttpMethods.Get || req.Method == HttpMethods.Delete)
                {
                    foreach (var parameterInfo in param)
                    {
                        bool found = false;
                        foreach (var queryParam in req.Query)
                        {
                            if (queryParam.Key.ToLower() == parameterInfo.Name)
                            {
                                try
                                {
                                    if (parameterInfo.Type == typeof(DateTime))
                                    {
                                        args.Add(DateTime.Parse(queryParam.Value));
                                    }
                                    else if (parameterInfo.Type == typeof(Guid))
                                    {
                                        args.Add(Guid.Parse(queryParam.Value));
                                    }
                                    else if (parameterInfo.Type == typeof(bool))
                                    {
                                        args.Add(bool.Parse(queryParam.Value));
                                    }
                                    else if (parameterInfo.Type == typeof(int))
                                    {
                                        args.Add(int.Parse(queryParam.Value));
                                    }
                                    else if (parameterInfo.Type == typeof(string))
                                    {
                                        args.Add(queryParam.Value.ToString());
                                    }
                                    else
                                    {
                                        args.Add(JsonConvert.DeserializeObject(queryParam.Value, parameterInfo.Type));
                                    }
                                }
                                catch (Exception)
                                {
                                    args.Add(JsonConvert.DeserializeObject($"\"{queryParam.Value}\"", parameterInfo.Type));
                                }
                                found = true;
                                break;
                            }
                        }
                        if (!found)
                            throw new APIException(0, $"Missing parameters '{parameterInfo.Name}'");
                    }
                }
                else
                {
                    JObject bodyParams = null;
                    JToken premitiveParam = null;
                    bool isPrimitveParam = false;
                    using (var sr = new StreamReader(req.Body))
                    {
                        var text = await sr.ReadToEndAsync();
                        if (!text.StartsWith("{"))
                        {
                            premitiveParam = JToken.Parse(text);
                            isPrimitveParam = true;
                        }
                        else
                            bodyParams = JObject.Parse(text);
                    }
                    if (isPrimitveParam)
                    {
                        AddPrimitiveParam(args, premitiveParam, param.ElementAt(0));
                    }
                    else if (bodyParams != null && param.Count() == 1)
                    {
                        args.Add(bodyParams.ToObject(param.ElementAt(0).Type));
                    }
                    else
                        foreach (var parameterInfo in param)
                        {
                            AddPrimitiveParam(args, bodyParams, parameterInfo);
                        }
                }
            }
            return args;
        }

        private static void AddPrimitiveParam(List<object> args, JObject bodyParams, ActionParameter parameterInfo)
        {
            if (bodyParams.TryGetValue(parameterInfo.Name, StringComparison.InvariantCultureIgnoreCase, out JToken value))
            {
                args.Add(value.ToObject(parameterInfo.Type));
            }
        }
        private static void AddPrimitiveParam(List<object> args, JToken bodyParams, ActionParameter parameterInfo)
        {
            args.Add(bodyParams.ToObject(parameterInfo.Type));
        }
    }
}
