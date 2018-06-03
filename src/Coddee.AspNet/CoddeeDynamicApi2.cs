using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Coddee.Data;
using Coddee.Loggers;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;

namespace Coddee.AspNet
{
    /// <summary>
    /// Coddee dynamic API implementation.
    /// </summary>
    public class CoddeeDynamicApi2 : IMiddleware
    {
        private const string _eventsSource = "CoddeeDynamicAPI";
        private readonly DynamicApiParametersParser _parser;
        private readonly DynamicApiControllersManager _controllersManager;
        private readonly DynamicApiConfigurations _configurations;
        private readonly LogAggregator _logger;
        private readonly ApiActionsCache _cache;
        private readonly IRepositoryManager _repositoryManager;
        private long _lastId;

        /// <inheritdoc />
        public CoddeeDynamicApi2(IContainer container)
        {
            _configurations = container.IsRegistered<DynamicApiConfigurations>() ?
                                  container.Resolve<DynamicApiConfigurations>() :
                                  DynamicApiConfigurations.Default;

            _cache = new ApiActionsCache();
            _parser = new DynamicApiParametersParser();

            if (container.IsRegistered<DynamicApiControllersManager>())
                _controllersManager = container.Resolve<DynamicApiControllersManager>();

            if (container.IsRegistered<IRepositoryManager>())
                _repositoryManager = container.Resolve<IRepositoryManager>();

            if (container.IsRegistered<ILogger>())
            {
                var logger = container.Resolve<ILogger>();
                if (logger is LogAggregator logAggregator)
                    _logger = logAggregator;
            }
            else
            {
                _logger = new LogAggregator();
            }
            
        }

        /// <inheritdoc />
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            Log($"Request received for path '{context.Request.Path}'");

            if (ValidateRequest(context))
            {
                var request = CreateApiRequest(context);
                Log(request, $"Request is valid, requesting [Controller:{request.RequestedActionPath.RequestedController}] [Action:{request.RequestedActionPath.RequestedAction}]");


                IDynamicApiAction action = _cache.GetAction(request);
                Log(request, $"Checking cache for action.");

                if (action == null)
                {
                    Log(request, $"Action not found in cache.");
                    Log(request, $"Looking in repository actions.");

                    action = CreateRepositoryAction(request);
                    if (action == null)
                        throw new DynamicApiException("Action not found in action repository.");

                    Log(request, $"Repository action created.");

                }
                else
                {
                    Log(request, $"Action found in cache.");
                }

                try
                {
                    Log(request, $"Parsing request parameters.");
                    var parameters = await _parser.ParseParameters(action, request);

                    Log(request, $"Invoking action.");
                    var value = await action.Invoke(parameters);
                    await context.Response.WriteAsync(JsonConvert.SerializeObject(value));
                }
                catch (Exception e)
                {
                    Log(request, e);
                    throw;
                }
            }
            else
            {
                await next(context);
            }
        }

        private IDynamicApiAction CreateRepositoryAction(DynamicApiRequest request)
        {
            if (_repositoryManager == null)
                return null;

            var repositoryName = request.RequestedActionPath.RequestedController;
            var repository = _repositoryManager.GetRepository(repositoryName);
            if (repository == null)
                return null;

            var actionName = request.RequestedActionPath.RequestedAction;
            if (actionName == "getitem")
                actionName = "get_item";

            var method = repository
                         .GetType()
                         .GetMethods()
                         .FirstOrDefault(e => e.Name.Equals(actionName, StringComparison.InvariantCultureIgnoreCase));

            var interfaceMethod = GetInterfaceMethod(repository, actionName);

            if (method == null || interfaceMethod == null)
                return null;

            var action = new DynamicApiRepositoryAction
            {
                ReturnType = interfaceMethod.ReturnType,
                RepositoryManager = _repositoryManager,
                Method = method,
                RepositoryName = repositoryName,
                Parameters = DynamicApiActionParameter.GetParameters(interfaceMethod),
                Path = new DynamicApiActionPath(repositoryName, actionName),
                ReturnsValue = interfaceMethod.ReturnType != typeof(Task) && interfaceMethod.ReturnType != typeof(void),
            };

            return action;
        }

        private static MethodInfo GetInterfaceMethod(IRepository repository, string actionName)
        {
            MethodInfo GetMethodFromType(Type type)
            {
                return type.GetMethods()
                       .FirstOrDefault(e => e.Name.Equals(actionName, StringComparison.InvariantCultureIgnoreCase));
            }

            var method = GetMethodFromType(repository.ImplementedInterface);
            if (method == null)
            {
                foreach (var interf in repository.ImplementedInterface.GetInterfaces())
                {
                    method = GetMethodFromType(interf);
                    if (method != null)
                        return method;
                }
            }
            return null;
        }

