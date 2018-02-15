// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using Coddee.Data;
using Coddee.Data.LinqToSQL;
using Coddee.Loggers;


namespace Coddee.AppBuilder
{
    /// <summary>
    /// Provides the required configurations to use Linq repositories.
    /// </summary>
    public class LinqInitializerConfig
    {
        /// <inheritdoc />
        public LinqInitializerConfig(Func<IContainer, string> databaseConnection, string repositoriesAssembly)
        {
            DatabaseConnection = databaseConnection;
            RepositoriesAssembly = repositoriesAssembly;
        }

        /// <summary>
        /// Configuration object to be passed to the repositories
        /// </summary>
        public RepositoryConfigurations RepositoryConfigurations { get; set; }

        /// <summary>
        /// An action that will be executed if no connection was provided.
        /// </summary>
        public Action ConnectionStringNotFound { get; set; }

        /// <summary>
        /// A function that returns a valid SQL connection to the database.
        /// </summary>
        public Func<IContainer, string> DatabaseConnection { get; set; }

        /// <summary>
        /// The assembly name containing the repository
        /// <remarks>Without extension</remarks>
        /// </summary>
        public string RepositoriesAssembly { get; set; }
    }

    
    /// <summary>
    /// Application builder extensions
    /// </summary>
    public static class LinqRepositoryExtension
    {
        private const string EventsSource = "ApplicationBuilder";


        /// <summary>
        /// Register LinQ repositories and adds a <see cref="LinqRepositoryInitializer"/> to the <see cref="IRepositoryManager"/>
        /// </summary>
        public static IApplicationBuilder UseLinqRepositories<TDBManager>(
            this IApplicationBuilder builder,
            LinqInitializerConfig config)
            where TDBManager : ILinqDBManager, new()
        {

            builder.BuildActionsCoordinator.AddAction(DefaultBuildActions.LinqRepositoryBuildAction((container) =>
            {
                var connectionString = config.DatabaseConnection(container);
                if (string.IsNullOrEmpty(connectionString))
                {
                    config.ConnectionStringNotFound?.Invoke(); 
                    return;
                }
                CreateRepositoryManager<TDBManager>(container, connectionString, config.RepositoriesAssembly, config.RepositoryConfigurations);
            }));
            return builder;
        }

        private static void CreateRepositoryManager<TDBManager>(IContainer container, string connectionString, string repositoriesAssembly, RepositoryConfigurations config = null)
            where TDBManager : ILinqDBManager, new()
        {

            if (!container.IsRegistered<IRepositoryManager>())
                throw new ApplicationBuildException("RepositoryManager is not registered. call UseSingletonRepositoryManager or UseTransientRepositoryManager to configuration the repository manager.");

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