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


        /// <summary>
        /// Register LinQ repositories and adds a <see cref="LinqRepositoryInitializer"/> to the <see cref="IRepositoryManager"/>
        /// </summary>
        public static IApplicationBuilder UseLinqRepositoryManager<TDBManager>(
            this IApplicationBuilder builder,
            Func<IContainer,string> GetSQLDBConnection,
            string repositoriesAssembly,
            Action ConnectionStringNotFound = null,
            RepositoryConfigurations config = null)
            where TDBManager : ILinqDBManager, new()
        {

            builder.BuildActionsCoordinator.AddAction(DefaultBuildActions.LinqRepositoryBuildAction((container) =>
            {
                var connectionString = GetSQLDBConnection(container);
                if (string.IsNullOrEmpty(connectionString))
                {
                    ConnectionStringNotFound();
                    return;
                }
                CreateRepositoryManager<TDBManager>( container, connectionString, repositoriesAssembly, config);
            }));
            return builder;
        }

        /// <summary>
        /// Register LinQ repositories and adds a <see cref="LinqRepositoryInitializer"/> to the <see cref="IRepositoryManager"/>
        /// </summary>
        public static IApplicationBuilder UseLinqRepositoryManager<TDBManager>(
            this IApplicationBuilder builder,
            string connectionString,
            string repositoriesAssembly,
            RepositoryConfigurations config = null)
            where TDBManager : ILinqDBManager, new()
        {
            builder.BuildActionsCoordinator.AddAction(DefaultBuildActions.LinqRepositoryBuildAction((container) =>
            {
                CreateRepositoryManager<TDBManager>(container, connectionString, repositoriesAssembly, config);
            }));
            return builder;
        }

        private static void CreateRepositoryManager<TDBManager>(IContainer container, string connectionString, string repositoriesAssembly, RepositoryConfigurations config = null)
            where TDBManager : ILinqDBManager, new()
        {

            if (!container.IsRegistered<IRepositoryManager>())
                 container.RegisterInstance<IRepositoryManager,RepositoryManager>();

            var repositoryManager = container.Resolve<IRepositoryManager>();

            var dbManager = new TDBManager();
            container.RegisterInstance<ILinqDBManager>(dbManager);
            dbManager.Initialize(connectionString);

            repositoryManager.AddRepositoryInitializer(new LinqRepositoryInitializer(dbManager, container.Resolve<IObjectMapper>(), config));
            repositoryManager.RegisterRepositories(repositoriesAssembly);

            var logger = container.Resolve<ILogger>();
            container.RegisterInstance<IRepositoryManager>(repositoryManager);
            
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