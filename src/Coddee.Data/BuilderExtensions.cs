// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using Coddee.Data;
using Coddee.Loggers;

namespace Coddee.AppBuilder
{
    public static class BuilderExtensions
    {
        private const string EventsSource = "ApplicationBuilder";

        public static T UseInMemoryRepositories<T>(this T builder, string repositoriesAssembly,
                                                                  RepositoryConfigurations config = null) where T :IApplicationBuilder
        {
            builder.BuildActionsCoordinator.AddAction(DefaultBuildActions.InMemoryRepositoryBuildAction((container) =>
            {
                if (!container.IsRegistered<IRepositoryManager>())
                    container.RegisterInstance<IRepositoryManager, RepositoryManager>();

                var repositoryManager = container.Resolve<IRepositoryManager>();

                repositoryManager
                    .AddRepositoryInitializer(new InMemoryRepositoryInitializer(container.Resolve<IObjectMapper>(),
                                                                                config),(int)RepositoryTypes.InMemory);
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
