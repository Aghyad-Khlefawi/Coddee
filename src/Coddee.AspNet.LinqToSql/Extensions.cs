using System;
using System.Linq;
using Coddee;
using Coddee.AppBuilder;
using Coddee.Data;
using Coddee.Data.LinqToSQL;
using Microsoft.Extensions.DependencyInjection;

namespace Coddee.AspNet.LinqToSql
{
    public static class Extensions
    {
        /// <summary>
        /// Configure and register linq repositories.
        /// </summary>
        public static IRepositoryManager AddLinqRepositories<TDBManager>(
            this IServiceCollection services,
            LinqInitializerConfig config,
            bool registerAsServices = false)
            where TDBManager : ILinqDBManager, new()
        {
            if (services.All(e => e.ServiceType != typeof(IRepositoryManager)))
                throw new ApplicationBuildException("RepositoryManager is not registered. call AddSingletonRepositoryManager or AddTransientRepositoryManager to configuration the repository manager.");


            var serviceProvider = services.BuildServiceProvider();
            var mapper = serviceProvider.GetService<IObjectMapper>();
            var dbManager = new TDBManager();
            dbManager.Initialize(config.DatabaseConnection(serviceProvider.GetService<IContainer>()));

            services.AddSingleton<ILinqDBManager>(dbManager);
            var repositoryManager = serviceProvider.GetService<IRepositoryManager>();

            repositoryManager.AddRepositoryInitializer(new LinqRepositoryInitializer(dbManager, mapper, config.RepositoryConfigurations));

            repositoryManager.RegisterRepositories(config.RepositoriesAssembly);
            if (registerAsServices)
                foreach (var repository in repositoryManager.GetRepositories())
                {
                    services.AddSingleton(repository.ImplementedInterface, repository);
                }
            return repositoryManager;
        }

    }
}
