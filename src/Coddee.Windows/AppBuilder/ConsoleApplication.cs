// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;

namespace Coddee.AppBuilder
{
    public class ConsoleApplication : IApplication
    {
        public ConsoleApplication(Guid applicationID, string applicationName, IContainer container)
        {
            ApplicationID = applicationID;
            ApplicationName = applicationName;
            ApplicationType = ApplicationTypes.Console;
            _container = container;
        }

        public ConsoleApplication(string applicationName, IContainer container)
            : this(Guid.NewGuid(), applicationName, container)
        {

        }
        
        public Guid ApplicationID { get; protected set; }
        public string ApplicationName { get; protected set; }
        public ApplicationTypes ApplicationType { get; protected set; }

        /// <summary>
        /// Dependency container
        /// </summary>
        protected IContainer _container;


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
