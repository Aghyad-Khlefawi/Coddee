// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using Coddee.Data;
using Coddee.Data.LinqToSQL;
using Coddee.Loggers;


namespace Coddee.AppBuilder
{
    public static class LinqRepositoryExtension
    {
        private const string EventsSource = "ApplicationBuilder";

       

        public static IApplicationBuilder UseLinqRepositoryManager<TDBManager, TRepositoryManager>(
            this IApplicationBuilder builder,
            Func<IContainer,string> GetSQLDBConnection,
            string repositoriesAssembly,
            bool registerTheRepositoresInContainer,
            Action ConnectionStringNotFound = null,
            RepositoryConfigurations config = null)
            where TDBManager : ILinqDBManager, new()
            where TRepositoryManager : ILinqRepositoryManager, new()
        {

            builder.BuildActionsCoordinator.AddAction(DefaultBuildActions.RepositoryBuildAction((container) =>
            {
                var connectionString = GetSQLDBConnection(container);
                if (string.IsNullOrEmpty(connectionString))
                {
                    ConnectionStringNotFound();
                    return;
                }
                CreateRepositoryManager<TDBManager, TRepositoryManager>(builder, container, connectionString, repositoriesAssembly, registerTheRepositoresInContainer, config);
            }));
            return builder;
        }

        public static IApplicationBuilder UseLinqRepositoryManager<TDBManager, TRepositoryManager>(
            this IApplicationBuilder builder,
            string connectionString,
            string repositoriesAssembly,
            bool registerTheRepositoresInContainer,
            RepositoryConfigurations config = null)
            where TDBManager : ILinqDBManager, new()
            where TRepositoryManager : ILinqRepositoryManager, new()
        {
            builder.BuildActionsCoordinator.AddAction(DefaultBuildActions.RepositoryBuildAction((container) =>
            {
                CreateRepositoryManager<TDBManager, TRepositoryManager>(builder, container, connectionString, repositoriesAssembly, registerTheRepositoresInContainer, config);
            }));
            return builder;
        }

        private static void CreateRepositoryManager<TDBManager, TRepositoryManager>(IApplicationBuilder builder, IContainer container, string connectionString, string repositoriesAssembly, bool registerTheRepositoresInContainer, RepositoryConfigurations config = null)
            where TDBManager : ILinqDBManager, new()
            where TRepositoryManager : ILinqRepositoryManager, new()
        {
            var dbManager = new TDBManager();
            container.RegisterInstance<ILinqDBManager>(dbManager);
            dbManager.Initialize(connectionString);
            var repositoryManager = new TRepositoryManager();
            repositoryManager.Initialize(dbManager, container.Resolve<IObjectMapper>(), config);
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
        }
        
    }
}