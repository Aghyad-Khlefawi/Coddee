using System;
using System.Linq;
using System.Threading;
using Coddee.AppBuilder;
using Coddee.Loggers;
using Coddee.ModuleDefinitions;
using Coddee.Mvvm;
using Coddee.Services;
using Coddee.Xamarin.Common;
using Coddee.Xamarin.Services.ApplicationModulesManager;
using Xamarin.Forms;
using IApplicationModulesManager = Coddee.Xamarin.Services.ApplicationModulesManager.IApplicationModulesManager;

namespace Coddee.Xamarin.AppBuilder
{
    public class XamarinApplicationBuilder : IXamarinApplicationBuilder
    {
        protected const string _eventsSource = "ApplicationBuilder";

        private readonly IApplication _app;
        private readonly IContainer _container;
        private readonly LogAggregator _logger;
        public BuildActionsCoordinator BuildActionsCoordinator { get; }

        public XamarinApplicationBuilder(IApplication app, IContainer container)
        {
            _app = app;
            _container = container;
            _logger = _container.Resolve<LogAggregator>();
            _container.RegisterInstance<ILogger>(_logger);
            BuildActionsCoordinator = _container.Resolve<BuildActionsCoordinator>();
        }
        

        public virtual void Start()
        {
            _logger.Log(_eventsSource, "Application build started.", LogRecordTypes.Information);
            try
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    UISynchronizationContext.SetContext(SynchronizationContext.Current);
                });
                SetupDefaultBuildActions();

                Log($"Invoking build actions.");
                BuildActionsCoordinator.InvokeAll(_container);
            }
            catch (Exception ex)
            {
                _logger.Log(_eventsSource, ex);
                throw;
            }
        }

        protected void Log(string log, LogRecordTypes type = LogRecordTypes.Debug)
        {
            _logger.Log(_eventsSource, log, type);
        }

        protected void SetupDefaultBuildActions()
        {
            Log($"Setting up default build actions.");

            BuildActionsCoordinator.AddAction(DefaultBuildActions.RegisterDefaultModulesBuildAction(container =>
            {
                var applicationModulesManager = container.RegisterInstance<IApplicationModulesManager, ApplicationModulesManager>();
                applicationModulesManager.RegisterModule(CoreModuleDefinitions.Modules.Concat(XamarinModuleDefinitions.Modules).ToArray());
                applicationModulesManager.InitializeAutoModules().GetAwaiter().GetResult();
            }));


            if (!BuildActionsCoordinator.BuildActionExists(BuildActionsKeys.ConfigureGlobalVariabls))
                ConfigureGlobalVariables();

            if (!BuildActionsCoordinator.BuildActionExists(BuildActionsKeys.ConfigFile))
                this.UseConfigurationFile();
            SetupViewModelBase();

        }



        protected void ConfigureGlobalVariables()
        {
            BuildActionsCoordinator.AddAction(DefaultBuildActions.ConfigureGlobalVariablesBuildAction(container =>
            {
                Log($"Setting up GlobalVariables.");

                _container.Resolve<IGlobalVariablesService>().GetVariable<ApplicationNameGlobalVariable>()
                    .SetValue(_app.ApplicationName);
            }));
        }

        protected void SetupViewModelBase()
        {
            BuildActionsCoordinator.AddAction(DefaultBuildActions.SetupViewModelBaseBuildAction(container =>
            {
                Log($"Setting up ViewModelBase.");
                UniversalViewModelBase.SetApplication((XamarinApplication) _app);
                UniversalViewModelBase.SetContainer(_container);

                ViewModelBase.SetContainer(_container);
                ViewModelEvent.SetViewModelManager(_container.Resolve<IViewModelsManager>());
            }));
        }
    }
}