        /// <summary>
        /// Add the controllers to the cache.
        /// </summary>
        public void CacheRegisteredControllers()
        {
            if (_controllersManager != null)
                _controllersManager.GetRegisteredAction().ForEach(_cache.AddAction);
        }

        private void Log(string content, LogRecordTypes logType = LogRecordTypes.Information)
        {
            _logger.Log(_eventsSource, content, logType);
        }
        private void Log(DynamicApiRequest request, string content, LogRecordTypes logType = LogRecordTypes.Information)
        {
            Log($"[Request Id:{request.Id}] {content}", logType);
        }
        private void Log(DynamicApiRequest request, Exception exception)
        {
            Log(request, exception.BuildExceptionString(), LogRecordTypes.Error);
        }

        private DynamicApiRequest CreateApiRequest(HttpContext context)
        {
            var path = context.Request.Path.ToString().Split('/');
            SetHeaders(context.Response);
            return new DynamicApiRequest
            {
                HttpContext = context,
                Date = DateTime.Now,
                RequestedActionPath = new DynamicApiActionPath(path[2], path[3]),
                Id = Interlocked.Increment(ref _lastId)
            };
        }

        private void SetHeaders(HttpResponse response)
        {
            response.Headers.Add("X-Powered-By", "Coddee Dynamic API");
            response.Headers.Add("Content-Type", "application/json");
        }

