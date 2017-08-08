// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using System.Diagnostics;
using System.Reflection;
using System.Threading;
using System.Windows;

using Coddee.Loggers;
using Coddee.WPF.AppBuilder;
using Coddee.AppBuilder;
using Coddee.ModuleDefinitions;
using Coddee.Services;
using Coddee.WPF.Modules;

namespace Coddee.WPF
{
    public interface IWPFApplicationBuilder : IApplicationBuilder
    {

    }

    public class WPFApplicationBuilder : IWPFApplicationFactory, IWPFApplicationBuilder
    {
        private const string EventsSource = "WPFApplicationBuilder";

        private readonly WPFApplication _app;
        private Application _systemApplication => _app.GetSystemApplication();
        private readonly IContainer _container;
        private readonly LogAggregator _logger;
        private IApplicationModulesManager _modulesManager;


        public WPFApplicationBuilder(WPFApplication app, IContainer container)
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

            if (!BuildActionsCoordinator.BuildActionExists(BuildActionsKeys.ConfigFile))
                this.UseConfigurationFile(null);
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
                                                              }));
        }

        protected virtual void SetupViewModelBase()
        {
            Log($"Setting up ViewModelBase.");
            ViewModelBase.SetApplication(_app);
            ViewModelBase.SetContainer(_container);
        }

    }
}