using System;
using System.Diagnostics;
using System.Reflection;
using Coddee.AppBuilder;
using Coddee.Loggers;
using Coddee.ModuleDefinitions;
using Coddee.Services;

namespace Coddee.AppBuilder
{
    public abstract class WindowsApplicationBuilder : IApplicationBuilder
    {
        protected const string _eventsSource = "ApplicationBuilder";

        protected readonly IApplication _app;
        protected readonly IContainer _container;
        protected readonly LogAggregator _logger;
        protected IApplicationModulesManager _modulesManager;

        protected WindowsApplicationBuilder(IApplication app, IContainer container)
        {
            _app = app;
            _container = container;
            _logger = _container.Resolve<LogAggregator>();
            _container.RegisterInstance<ILogger>(_logger);
            BuildActionsCoordinator = _container.Resolve<BuildActionsCoordinator>();
        }

        //Build Actions
        public BuildActionsCoordinator BuildActionsCoordinator { get; }

        protected virtual void LogCoddeeVersions()
        {
            _logger.Log(_eventsSource,
                        $"using Coddee.Core: {FileVersionInfo.GetVersionInfo(Assembly.Load("Coddee.Core").Location).ProductVersion}");
            _logger.Log(_eventsSource,
                      $"using Coddee.Windows: {FileVersionInfo.GetVersionInfo(Assembly.Load("Coddee.Windows").Location).ProductVersion}");
        }

        protected void ConfigureGlobalVariables()
        {
            BuildActionsCoordinator.AddAction(DefaultBuildActions.ConfigureGlobalVariablesBuildAction(container =>
            {
                _container.Resolve<IGlobalVariablesService>().SetValue(Globals.ApplicationName, _app.ApplicationName);
            }));
        }


        protected virtual void ConfigureAutoModuleDiscovery()
        {
            BuildActionsCoordinator.AddAction(DefaultBuildActions.DiscoverModulesBuildAction(
                                                                                             container =>
                                                                                             {
                                                                                                 _modulesManager = _container.Resolve<ApplicationModulesManager>();
                                                                                                 _modulesManager.RegisterModule(CoreModuleDefinitions.Modules);
                                                                                                 _modulesManager.RegisterModule(WindowsModuleDefinitions.Modules);
                                                                                                 _modulesManager.InitializeAutoModules();
                                                                                             }));
        }
        protected void Log(string log, LogRecordTypes type = LogRecordTypes.Debug)
        {
            _logger.Log(_eventsSource, log, type);
        }

        public virtual void Start()
        {
            LogCoddeeVersions();
            _logger.Log(_eventsSource, "Application build started.", LogRecordTypes.Information);

            try
            {
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

        protected virtual void SetupDefaultBuildActions()
        {
            Log($"Setting up default build actions.");

            if (!BuildActionsCoordinator.BuildActionExists(BuildActionsKeys.ConfigureGlobalVariabls))
                ConfigureGlobalVariables();

            if (!BuildActionsCoordinator.BuildActionExists(BuildActionsKeys.DiscoverModules))
                ConfigureAutoModuleDiscovery();

            if (!BuildActionsCoordinator.BuildActionExists(BuildActionsKeys.ConfigFile))
                this.UseConfigurationFile(null);
        }
    }
}