        private bool ValidateRequest(HttpContext context)
        {
            return context.Request.Path.StartsWithSegments(_configurations.RoutePrefix);
        }
    }

    /// <summary>
    /// Contains the information of a dynamic API request
    /// </summary>
    public class DynamicApiRequest
    {
        /// <summary>
        /// The time the request was received.
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// The <see cref="HttpContext"/> object.
        /// </summary>
        public HttpContext HttpContext { get; set; }

        /// <summary>
        /// Contains the requested controller and action information.
        /// </summary>
        public DynamicApiActionPath RequestedActionPath { get; set; }

        /// <summary>
        /// An id to Identify the request.
        /// </summary>
        public long Id { get; set; }
    }

    /// <summary>
    /// Configurations object for <see cref="CoddeeDynamicApi2"/>
    /// </summary>
    public class DynamicApiConfigurations
    {
        /// <summary>
        /// The default configurations values.
        /// </summary>
        public static DynamicApiConfigurations Default = new DynamicApiConfigurations
        {
            RoutePrefix = "/dapi"
        };

        /// <summary>
        /// Determine that start segment of the API URL.
        /// </summary>
        public string RoutePrefix { get; set; }
    }


    /// <summary>
    /// Keeps instances of used ApiActions.
    /// </summary>
    public class ApiActionsCache
    {
        /// <inheritdoc />
        public ApiActionsCache()
        {
            _actions = new Dictionary<string, IDynamicApiAction>();
        }

        private readonly Dictionary<string, IDynamicApiAction> _actions;

        /// <summary>
        /// Returns an instance of the <see cref="IDynamicApiAction"/> if found and <see langword="null"/> if not.
        /// </summary>
        /// <param name="request">The request object.</param>
        public IDynamicApiAction GetAction(DynamicApiRequest request)
        {
            var key = GetActionKey(request.RequestedActionPath);
            if (_actions.TryGetValue(key, out var action))
            {
                return action;
            }
            return null;
        }

        /// <summary>
        /// Adds an action to the cache.
        /// </summary>
        public void AddAction(IDynamicApiAction action)
        {
            var key = GetActionKey(action.Path);
            if (!_actions.ContainsKey(key))
                _actions.Add(key, action);
        }

        private static string GetActionKey(DynamicApiActionPath actionPath)
        {
            return $"{actionPath.RequestedController}:{actionPath.RequestedAction}";
        }
    }

    /// <summary>
    /// Contains the route information of a <see cref="DynamicApiRequest"/>
    /// </summary>
    public class DynamicApiActionPath
    {
        /// <inheritdoc />
        public DynamicApiActionPath(string requestedController, string requestedAction)
        {
            RequestedController = requestedController.ToLower();
            RequestedAction = requestedAction.ToLower();
        }

        /// <summary>
        /// Controller that requested by the URL.
        /// </summary>
        public string RequestedController { get; }

        /// <summary>
        /// Action that requested by the URL.
        /// </summary>
        public string RequestedAction { get; }

    }

    /// <summary>
    /// Represents an action that can be invoked by the <see cref="CoddeeDynamicApi2"/>
    /// </summary>
    public interface IDynamicApiAction
    {
        /// <summary>
        /// The route path of the action.
        /// </summary>
        DynamicApiActionPath Path { get; set; }

        /// <summary>
        /// Action parameters.
        /// </summary>
        List<DynamicApiActionParameter> Parameters { get; set; }

        /// <summary>
        /// Invoke the action.
        /// </summary>
        Task<object> Invoke(DynamicApiActionParameterValue[] parametersValue);

        /// <summary>
        /// Indicates whether the action returns a value.
        /// </summary>
        bool ReturnsValue { get; set; }
    }

    /// <summary>
    /// Discovers and registers controllers actions.
    /// </summary>
    public class DynamicApiControllersManager
    {
        private readonly List<IDynamicApiAction> _actions;

        /// <inheritdoc />
        public DynamicApiControllersManager()
        {
            _actions = new List<IDynamicApiAction>();
        }

        /// <summary>
        /// Returns the registered actions.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<IDynamicApiAction> GetRegisteredAction()
        {
            return _actions.AsReadOnly();
        }

        /// <summary>
        /// Register a controller and it's actions.
        /// </summary>
        public void RegisterController(Type type)
        {
            var actions = type.GetMethods().Where(e => Attribute.IsDefined(e, typeof(ApiActionAttribute)));
            foreach (var action in actions)
            {
                var attribute = action.GetCustomAttribute<ApiActionAttribute>();
                var path = attribute.Path.Split('/');
                var controllerAction = new DynamicApiControllersAction
                {
                    Path = new DynamicApiActionPath(path[0], path[1]),
                    ControllerType = type,
                    Method = action,
                    Parameters = DynamicApiActionParameter.GetParameters(action),
                    ReturnsValue = action.ReturnType != typeof(Task) && action.ReturnType != typeof(void),
                    ReturnType = action.ReturnType
                };
                _actions.Add(controllerAction);
            }
        }
    }

    /// <summary>
    /// A base implementation for <see cref="IDynamicApiAction"/>.
    /// </summary>
    public abstract class DynamicApiActionBase : IDynamicApiAction
    {
        private PropertyInfo _taskResult;

        /// <inheritdoc />
        public DynamicApiActionPath Path { get; set; }

        /// <inheritdoc />
        public List<DynamicApiActionParameter> Parameters { get; set; }

        /// <inheritdoc />
        public async Task<object> Invoke(DynamicApiActionParameterValue[] parametersValue)
        {
            object[] parameters = null;
            parametersValue = parametersValue.OrderBy(e => e.Parameter.Index).ToArray();
            var instance = GetInstnaceOwner();
            if (parametersValue.Any())
            {
                parameters = new object[parametersValue.Length];
                for (int i = 0; i < parametersValue.Length; i++)
                {
                    parameters[i] = parametersValue[i].Value;
                }
            }

            var result = Method.Invoke(instance, parameters);

            if (result is Task task)
            {
                await task;
                if (ReturnsValue)
                {
                    if (_taskResult == null)
                        _taskResult = result.GetType().GetProperty(nameof(Task<object>.Result));
                    result = _taskResult.GetValue(task);
                }
            }
            return result;
        }

        /// <summary>
        /// Get an instance of the owner type to invoke the action.
        /// </summary>
        /// <returns></returns>
        protected abstract object GetInstnaceOwner();

        /// <inheritdoc />
        public bool ReturnsValue { get; set; }

        /// <summary>
        /// The action return type.
        /// </summary>
        public Type ReturnType { get; set; }


        /// <summary>
        /// The method info object.
        /// </summary>
        public MethodInfo Method { get; set; }
    }

    /// <summary>
    /// An <see cref="IDynamicApiAction"/> implementation for repository actions.
    /// </summary>
    public class DynamicApiRepositoryAction : DynamicApiActionBase
    {
        /// <summary>
        /// The name of the repository.
        /// </summary>
        public string RepositoryName { get; set; }

        /// <summary>
        /// The used repository manager instance.
        /// </summary>
        public IRepositoryManager RepositoryManager { get; set; }

        /// <inheritdoc />
        protected override object GetInstnaceOwner()
        {
            return RepositoryManager.GetRepository(RepositoryName);
        }
    }

    /// <summary>
    /// An <see cref="IDynamicApiAction"/> implementation for controllers
    /// </summary>
    public class DynamicApiControllersAction : DynamicApiActionBase
    {
        /// <summary>
        /// They type of the controller.
        /// </summary>
        public Type ControllerType { get; set; }

        /// <inheritdoc />
        protected override object GetInstnaceOwner()
        {
            return Activator.CreateInstance(ControllerType);
        }
    }

    /// <summary>
    /// <see cref="IDynamicApiAction"/> parameters.
    /// </summary>
    public class DynamicApiActionParameter
    {
        /// <inheritdoc />
        public DynamicApiActionParameter(ParameterInfo param)
        {
            Name = param.Name.ToLower();
            Type = param.ParameterType;
            Index = param.Position;
        }

        /// <summary>
        /// Parameter name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Parameter type.
        /// </summary>
        public Type Type { get; set; }

        /// <summary>
        /// The index of the parameter.
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        /// Returns the parameters of a specific method.
        /// </summary>
        /// <param name="method"></param>
        /// <returns></returns>
        public static List<DynamicApiActionParameter> GetParameters(MethodInfo method)
        {
            return method.GetParameters().Select(e => new DynamicApiActionParameter(e)).ToList();
        }
    }

    /// <summary>
    /// <see cref="IDynamicApiAction"/> parameters values.
    /// </summary>
    public class DynamicApiActionParameterValue
    {
        /// <summary>
        /// The parameter object.
        /// </summary>
        public DynamicApiActionParameter Parameter { get; set; }

        /// <summary>
        /// The parameter value.
        /// </summary>
        public object Value { get; set; }

    }

    /// <summary>
    /// Parses the request parameters.
    /// </summary>
    public class DynamicApiParametersParser
    {
        /// <summary>
        /// Parse request parameters.
        /// </summary>
        public async Task<DynamicApiActionParameterValue[]> ParseParameters(IDynamicApiAction action, DynamicApiRequest request)
        {
            var parameters = new DynamicApiActionParameterValue[action.Parameters.Count];
            var httpRequest = request.HttpContext.Request;


            for (int i = 0; i < action.Parameters.Count; i++)
            {
                var actionParameter = action.Parameters[i];
                bool found = false;
                if (HttpMethod.IsGet(httpRequest.Method) || HttpMethod.IsDelete(httpRequest.Method))
                {

                    if (httpRequest.Query.ContainsKey(actionParameter.Name))
                    {
                        var value = ParseGetValue(actionParameter, httpRequest.Query[actionParameter.Name]);
                        var paramValue = new DynamicApiActionParameterValue
                        {
                            Parameter = actionParameter,
                            Value = value
                        };
                        parameters[i] = paramValue;
                        found = true;
                    }
                }
                else
                {
                    string content;
                    using (var sr = new StreamReader(httpRequest.Body))
                    {
                        content = await sr.ReadToEndAsync();
                    }
                    var value = ParsePostValue(actionParameter, content);
                    var paramValue = new DynamicApiActionParameterValue
                    {
                        Parameter = actionParameter,
                        Value = value
                    };
                    parameters[i] = paramValue;
                    found = true;
                }

                if (!found)
                    throw new DynamicApiException($"Parameter {actionParameter.Name} is missing.");
            }



            return parameters;
        }

        private object ParsePostValue(DynamicApiActionParameter actionParameter, string content)
        {
            return JsonConvert.DeserializeObject(content, actionParameter.Type);
        }

        private object ParseGetValue(DynamicApiActionParameter actionParameter, StringValues queryParam)
        {
            var converter = TypeDescriptor.GetConverter(actionParameter.Type);
            return converter.ConvertFrom(queryParam.ToString());
        }
    }

    /// <summary>
    /// Dynamic API Exception.
    /// </summary>
    [Serializable]
    public class DynamicApiException : Exception
    {
        /// <inheritdoc />
        public DynamicApiException()
        {
        }

        /// <inheritdoc />
        public DynamicApiException(string message) : base(message)
        {
        }

        /// <inheritdoc />
        public DynamicApiException(string message, Exception inner) : base(message, inner)
        {
        }

        /// <inheritdoc />
        protected DynamicApiException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }

    /// <summary>
    /// HTTP methods.
    /// </summary>
    public static class HttpMethod
    {
        /// <summary>
        /// GET Http method.
        /// </summary>
        public const string Get = "GET";
        /// <summary>
        /// POST Http method.
        /// </summary>
        public const string Post = "POST";
        /// <summary>
        /// PUT Http method.
        /// </summary>
        public const string Put = "PUT";
        /// <summary>
        /// DELETE Http method.
        /// </summary>
        public const string Delete = "DELETE";

        /// <summary>
        /// Compares the method to GET method.
        /// </summary>
        public static bool IsGet(string method)
        {
            return method.ToUpper() == Get;
        }

        /// <summary>
        /// Compares the method to POST method.
        /// </summary>

        public static bool IsPost(string method)
        {
            return method.ToUpper() == Post;
        }

        /// <summary>
        /// Compares the method to PUT method.
        /// </summary>
        public static bool IsPut(string method)
        {
            return method.ToUpper() == Put;
        }

        /// <summary>
        /// Compares the method to DELETE method.
        /// </summary>
        public static bool IsDelete(string method)
        {
            return method.ToUpper() == Delete;
        }
    }
}