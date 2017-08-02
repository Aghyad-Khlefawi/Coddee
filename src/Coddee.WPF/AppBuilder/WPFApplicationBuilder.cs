// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using System.Diagnostics;
using System.Reflection;
using System.Threading;
using System.Windows;
using Microsoft.Practices.Unity;
using Coddee.Loggers;
using Coddee.WPF.AppBuilder;
using Coddee.AppBuilder;
using Coddee.Data;
using Coddee.ModuleDefinitions;
using Coddee.Services;
using Coddee.SQL;
using Coddee.WPF.Modules;

namespace Coddee.WPF
{
    public interface IWPFApplicationBuilder : IApplicationBuilder
    {
        WPFApplicationBuilder WPFBuilder { get; } 
    }

    public class WPFApplicationBuilder : IWPFApplicationFactory, IWPFApplicationBuilder
    {
        private const string EventsSource = "WPFApplicationBuilder";

        private readonly WPFApplication _app;
        private Application _systemApplication => _app.GetSystemApplication();
        private readonly IUnityContainer _container;
        private readonly LogAggregator _logger;
        private IApplicationModulesManager _modulesManager;
        

        public WPFApplicationBuilder(WPFApplication app, IUnityContainer container)
        {
            _app = app;
            _container = container;
            _systemApplication.Startup += OnStartup;
            _logger = _container.Resolve<LogAggregator>();
            _container.RegisterInstance<ILogger>(_logger);

            BuildActionsCoordinator = _container.Resolve<BuildActionsCoordinator>();
        }


        //Build Actions
        
        public BuildActionsCoordinator BuildActionsCoordinator { get; }


        /// <summary>
        /// Returns the application builder
        /// </summary>
        /// <param name="applicationName">The application name</param>
        /// <param name="applicationID">A GUID to globally identify the application</param>
        /// <returns></returns>
        public IWPFApplicationBuilder CreateWPFApplication(string applicationName, Guid applicationID)
        {
            _app.SetApplicationName(applicationName);
            _app.SetApplicationID(applicationID);
            _app.SetApplicationType(ApplicationTypes.WPF);

            LogCoddeeVersions();

            return this;
        }

        private void LogCoddeeVersions()
        {
            _logger.Log(EventsSource,
                                    $"using Coddee.Core: {FileVersionInfo.GetVersionInfo(Assembly.Load("Coddee.Core").Location).ProductVersion}");
            _logger.Log(EventsSource,
                        $"using Coddee.Data: {FileVersionInfo.GetVersionInfo(Assembly.Load("Coddee.Data").Location).ProductVersion}");
            _logger.Log(EventsSource,
                        $"using Coddee.WPF: {FileVersionInfo.GetVersionInfo(Assembly.Load("Coddee.WPF").Location).ProductVersion}");
        }

        protected virtual void OnStartup(object sender, StartupEventArgs e)
        {
            _logger.Log(EventsSource, "Application build started.", LogRecordTypes.Information);

            _systemApplication.Dispatcher.Invoke(() =>
            {
                UISynchronizationContext.SetContext(SynchronizationContext.Current);
            });

            try
            {
                SetupDefaultBuildActions();

                Log($"Invoking build actions.");
                BuildActionsCoordinator.InvokeAll(_container);
            }
            catch (Exception ex)
            {
                _logger.Log(EventsSource, ex);
                throw;
            }
        }

        private void Log(string log, LogRecordTypes type = LogRecordTypes.Debug)
        {
            _logger.Log(EventsSource, log, type);
        }

        private void SetupDefaultBuildActions()
        {
            Log($"Setting up default build actions.");

            BuildActionsCoordinator.AddAction(DefaultBuildActions.SetupViewModelBaseBuildAction(container =>
            {
                SetupViewModelBase();
            }));

            if (!BuildActionsCoordinator.BuildActionExists(BuildActionsKeys.ConfigureGlobalVariabls))
                ConfigureGlobalVariables();

            if (!BuildActionsCoordinator.BuildActionExists(BuildActionsKeys.DiscoverModules))
                ConfigureAutoModuleDiscovery();

        }

        private void ConfigureGlobalVariables()
        {
            BuildActionsCoordinator.AddAction(DefaultBuildActions.ConfigureGlobalVariablesBuildAction(container =>
            {
                _container.Resolve<IGlobalVariablesService>().SetValue(Globals.ApplicationName, _app.ApplicationName);
            }));
        }

        private void ConfigureAutoModuleDiscovery()
        {
            BuildActionsCoordinator.AddAction(DefaultBuildActions.DiscoverModulesBuildAction(
                                                              container =>
                                                              {
                                                                  _modulesManager = _container.Resolve<ApplicationModulesManager>();
                                                                  _modulesManager.RegisterModule(CoreModuleDefinitions.Modules);
                                                                  _modulesManager.RegisterModule(WindowsModuleDefinitions.Modules);
                                                                  _modulesManager.RegisterModule(WPFModuleDefinitions.Modules);

                                                                  _modulesManager.InitializeAutoModules();
                                                                  _app.OnAutoModulesInitialized();
                                                              }));
        }

        protected virtual void SetupViewModelBase()
        {
            Log($"Setting up ViewModelBase.");
            ViewModelBase.SetApplication(_app);
            ViewModelBase.SetContainer(_container);
        }

        

        /// <summary>
        /// Register the repository manager in the container
        /// </summary>
        public void SetRepositoryManager(IRepositoryManager repositoryManager, bool registerTheRepositoresInContainer)
        {
            _logger.Log(EventsSource,
                        $"Registering repository manager of type {repositoryManager.GetType().Name}",
                        LogRecordTypes.Debug);
            _container.RegisterInstance<IRepositoryManager>(repositoryManager);
            _app.OnRepositoryManagerSet();
            if (registerTheRepositoresInContainer)
                foreach (var repository in repositoryManager.GetRepositories())
                {
                    _logger.Log(EventsSource,
                                $"Registering repository of type {repository.GetType().Name}",
                                LogRecordTypes.Debug);
                    _container.RegisterInstance(repository.ImplementedInterface, repository);
                }
        }

        /// <summary>
        /// Uses SQLDBBrowser to let the use select an SQL Server database
        /// </summary>
        /// <returns></returns>
        public string GetSQLDBConnection()
        {
            if (!_container.IsRegistered<IConfigurationManager>())
            {
                //TODO using configurations.
                //this.UseConfigurationFile(true);
                //InvokeBuildAction(BuildActions.ConfigFile);
            }
            var config = _container.Resolve<IConfigurationManager>();
            string connection = null;
            if (config.TryGetValue(BuiltInConfigurationKeys.SQLDBConnection, out connection) &&
                !string.IsNullOrEmpty(connection))
            {
                return connection;
            }
            var browser = _container.Resolve<ISQLDBBrowser>();
            connection = browser.GetDatabaseConnectionString();
            config.SetValue(BuiltInConfigurationKeys.SQLDBConnection, connection);
            return connection;
        }

        /// <summary>
        /// Returns the application builder as a WPFApplicationBuilder
        /// </summary>
        /// <value></value>
        public WPFApplicationBuilder WPFBuilder => this;


        /// <summary>
        /// Returns the WPFApplication instance
        /// </summary>
        /// <returns></returns>
        public WPFApplication GetApp()
        {
            return _app;
        }
    }
}