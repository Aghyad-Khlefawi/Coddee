﻿// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using Coddee.Data;
using Coddee.Data.MongoDB;
using Coddee.Loggers;
using Microsoft.Practices.Unity;

namespace Coddee.WPF.AppBuilder
{
    public static class MongoRepositoyExtension
    {
        private const string EventsSource = "WPFApplicationBuilder";

        public static IWPFApplicationBuilder UseMongoDBRepository<TRepositoryManager>(
            this IWPFApplicationBuilder builder,
            string connection,
            string databaseName,
            string repositoriesAssembly,
            bool registerTheRepositoresInContainer)
            where TRepositoryManager : IMongoRepositoryManager, new()
        {
            builder.BuildActionsCoordinator.AddAction(DefaultBuildActions.RepositoryBuildAction((container) =>
            {

                IMongoRepositoryManager repositoryManager = new TRepositoryManager();
                repositoryManager.Initialize(new MongoDBManager(connection, databaseName), container.Resolve<IObjectMapper>());
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

        public static IWPFApplicationBuilder UseMongoDBRepository(
            this IWPFApplicationBuilder builder,
            string connection,
            string databaseName,
            string repositoriesAssembly,
            bool registerTheRepositoresInContainer)
        {
            return builder.UseMongoDBRepository<MongoRepositoryManager>(connection,
                                                                        databaseName,
                                                                        repositoriesAssembly,
                                                                        registerTheRepositoresInContainer);
        }
    }
}