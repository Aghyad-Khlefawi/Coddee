﻿using System;
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
        private readonly PagesProvider _pagesProvider;
        private readonly RepositoryActionLoactor _repositoryActionLoactor;
        private long _lastId;

        /// <inheritdoc />
        public DynamicApi(IContainer container)
        {
            _configurations = container.Resolve<DynamicApiConfigurations>();

            _dateTimeConverter = new IsoDateTimeConverter
            {
                DateTimeFormat = _configurations.DateTimeForamt
            };

            _pagesProvider = new PagesProvider();
            _cache = new ApiActionsCache();
            _parser = new DynamicApiParametersParser(_dateTimeConverter);
            _authorizationValidator = _configurations.AuthorizationValidator;
            _repositoryActionLoactor = new RepositoryActionLoactor();


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
                    if (IsLoggingPageRequest(context))
                    {
                        await ShowLogPage(context);
                        return;
                    }
                }

                var request = CreateApiRequest(context);
                Log(request, $"Request is valid, requesting [Controller:{request.RequestedActionPath.RequestedController}] [Action:{request.RequestedActionPath.RequestedAction}]");


                DynamicApiException exception = null;

                try
                {
                    await Task.Run(() => HandleRequest(request));
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
                    await HandleException(request, exception);
            }

            else
            {
                await next(context);
            }
        }

        private bool IsLoggingPageRequest(HttpContext context)
        {
            return context.Request.Path.ToString() == $"{_configurations.RoutePrefix}{_configurations.LoggingPageRoute}";
        }

        private async Task HandleRequest(DynamicApiRequest request)
        {
            IDynamicApiAction action = _cache.GetAction(request);
            Log(request, $"Checking cache for action.");

            if (action == null)
            {
                Log(request, $"Action not found in cache.");
                Log(request, $"Looking in repository actions.");
                action = _repositoryActionLoactor.CreateRepositoryAction(_repositoryManager, request);
                if (action == null)
                    throw new DynamicApiException(DynamicApiExceptionCodes.ActionNotFound, "Action not found.", request);
                _cache.AddAction(action);
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

            var context = _configurations.GetApiContext(request);
            Log(request, $"Context: {context}.");

            var resLength = await InvokeAction(request, action, parameters, context);

            Log(request, $"Response completed in {(DateTime.Now - request.Date).Milliseconds} ms, content size:{SizeToString(resLength)}");
        }

        private string SizeToString(long resLength)
        {
            const int kb = 1024;
            const int mb = 1024 * 1024;
            const int gb = 1024 * 1024 * 1024;

            if (resLength < kb)
            {
                return $"{resLength} B";
            }
            if (resLength < mb)
            {
                return $"{(float)resLength / kb} KB";
            }
            if (resLength < gb)
            {
                return $"{(float)resLength / mb} MB";
            }

            return $"{resLength} B";
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

        private async Task HandleException(DynamicApiRequest request, DynamicApiException exception)
        {
            Log(exception);
            var statusCode = request.HttpContext.Response.StatusCode = GetExceptionStatusCode(exception);
            request.HttpContext.Response.Headers.Add("X-Coddee-Exception", exception.Code.ToString());

            if (_configurations.UseErrorPages)
            {
                string content = null;
                if (_configurations.ErrorPagesConfiguration.DisplayExceptionDetailes)
                    content = exception.BuildExceptionString(debuginfo: true);
                string page = _pagesProvider.GetErrorPage(statusCode, content);
                await request.HttpContext.Response.WriteAsync(page);
            }
            else if (_configurations.ReturnException)
            {
                await request.HttpContext.Response.WriteAsync(JsonConvert.SerializeObject(exception));
            }
        }



        private int GetExceptionStatusCode(DynamicApiException exception)
        {
            switch (exception.Code)
            {
                case DynamicApiExceptionCodes.Unauthorized:
                    return StatusCodes.Status401Unauthorized;
                case DynamicApiExceptionCodes.ActionNotFound:
                    return StatusCodes.Status404NotFound;
                case DynamicApiExceptionCodes.MissingParameter:
                    return StatusCodes.Status400BadRequest;
                default:
                    return StatusCodes.Status500InternalServerError;
            }
        }

        private async Task<long> InvokeAction(DynamicApiRequest request, IDynamicApiAction action, DynamicApiActionParameterValue[] parameters, object context)
        {
            var value = await action.Invoke(parameters, context);
            var res = JsonConvert.SerializeObject(value, _dateTimeConverter);
            var response = request.HttpContext.Response;
            response.Headers.Add("Content-Type", "application/json");
            await response.WriteAsync(res);
            return Encoding.UTF8.GetByteCount(res);
        }



        /// <summary>
        /// Add the controllers to the cache.
        /// </summary>
        public void CacheActions()
        {
            if (_controllersManager != null)
                _controllersManager.GetRegisteredAction().ForEach(_cache.AddAction);

            if (_configurations.CacheRepositoryActionsOnStartup)
            {
                CacheRepositoryActions();
            }
        }

        private void CacheRepositoryActions()
        {
            if (_repositoryManager == null)
                return;

            foreach (var repository in _repositoryManager.GetRepositories())
            {
                foreach (var repositoryAction in _repositoryActionLoactor.GetRepositoryActions(_repositoryManager, repository))
                {
                    _cache.AddAction(repositoryAction);
                }
            }
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
            response.Headers.Add("X-Coddee-DAPI", "v1");
        }

        private bool ValidateRequest(HttpContext context)
        {
            return context.Request.Path.StartsWithSegments(_configurations.RoutePrefix);
        }
    }
}