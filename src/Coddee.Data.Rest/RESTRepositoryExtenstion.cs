// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using Coddee.Data;
using Coddee.Data.REST;
using Coddee.Loggers;


namespace Coddee.AppBuilder
{
    /// <summary>
    /// Builder extension for Rest repositories.
    /// </summary>
    public static class RESTRepositoryExtenstion
    {
        private const string EventsSource = "ApplicationBuilder";

        /// <summary>
        /// Register REST repositories and adds a <see cref="RESTRepositoryInitializer"/> to the <see cref="IRepositoryManager"/>
        /// </summary>
        public static TBuilder UseRESTRepositories<TBuilder>(
            this TBuilder builder,
            Func<IContainer, RESTInitializerConfig> config)
            where TBuilder : IApplicationBuilder
        {
            builder.BuildActionsCoordinator.AddAction(DefaultBuildActions.RESTRepositoryBuildAction((container) =>
            {
                if (!container.IsRegistered<IRepositoryManager>())
                    throw new ApplicationBuildException("RepositoryManager is not registered. call UseSingletonRepositoryManager or UseTransientRepositoryManager to configuration the repository manager.");

                var repositoryManager = container.Resolve<IRepositoryManager>();

                var configRes = config(container);
                repositoryManager.AddRepositoryInitializer(new RESTRepositoryInitializer(configRes.ApiUrl, configRes.UnauthorizedRequestHandler, container.Resolve<IObjectMapper>(),configRes.RequestTimeout)
                {
                    AddTimestampToRequests = configRes.AddTimestampToRequests
                });

                if (!string.IsNullOrEmpty(configRes.RepositoriesAssembly))
                    repositoryManager.RegisterRepositories(configRes.RepositoriesAssembly);
                if (configRes.RepositoriesTypes != null)
                    repositoryManager.RegisterRepositories(configRes.RepositoriesTypes);

                var logger = container.Resolve<ILogger>();
                container.RegisterInstance<IRepositoryManager>(repositoryManager);
                if (configRes.RegisterTheRepositoresInContainer)
                    foreach (var repository in repositoryManager.GetRepositories())
                    {
                        logger.Log(EventsSource,
                                   $"Registering repository of type {repository.GetType().Name}",
                                   LogRecordTypes.Debug);
                        container.RegisterInstance(repository.ImplementedInterface, repository);
                    }
            }));
            return builder;
        }
    }
}