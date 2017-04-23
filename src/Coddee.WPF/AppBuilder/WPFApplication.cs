// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using System.Windows;
using Coddee.AppBuilder;
using Coddee.WPF.AppBuilder;
using Microsoft.Practices.Unity;

namespace Coddee.WPF
{
    /// <summary>
    /// The WPF application wrapper
    /// Extend the functionality of the regular WPF Application class
    /// </summary>
    public abstract class WPFApplication : IApplication
    {
        public void Run()
        {
            _systemApplication = Application.Current;
            _container = new UnityContainer();
            _systemApplication.ShutdownMode = ShutdownMode.OnExplicitShutdown;
            Start();
        }

        /// <summary>
        /// Dependency container
        /// </summary>
        protected IUnityContainer _container;

        /// <summary>
        /// The base Application class instance
        /// </summary>
        protected Application _systemApplication;

        public Guid ApplicationID { get; private set; }
        public string ApplicationName { get; private set; }
        public ApplicationTypes ApplicationType { get; private set; }

        /// <summary>
        /// Start the build phase
        /// </summary>
        private void Start()
        {
            BuildApplication(new WPFApplicationBuilder(this, _container));
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
        }
    }
}