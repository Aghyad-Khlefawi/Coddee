// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;

namespace Coddee.AppBuilder
{
    /// <summary>
    /// A windows console application.
    /// </summary>
    public class ConsoleApplication : IApplication
    {
        /// <inheritdoc />
        public ConsoleApplication(Guid applicationID, string applicationName, IContainer container)
        {
            ApplicationID = applicationID;
            ApplicationName = applicationName;
            ApplicationType = ApplicationTypes.Console;
            _container = container;
        }

        /// <inheritdoc />
        public ConsoleApplication(string applicationName, IContainer container)
            : this(Guid.NewGuid(), applicationName, container)
        {

        }
        
        /// <inheritdoc />
        public Guid ApplicationID { get; protected set; }

        /// <inheritdoc />
        public string ApplicationName { get; protected set; }

        /// <inheritdoc />
        public ApplicationTypes ApplicationType { get; protected set; }

        /// <summary>
        /// Dependency container
        /// </summary>
        protected IContainer _container;


        /// <summary>
        /// Start the application build process.
        /// </summary>
        /// <param name="BuildApplication"></param>
        public void Run(Action<IConsoleApplicationBuilder> BuildApplication)
        {
            _container.RegisterInstance<IApplication>(this);
            _container.RegisterInstance(this);
            var factory = _container.Resolve<ConsoleApplicationBuilder>();
            BuildApplication(factory);
            factory.Start();
        }
    }
}
