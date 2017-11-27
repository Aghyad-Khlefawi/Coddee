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

        public CoddeeDynamicApi(RequestDelegate next, IRepositoryManager repositoryManager)
        {
            _next = next;
            _repositoryManager = repositoryManager;
        }

        IRepository GetRepositoryByName(string name)
        {
            var repositories = _repositoryManager.GetRepositories();
            foreach (var repository in repositories)
            {
                var reportAttr = repository.GetType().GetCustomAttribute<RepositoryAttribute>();
                var interType = reportAttr.ImplementedRepository;
                var nameAttr = interType.GetCustomAttribute<NameAttribute>();
                if (nameAttr != null && nameAttr.Value.Equals(name, StringComparison.InvariantCultureIgnoreCase))
                    return repository;


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
                var path = req.Path.Value.Split('/');
                var repositoryName = path[2];
                var actionName = path[3];
                
                var repository = GetRepositoryByName(repositoryName);
                if (repository != null)
                {
                    var action = repository.GetType().GetMethods().FirstOrDefault(e => e.Name.Equals(actionName, StringComparison.InvariantCultureIgnoreCase));
                    if (action != null)
                    {
                        var param = action.GetParameters();
                        IEnumerable<object> args;

                        try
                        {
                            args = await ParseParameters(req, param);
                        }
                        catch (APIException ex)
                        {
                            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                            await context.Response.WriteAsync(ex.Message);
                            return true;
                        }

                        if (action.ReturnType == typeof(Task))
                        {
                            await ((Task)action.Invoke(repository, args.ToArray()));
                            context.Response.StatusCode = (int)HttpStatusCode.OK;
                            return true;
                        }
                        if (action.ReturnType.GenericTypeArguments.Any())
                        {
                            dynamic task = ((Task)action.Invoke(repository, args.ToArray()));
                            object res = (object)task.Result;
                            context.Response.StatusCode = (int)HttpStatusCode.OK;
                            await context.Response.WriteAsync(JsonConvert.SerializeObject(res));
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        public async Task<IEnumerable<object>> ParseParameters(HttpRequest req, ParameterInfo[] param)
        {
            var args = new List<object>();
            if (param.Any())
            {
                if (req.Method == "POST")
                {
                    JObject bodyParams;
                    using (var sr = new StreamReader(req.Body))
                    {
                        var text = await sr.ReadToEndAsync();
                        bodyParams = JObject.Parse(text);
                    }

                    if (param.Length == 1)
                        args.Add(bodyParams.ToObject(param[0].ParameterType));
                    else
                        foreach (var parameterInfo in param)
                        {
                            if (bodyParams.TryGetValue(parameterInfo.Name, StringComparison.InvariantCultureIgnoreCase, out JToken value))
                            {
                                args.Add(value.ToObject(parameterInfo.ParameterType));
                            }
                        }
                }
                else
                {
                    foreach (var parameterInfo in param)
                    {
                        var queryItemExists = req.Query.Any(e => e.Key.Equals(parameterInfo.Name, StringComparison.InvariantCultureIgnoreCase));
                        if (queryItemExists)
                        {
                            var val = $"\"{req.Query.First(e => e.Key.Equals(parameterInfo.Name, StringComparison.InvariantCultureIgnoreCase)).Value.ToString()}\"";
                            var json = JToken.Parse(val);
                            args.Add(json.ToObject(parameterInfo.ParameterType));
                        }
                        else
                        {
                            throw new APIException(0, $"Missing parameters '{parameterInfo.Name}'");
                        }
                    }
                }
            }
            return args;
        }
    }
}
