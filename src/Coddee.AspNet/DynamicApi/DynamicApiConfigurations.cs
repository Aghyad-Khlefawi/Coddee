using System;

namespace Coddee.AspNet
{
    /// <summary>
    /// Configurations object for <see cref="DynamicApi"/>
    /// </summary>
    public class DynamicApiConfigurations
    {
        /// <summary>
        /// The default configurations values.
        /// </summary>
        public static DynamicApiConfigurations Default() => new DynamicApiConfigurations
        {
            RoutePrefix = "/dapi",
            LoggingPageRoute = "/__log",
            DateTimeFormat = "dd/MM/yyyy HH:mm:ss",
            UseLoggingPage = false,
            ReturnException = false,
            UseErrorPages = false,
            CacheRepositoryActionsOnStartup = false,
            ErrorPagesConfiguration = new ErrorPagesConfiguration
            {
                DisplayExceptionDetails = true
            },
            CorsAllowedHeaders = "Content-Type,Authorization",
            CorsAllowedMethods = "GET,POST",
            CorsAllowedOrigin = "*"
        };

        /// <summary>
        /// Determine that start segment of the API URL.
        /// </summary>
        public string RoutePrefix { get; set; }

        /// <summary>
        /// The date and time format used to serialize and deserialize objects.
        /// </summary>
        public string DateTimeFormat { get; set; }

        /// <summary>
        /// An instance of a <see cref="IAuthorizationValidator"/>
        /// </summary>
        public IAuthorizationValidator AuthorizationValidator { get; set; }

        /// <summary>
        /// If true a page will display the API log on <see cref="LoggingPageRoute"/>
        /// </summary>
        public bool UseLoggingPage { get; set; }

        /// <summary>
        /// The route that will display the API log if <see cref="UseLoggingPage"/> is set to true.
        /// <remarks>This route will be appended to <see cref="RoutePrefix"/></remarks>
        /// </summary>
        public string LoggingPageRoute { get; set; }

        /// <summary>
        /// If set to true the exception object will be returned to the client.
        /// <remarks>Doesn't work if <see cref="UseErrorPages"/> is true</remarks>
        /// </summary>
        public bool ReturnException { get; set; }

        /// <summary>
        /// If true an HTML error page will be returned to the client.
        /// </summary>
        public bool UseErrorPages { get; set; }

        /// <summary>
        /// Configurations for error pages.
        /// </summary>
        public ErrorPagesConfiguration ErrorPagesConfiguration { get; set; }


        /// <summary>
        /// If set to true then the repository actions will be cached on the application startup.
        /// </summary>
        public bool CacheRepositoryActionsOnStartup { get; set; }

        /// <summary>
        /// Called before calling an action to give context to the action owner.
        /// </summary>
        public Func<DynamicApiRequest, object> GetApiContext { get; set; }

        /// <summary>
        /// When set to true CORS allow headers will be added to the response
        /// </summary>
        public bool UseCors { get; set; }

        /// <summary>
        /// Cors options
        /// </summary>
        public string CorsAllowedHeaders { get; set; }

        /// <summary>
        /// Cors options
        /// </summary>
        public string CorsAllowedMethods { get; set; }

        /// <summary>
        /// Cors options
        /// </summary>
        public string CorsAllowedOrigin { get; set; }
    }

    /// <summary>
    /// Configurations object for error pages.
    /// </summary>
    public class ErrorPagesConfiguration
    {
        /// <summary>
        /// If true the exception details will be shown in the error page.
        /// </summary>
        public bool DisplayExceptionDetails { get; set; }
    }
}