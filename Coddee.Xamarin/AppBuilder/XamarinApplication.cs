using System;
using Coddee.AppBuilder;
using Coddee.Loggers;
using Xamarin.Forms;

namespace Coddee.Xamarin.AppBuilder
{
    /// <summary>
    /// The Xamarin application wrapper
    /// Extend the functionality of the regular Xamarin.Form Application class
    /// </summary>
    public class XamarinApplication : IApplication
    {
        private const string _eventsSource = "Application";

        public XamarinApplication(Guid applicationID, string applicationName, IContainer container)
        {
            ApplicationID = applicationID;
            ApplicationName = applicationName;
            ApplicationType = ApplicationTypes.Xamarin;
            _container = container;
        }

        public XamarinApplication(string applicationName, IContainer container)
            : this(Guid.NewGuid(), applicationName, container)
        {
        }

        public Guid ApplicationID { get; }
        public string ApplicationName { get; }
        public ApplicationTypes ApplicationType { get; }
        /// <summary>
        /// Dependency container
        /// </summary>
        protected IContainer _container;

        /// <summary>
        /// The base Application class instance
        /// </summary>
        protected Application _systemApplication;
        private ILogger _logger;

        public static XamarinApplication Current { get; protected set; }

        /// <summary>
        /// Returns the system application
        /// </summary>
        /// <returns></returns>
        public Application GetSystemApplication()
        {
            return _systemApplication;
        }

        public IContainer GetContainer()
        {
            return _container;
        }
        public void Run(Action<IXamarinApplicationBuilder> BuildApplication)
        {
            Current = this;
            _systemApplication = Application.Current;
            _container.RegisterInstance<IApplication>(this);
            _container.RegisterInstance(this);
            var builder = _container.Resolve<XamarinApplicationBuilder>();
            _logger = _container.Resolve<ILogger>();
            LogStart();
            BuildApplication(builder);
            builder.Start();
        }

        protected virtual void LogStart()
        {
            _logger.Log(_eventsSource, $"Initializing application {ApplicationName}");
        }
    }
}