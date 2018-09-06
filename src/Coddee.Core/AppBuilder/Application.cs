// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using Coddee.AppBuilder;

namespace Coddee
{
    public class Application<TBuilder> : IApplication where TBuilder : IApplicationBuilder
    {
        /// <inheritdoc />
        public Application(Guid applicationID, string applicationName,ApplicationTypes applicationType,IContainer container)
        {
            _container = container;
            ApplicationID = applicationID;
            ApplicationName = applicationName;
            ApplicationType = applicationType;
        }

        public Guid ApplicationID { get; }
        public string ApplicationName { get; }
        public ApplicationTypes ApplicationType { get; }

        /// <summary>
        /// Dependency container
        /// </summary>
        protected IContainer _container;

        /// <summary>
        /// Start the application build process.
        /// </summary>
        public virtual void Run(Action<TBuilder> buildApplication)
        {
            TBuilder builder = RegisterApplication();
            buildApplication(builder);
            builder.Start();
        }

        protected virtual TBuilder RegisterApplication()
        {
            _container.RegisterInstance<IApplication>(this);
            _container.RegisterInstance(this);
            var Builder = ResolveBuilder();
            return Builder;
        }


        protected virtual TBuilder ResolveBuilder()
        {
            return _container.Resolve<TBuilder>();
        }
    }
}
