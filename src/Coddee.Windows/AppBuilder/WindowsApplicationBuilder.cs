// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
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

        protected void ConfigureGlobalVariables()
        {
            BuildActionsCoordinator.AddAction(DefaultBuildActions.ConfigureGlobalVariablesBuildAction(container =>
            {
                _container.Resolve<IGlobalVariablesService>().GetVariable<ApplicationNameGlobalVariable>().SetValue(_app.ApplicationName);
            }));
        }


        protected void Log(string log, LogRecordTypes type = LogRecordTypes.Debug)
        {
            _logger.Log(_eventsSource, log, type);
        }

        public virtual void Start()
        {
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


            BuildActionsCoordinator.AddAction(DefaultBuildActions.RegisterDefaultModulesBuildAction(container =>
            {
                var applicationModulesManager = container.RegisterInstance<IApplicationModulesManager,ApplicationModulesManager>();
                applicationModulesManager.RegisterModule(GetDefaultModules());
                applicationModulesManager.InitializeAutoModules().GetAwaiter().GetResult();
            }));

            if (!BuildActionsCoordinator.BuildActionExists(BuildActionsKeys.ConfigureGlobalVariabls))
                ConfigureGlobalVariables();
            
            if (!BuildActionsCoordinator.BuildActionExists(BuildActionsKeys.ConfigFile))
                this.UseConfigurationFile(null);
        }

        protected virtual Type[] GetDefaultModules()
        {
            return CoreModuleDefinitions.Modules.Concat(WindowsModuleDefinitions.Modules).ToArray();
        }
    }
}
