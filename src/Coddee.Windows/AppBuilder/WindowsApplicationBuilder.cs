﻿// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using System.Linq;
using Coddee.Loggers;
using Coddee.ModuleDefinitions;
using Coddee.Services;

namespace Coddee.AppBuilder
{
    /// <summary>
    /// Base class for windows applications builders.
    /// </summary>
    public abstract class WindowsApplicationBuilder : IApplicationBuilder
    {
        /// <summary>
        /// The source for the logging service.
        /// </summary>
        protected const string _eventsSource = "ApplicationBuilder";

        /// <summary>
        /// The application.
        /// </summary>
        protected readonly IApplication _app;

        /// <summary>
        /// Dependency container.
        /// </summary>
        protected readonly IContainer _container;

        /// <summary>
        /// Logging service.
        /// </summary>
        protected readonly LogAggregator _logger;

        /// <inheritdoc />
        protected WindowsApplicationBuilder(IApplication app, IContainer container)
        {
            _app = app;
            _container = container;
            _logger = _container.Resolve<LogAggregator>();
            _container.RegisterInstance<ILogger>(_logger);
            BuildActionsCoordinator = _container.Resolve<BuildActionsCoordinator>();
        }

        /// <inheritdoc />
        public BuildActionsCoordinator BuildActionsCoordinator { get; }

        /// <summary>
        /// Set the default global variables values.
        /// </summary>
        protected void ConfigureGlobalVariables()
        {
            BuildActionsCoordinator.AddAction(DefaultBuildActions.ConfigureGlobalVariablesBuildAction(container =>
            {
                _container.Resolve<IGlobalVariablesService>().GetVariable<ApplicationNameGlobalVariable>().SetValue(_app.ApplicationName);
            }));
        }


        /// <summary>
        /// send a record to the logging service.
        /// </summary>
        protected void Log(string log, LogRecordTypes type = LogRecordTypes.Debug)
        {
            _logger.Log(_eventsSource, log, type);
        }

        /// <summary>
        /// Start the application build process.
        /// </summary>
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

        /// <summary>
        /// set the default application build steps.
        /// </summary>
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

        /// <summary>
        /// Get the default modules supported by a windows application.
        /// </summary>
        protected virtual Type[] GetDefaultModules()
        {
            return CoreModuleDefinitions.Modules.Concat(WindowsModuleDefinitions.Modules).ToArray();
        }
    }
}
