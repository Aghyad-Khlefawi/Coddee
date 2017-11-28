// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using Coddee.Attributes;
using Coddee.Data;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Coddee.AspNet
{
    public class CoddeeDynamicApi
    {
        private readonly RequestDelegate _next;
        private readonly IRepositoryManager _repositoryManager;

        public CoddeeDynamicApi(RequestDelegate next,
            IRepositoryManager repositoryManager,
            CoddeeControllersManager controllersManager)
        {
            _next = next;
            _repositoryManager = repositoryManager;
            _apiActions = controllersManager.GetRegisteredActions();
        }

        private readonly Dictionary<string, IApiAction> _apiActions;

        IRepository GetRepositoryByName(string name)
        {
            var repositories = _repositoryManager.GetRepositories();
            foreach (var repository in repositories)
            {
                var reportAttr = repository.GetType().GetCustomAttribute<RepositoryAttribute>();
                var interType = reportAttr.ImplementedRepository;
                var nameAttrs = interType.GetCustomAttributes<NameAttribute>();
                foreach (var nameAttr in nameAttrs)
                {
                    if (nameAttr != null && nameAttr.Value.Equals(name, StringComparison.InvariantCultureIgnoreCase))
                        return repository;
                }

                var repoName = interType.Name;
                repoName = repoName.Remove(0, 1);
                if (repoName.EndsWith("Repository"))
                    repoName = repoName.Substring(0, repoName.Length - 10);

                if (repoName.Equals(name, StringComparison.InvariantCultureIgnoreCase))
                    return repository;
            }
            return null;
        }

        public async Task Invoke(HttpContext context)
        {
            var req = context.Request;
            var handled = await HandleRequest(context, req);
            if (!handled)
                await _next(context);
        }

        private async Task<bool> HandleRequest(HttpContext context, HttpRequest req)
        {
            if (req.Path.HasValue)
            {
                var pathParts = req.Path.Value.ToLower().Split('/').Skip(2);
                var repositoryName = pathParts.ElementAt(0);
                var actionName = pathParts.ElementAt(1);
                var path = $"{repositoryName}/{actionName}";

                if (!_apiActions.TryGetValue(path, out var action))
                    action = CreateAction(repositoryName, actionName, path);

                if (action != null)
                {
                    if (action.RequiredAuthentication)
                    {
                        bool authoized = context.User.Identity.IsAuthenticated;
                        if (!string.IsNullOrWhiteSpace(action.Claim))
                            authoized = false;

                        if (!authoized)
                        {
                            context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                            await context.Response.WriteAsync("Unauthorize.");
                            return true;
                        }
                    }

                    IEnumerable<object> args;
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

                    var res = await action.Invoke(args);
                    context.Response.StatusCode = (int)HttpStatusCode.OK;
                    if (action.RetrunsValue)
                        await context.Response.WriteAsync(JsonConvert.SerializeObject(res));

                    return true;
                }

            }

            return false;
        }

        private IApiAction CreateAction(string repositoryName, string actionName, string path)
        {
            IApiAction action = null;
            var repository = GetRepositoryByName(repositoryName);
            if (repository != null)
            {
                if (actionName.ToLower() == "getitem")
                    actionName = "get_item";

                var method = repository
                    .GetType()
                    .GetMethods()
                    .FirstOrDefault(e => e.Name.Equals(actionName, StringComparison.InvariantCultureIgnoreCase));

                var interfaceMethod = repository.ImplementedInterface
                                                .GetMethods()
                                                .FirstOrDefault(e => e.Name.Equals(actionName, StringComparison.InvariantCultureIgnoreCase));

                var param = method.GetParameters();
                action = new DelegateAction(path, repository, method, param);
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

        public async Task<IEnumerable<object>> ParseParameters(HttpRequest req, IEnumerable<ActionParameter> param)
        {
            var args = new List<object>();
            if (param.Any())
            {
                if (req.Method == "GET" || req.Method == "DELETE")
                {
                    foreach (var parameterInfo in param)
                    {
                        bool found = false;
                        foreach (var queryParam in req.Query)
                        {
                            if (queryParam.Key.ToLower() == parameterInfo.Name)
                            {
                                var val = $"\"{queryParam.Value}\"";
                                var json = JToken.Parse(val);
                                args.Add(json.ToObject(parameterInfo.Type));
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
                    JObject bodyParams;
                    using (var sr = new StreamReader(req.Body))
                    {
                        var text = await sr.ReadToEndAsync();
                        bodyParams = JObject.Parse(text);
                    }
                    if (param.Count() == 1)
                        args.Add(bodyParams.ToObject(param.ElementAt(0).Type));
                    else
                        foreach (var parameterInfo in param)
                        {
                            if (bodyParams.TryGetValue(parameterInfo.Name, StringComparison.InvariantCultureIgnoreCase, out JToken value))
                            {
                                args.Add(value.ToObject(parameterInfo.Type));
                            }
                        }
                }
            }
            return args;
        }
    }
}
