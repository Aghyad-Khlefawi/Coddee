﻿ // Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using Coddee.Data;
using Coddee.Data.MongoDB;
using Coddee.Loggers;


namespace Coddee.AppBuilder
{
    public static class MongoRepositoyExtension
    {
        private const string EventsSource = "ApplicationBuilder";

        public static IApplicationBuilder UseMongoDBRepositories(
            this IApplicationBuilder builder,
            string connection,
            string databaseName,
            string repositoriesAssembly,
            bool registerTheRepositoresInContainer)
        {
            builder.BuildActionsCoordinator.AddAction(DefaultBuildActions.MongoRepositoryBuildAction((container) =>
            {

                if (!container.IsRegistered<IRepositoryManager>())
                    throw new ApplicationBuildException("RepositoryManager is not registered. call UseSingletonRepositoryManager or UseTransientRepositoryManager to configuration the repository manager.");

                var repositoryManager = container.Resolve<IRepositoryManager>();

                repositoryManager.AddRepositoryInitializer(new  MongoRepositoryInitializer(new MongoDBManager(connection, databaseName), container.Resolve<IObjectMapper>()));
                repositoryManager.RegisterRepositories(repositoriesAssembly);

                var logger = container.Resolve<ILogger>();
                container.RegisterInstance<IRepositoryManager>(repositoryManager);
                if (registerTheRepositoresInContainer)
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