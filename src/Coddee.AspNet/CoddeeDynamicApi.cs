using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using Coddee.Attributes;
using Coddee.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Coddee.AspNet
{
    public class CoddeeDynamicApi
    {
        private readonly RequestDelegate _next;
        private readonly IRepositoryManager _repositoryManager;

        public CoddeeDynamicApi(RequestDelegate next, IRepositoryManager repositoryManager, CoddeeControllersManager controllersManager)
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
                var pathParts = req.Path.Value.Split('/').Skip(2);
                var repositoryName = pathParts.ElementAt(0);
                var actionName = pathParts.ElementAt(1);
                var path = pathParts.Combine("/").ToLower();

                IApiAction action = null;
                if (_apiActions.ContainsKey(path))
                    action = _apiActions[path];
                else
                {
                    var repository = GetRepositoryByName(repositoryName);
                    if (repository != null)
                    {
                        var method = repository.GetType().GetMethods().FirstOrDefault(e => e.Name.Equals(actionName, StringComparison.InvariantCultureIgnoreCase));
                        var param = method.GetParameters();
                        action = new DelegateAction(path, repository, method, param);
                        _apiActions.Add(path, action);
                    }
                }
                if (action != null)
                {

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
                            var val = $"\"{req.Query.First(e => e.Key.Equals(parameterInfo.Name, StringComparison.InvariantCultureIgnoreCase)).Value}\"";
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

    public interface IApiAction
    {
        string Path { get; set; }
        Task<object> Invoke(IEnumerable<object> param);
        bool RetrunsValue { get; }
        ParameterInfo[] ParametersInfo { get; set; }
    }

    public class DelegateAction : IApiAction
    {
        public DelegateAction(string path, object owner, MethodInfo method)
        {
            Path = path;
            Owner = owner;
            Method = method;
            ReturnType = method.ReturnType;
            RetrunsValue = ReturnType != typeof(Task);
        }

        public DelegateAction(string path, object owner, MethodInfo method, ParameterInfo[] parametersInfo)
            : this(path, owner, method)
        {
            ParametersInfo = parametersInfo;
        }

        public object Owner { get; set; }
        public MethodInfo Method { get; set; }
        public ParameterInfo[] ParametersInfo { get; set; }
        public Type ReturnType { get; set; }
        public bool RetrunsValue { get; set; }
        public string Path { get; set; }
        public async Task<object> Invoke(IEnumerable<object> param)
        {
            if (!RetrunsValue)
                await (Task)Method.Invoke(Owner, param.ToArray());
            else
            {
                dynamic task = ((Task)Method.Invoke(Owner, param.ToArray()));
                return (object)task.Result;
            }
            return null;
        }

    }

    public class CoddeeControllersManager
    {
        private readonly IServiceCollection _container;

        public CoddeeControllersManager(IServiceCollection container)
        {
            _container = container;
            _apiActions = new Dictionary<string, IApiAction>();
            _controllerTypes = new List<Type>();
        }

        private readonly List<Type> _controllerTypes;
        private readonly Dictionary<string, IApiAction> _apiActions;

        public Dictionary<string, IApiAction> GetRegisteredActions()
        {
            foreach (var type in _controllerTypes)
            {
                var actionsInfo = type.GetMethods().Where(e => Attribute.IsDefined(e, typeof(ApiActionAttribute)));
                var controller = _container.BuildServiceProvider().GetService(type);
                foreach (var memberInfo in actionsInfo)
                {
                    var paths = memberInfo.GetCustomAttributes<ApiActionAttribute>().Select(e => e.Path).ToList();
                    foreach (var path in paths)
                    {
                        var pathLower = path.ToLower();
                        var delegateAction = new DelegateAction(pathLower, controller, memberInfo, memberInfo.GetParameters());
                        _apiActions.Add(pathLower, delegateAction);
                    }
                }
            }
            return _apiActions;
        }

        public void RegisterController<T>() where T : class
        {
            var type = typeof(T);
            _controllerTypes.Add(type);
            _container.AddTransient<T>();
        }
    }

    [AttributeUsage(AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
    public sealed class ApiActionAttribute : Attribute
    {
        public ApiActionAttribute(string path)
            : this(path, HttpMethod.Get)
        {

        }

        public ApiActionAttribute(string path, HttpMethod httpMethod)
        {
            Path = path;
            HttpMethod = httpMethod;
        }

        public string Path { get; }
        public HttpMethod HttpMethod { get; set; }


    }
}
