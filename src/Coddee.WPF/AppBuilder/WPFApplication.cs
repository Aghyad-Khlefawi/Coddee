// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using System.Windows;
using Coddee.AppBuilder;
using Coddee.Services;
using Coddee.WPF.AppBuilder;
using Coddee.WPF.Events;


namespace Coddee.WPF
{

    /// <summary>
    /// The WPF application wrapper
    /// Extend the functionality of the regular WPF Application class
    /// </summary>
    public abstract class WPFApplication : IApplication
    {
        public static WPFApplication Current { get; protected set; }

        public void Run(IContainer container)
        {
            Current = this;
            _systemApplication = Application.Current;
            _container = container;
            _systemApplication.ShutdownMode = ShutdownMode.OnExplicitShutdown;
            Start();
        }

        /// <summary>
        /// Dependency container
        /// </summary>
        protected IContainer _container;

        /// <summary>
        /// The base Application class instance
        /// </summary>
        protected Application _systemApplication;

        public Guid ApplicationID { get; private set; }
        public string ApplicationName { get; private set; }
        public ApplicationTypes ApplicationType { get; private set; }


        public IContainer GetContainer()
        {
            return _container;
        }

        /// <summary>
        /// Start the build phase
        /// </summary>
        private void Start()
        {
            _container.RegisterInstance<IApplication>(this);
            _container.RegisterInstance(this);
            var factory = _container.Resolve<WPFApplicationBuilder>();
            BuildApplication(factory);
        }

        public abstract void BuildApplication(IWPFApplicationFactory app);

        
        /// <summary>
        /// Setter for the application name called by the application identifier
        /// </summary>
        /// <param name="applicationName"></param>
        public void SetApplicationName(string applicationName)
        {
            ApplicationName = applicationName;
        }

        /// <summary>
        /// Setter for the application ID called by the application identifier
        /// </summary>
        /// <param name="applicationID"></param>
        public void SetApplicationID(Guid applicationID)
        {
            ApplicationID = applicationID;
        }

        /// <summary>
        /// Setter for the application type called by the application identifier
        /// </summary>
        /// <param name="applicationType"></param>
        public void SetApplicationType(ApplicationTypes applicationType)
        {
            ApplicationType = applicationType;
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
        /// Shows the application mainwindow
        /// </summary>
        public void ShowWindow()
        {
            _systemApplication.MainWindow.Show();
            _container.Resolve<IGlobalEventsService>().GetEvent<ApplicationStartedEvent>().Invoke(this);
        }

        
    }
}