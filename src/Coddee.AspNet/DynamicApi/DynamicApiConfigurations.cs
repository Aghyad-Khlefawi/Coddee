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
        public static DynamicApiConfigurations Default => new DynamicApiConfigurations
        {
            RoutePrefix = "/dapi",
            LoggingPageRoute = "/__log",
            DateTimeForamt = "dd/MM/yyyy HH:mm:ss",
            UseLoggingPage = false
        };

        /// <summary>
        /// Determine that start segment of the API URL.
        /// </summary>
        public string RoutePrefix { get; set; }

        /// <summary>
        /// The date and time format used to serialize and deserialize objects.
        /// </summary>
        public string DateTimeForamt { get; set; }

        /// <summary>
        /// An instance of a <see cref="IAuthorizationValidator"/>
        /// </summary>
        public IAuthorizationValidator AuthorizationValdiator { get; set; }

        /// <summary>
        /// If true a page will display the API log on <see cref="LoggingPageRoute"/>
        /// </summary>
        public bool UseLoggingPage { get; set; }

        /// <summary>
        /// The route that will display the API log if <see cref="UseLoggingPage"/> is set to true.
        /// <remarks>This route will be appended to <see cref="RoutePrefix"/></remarks>
        /// </summary>
        public string LoggingPageRoute { get; set; }
    }
}