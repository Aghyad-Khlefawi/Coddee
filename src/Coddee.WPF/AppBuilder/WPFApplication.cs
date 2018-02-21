// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows;
using Coddee.AppBuilder;
using Coddee.Loggers;
using Coddee.Services.ApplicationConsole;

namespace Coddee.WPF
{

    /// <summary>
    /// The WPF application wrapper
    /// Extend the functionality of the regular WPF Application class
    /// </summary>
    public class WPFApplication : IApplication
    {
        private const string _eventsSource = "Application";

        /// <inheritdoc />
        public WPFApplication(Guid applicationID, string applicationName, IContainer container)
        {
            ApplicationID = applicationID;
            ApplicationName = applicationName;
            ApplicationType = ApplicationTypes.WPF;
            _container = container;
        }

        /// <inheritdoc />
        public WPFApplication(string applicationName, IContainer container)
            : this(Guid.NewGuid(), applicationName, container)
        {
        }


        /// <summary>
        /// Log the initial application information.
        /// </summary>
        protected virtual void LogStart()
        {
            _logger.Log(_eventsSource, $"Initializing application {ApplicationName} {Assembly.GetEntryAssembly().GetName().Version}");
            _logger.Log(_eventsSource, $"using Coddee.Core: {Assembly.Load("Coddee.Core").GetName().Version}");
            _logger.Log(_eventsSource, $"using Coddee.Windows: {Assembly.Load("Coddee.Windows").GetName().Version}");
            _logger.Log(_eventsSource, $"using Coddee.WPF: {Assembly.Load("Coddee.WPF").GetName().Version}");
        }

        /// <summary>
        /// The running WPF application.
        /// </summary>
        public static WPFApplication Current { get; protected set; }

        ILogger _logger;

        /// <summary>
        /// Start the application build process.
        /// </summary>
        public void Run(Action<IWPFApplicationBuilder> BuildApplication, StartupEventArgs startupEventArgs)
        {


            Current = this;
            _systemApplication = Application.Current;
            _systemApplication.ShutdownMode = ShutdownMode.OnExplicitShutdown;
            _container.RegisterInstance<IApplication>(this);
            _container.RegisterInstance(this);
            var builder = _container.Resolve<WPFApplicationBuilder>();
            _logger = _container.Resolve<ILogger>();
            LogStart();
            ResolveStartupArgs(startupEventArgs);
            BuildApplication(builder);
        }
        
        /// <summary>
        /// The startup command passed to the executable.
        /// </summary>
        public IEnumerable<string> StartupCommandStrings { get; private set; }

        /// <summary>
        /// The parsed result of the startup command passed to the executable.
        /// </summary>
        public IEnumerable<CommandParseResult> StartupCommands { get; private set; }

        private void ResolveStartupArgs(StartupEventArgs startupEventArgs)
        {
            var command = string.Concat(startupEventArgs.Args.Select(e => e + " ")).TrimEnd();
            var commandParser = new ConsoleCommandParser();
            commandParser.RegisterCommands(DefaultCommands.AllCommands);
            StartupCommandStrings = command.Split('|');
            var parsedCommands = new List<CommandParseResult>();
            foreach (var cmd in StartupCommandStrings)
            {
                parsedCommands.Add(commandParser.ParseCommand(cmd));
            }
            StartupCommands = parsedCommands;
        }

        /// <summary>
        /// Dependency container
        /// </summary>
        protected IContainer _container;

        /// <summary>
        /// The base Application class instance
        /// </summary>
        protected Application _systemApplication;

        /// <inheritdoc />
        public Guid ApplicationID { get; }
        /// <inheritdoc />
        public string ApplicationName { get; }
        /// <inheritdoc />
        public ApplicationTypes ApplicationType { get; }


        /// <summary>
        /// Returns the dependency container used in the application.
        /// </summary>
        /// <returns></returns>
        public IContainer GetContainer()
        {
            return _container;
        }

        /// <summary>
        /// Returns the system application
        /// </summary>
        /// <returns></returns>
        public Application GetSystemApplication()
        {
            return _systemApplication;
        }

        /// <summary>
        /// Shows the application MainWindow
        /// </summary>
        public void ShowWindow()
        {
            _systemApplication.MainWindow.Show();
        }
    }
}