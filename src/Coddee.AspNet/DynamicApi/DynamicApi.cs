using System;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Coddee.Data;
using Coddee.Loggers;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Coddee.AspNet
{
    /// <summary>
    /// Coddee dynamic API implementation.
    /// </summary>
    public class DynamicApi : IMiddleware
    {
        private const string _eventsSource = "CoddeeDynamicAPI";
        private readonly DynamicApiParametersParser _parser;
        private readonly DynamicApiControllersManager _controllersManager;
        private readonly DynamicApiConfigurations _configurations;
        private readonly LogAggregator _logger;
        private readonly ApiActionsCache _cache;
        private readonly IRepositoryManager _repositoryManager;
        private readonly IsoDateTimeConverter _dateTimeConverter;
        private readonly IAuthorizationValidator _authorizationValidator;
        private long _lastId;

        /// <inheritdoc />
        public DynamicApi(IContainer container)
        {
            _configurations = container.IsRegistered<DynamicApiConfigurations>() ?
                                  container.Resolve<DynamicApiConfigurations>() :
                                  DynamicApiConfigurations.Default;


            _dateTimeConverter = new IsoDateTimeConverter
            {
                DateTimeFormat = _configurations.DateTimeForamt
            };

            _cache = new ApiActionsCache();
            _parser = new DynamicApiParametersParser(_dateTimeConverter);
            _authorizationValidator = _configurations.AuthorizationValdiator;

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

                if (_configurations.UseLoggingPage)
                {
                    if (context.Request.Path.ToString() == $"{_configurations.RoutePrefix}{_configurations.LoggingPageRoute}")
                    {
                        await ShowLogPage(context);
                        return;
                    }
                }

                var request = CreateApiRequest(context);
                Log(request, $"Request is valid, requesting [Controller:{request.RequestedActionPath.RequestedController}] [Action:{request.RequestedActionPath.RequestedAction}]");

                async Task HandleRequest()
                {


                    IDynamicApiAction action = _cache.GetAction(request);
                    Log(request, $"Checking cache for action.");

                    if (action == null)
                    {
                        Log(request, $"Action not found in cache.");
                        Log(request, $"Looking in repository actions.");

                        action = CreateRepositoryAction(request);
                        if (action == null)
                            throw new DynamicApiException(DynamicApiExceptionCodes.ActionNotFound, "Action not found in repository.", request);

                        Log(request, $"Repository action created.");
                    }
                    else
                    {
                        Log(request, $"Action found in cache.");
                    }


                    Log(request, $"Parsing request parameters.");
                    var parameters = await _parser.ParseParameters(action, request);

                    if (action.RequiresAuthorization)
                    {
                        if (!_authorizationValidator.IsAuthorized(action, request))
                            throw new DynamicApiException(DynamicApiExceptionCodes.Unauthorized, "Unauthorized client.", request);
                    }

                    Log(request, $"Invoking action.");
                    await InvokeAction(context, action, parameters);
                    Log(request, $"Response completed in {(DateTime.Now - request.Date).Milliseconds} ms");
                }

                DynamicApiException exception = null;
                try
                {
                    await Task.Run(HandleRequest);
                }
                catch (DynamicApiException dynamicApiException)
                {
                    exception = dynamicApiException;
                }
                catch (Exception e)
                {
                    exception = new DynamicApiException(DynamicApiExceptionCodes.UnknownError, "An error occurred while processing the request.", e, request);
                }

                if (exception != null)
                    await HandleException(exception);
            }

            else
            {
                await next(context);
            }
        }

        private async Task ShowLogPage(HttpContext context)
        {
            var log = new StringBuilder();
            foreach (var loggerRecord in _logger.Records)
            {
                log.Append(loggerRecord);
                log.Append(Environment.NewLine);
            }

            await context.Response.WriteAsync(log.ToString());
        }

        private async Task HandleException(DynamicApiException exception)
        {
            Log(exception);

        }

        private async Task InvokeAction(HttpContext context, IDynamicApiAction action, DynamicApiActionParameterValue[] parameters)
        {
            var value = await action.Invoke(parameters);
            await context.Response.WriteAsync(JsonConvert.SerializeObject(value, _dateTimeConverter));
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
            return method;
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
            Log(request, exception.BuildExceptionString(0, true), LogRecordTypes.Error);
        }
        private void Log(DynamicApiException exception)
        {
            Log(exception.Request, exception);
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
}