// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using Coddee.Data;
using Coddee.Loggers;

namespace Coddee.AppBuilder
{
    /// <summary>
    /// Extensions for the application builder
    /// </summary>
    public static class BuilderExtensions
    {
        private const string EventsSource = "ApplicationBuilder";

        /// <summary>
        /// Configure the application to use a <see cref="SingletonRepositoryManager"/> that returns the same repository instance on each call.
        /// </summary>
        public static T UseSingletonRepositoryManager<T>(this T builder) where T : IApplicationBuilder
        {
            builder.BuildActionsCoordinator.AddAction(DefaultBuildActions.InMemoryRepositoryBuildAction((container) =>
            {
                container.RegisterInstance<IRepositoryManager, SingletonRepositoryManager>();
            }));
            return builder;
        }

        /// <summary>
        /// Configure the application to use a <see cref="TransientRepositoryManager"/> that returns the a new repository instance on each call.
        /// </summary>
        public static T UseTransientRepositoryManager<T>(this T builder) where T : IApplicationBuilder
        {
            builder.BuildActionsCoordinator.AddAction(DefaultBuildActions.InMemoryRepositoryBuildAction((container) =>
            {
                container.RegisterInstance<IRepositoryManager, TransientRepositoryManager>();
            }));
            return builder;
        }


        /// <summary>
        /// Configure the repository manager to use InMemory repositories
        /// </summary>
        /// <param name="builder">Application builder</param>
        /// <param name="repositoriesAssembly">The name of the assembly containing the repositories
        /// <remarks>The assembly name should not contain the extension.</remarks></param>
        /// <param name="config">The configuration object to be passed to the repositories</param>
        /// <returns>Application builder</returns>
        public static T UseInMemoryRepositories<T>(this T builder, string repositoriesAssembly,
                                                              RepositoryConfigurations config = null) where T : IApplicationBuilder
        {
            builder.BuildActionsCoordinator.AddAction(DefaultBuildActions.InMemoryRepositoryBuildAction((container) =>
            {
                if (!container.IsRegistered<IRepositoryManager>())
                    throw new ApplicationBuildException("RepositoryManager is not registered. call UseSingletonRepositoryManager or UseTransientRepositoryManager to configuration the repository manager.");

                var repositoryManager = container.Resolve<IRepositoryManager>();

                repositoryManager.AddRepositoryInitializer(new InMemoryRepositoryInitializer(container.Resolve<IObjectMapper>(), config));
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

            }));
            return builder;
        }
    }
}
