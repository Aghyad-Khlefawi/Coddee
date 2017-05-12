// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using Coddee.Data;
using Coddee.Data.LinqToSQL;
using Coddee.Data.MongoDB;
using Coddee.Loggers;
using Coddee.Windows.Mapper;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using IApplicationBuilder = Microsoft.AspNetCore.Builder.IApplicationBuilder;

namespace Coddee.AspNet
{
    public static class Extensions
    {
        public static IServiceCollection AddILObjectMapper(this IServiceCollection services)
        {
            var mapper = new ILObjectsMapper();
            services.AddSingleton<IObjectMapper>(mapper);
            return services;
        }

        public static IServiceCollection AddLogger(this IServiceCollection services,
                                                   LoggerTypes loggerType,
                                                   LogRecordTypes level)
        {
            var logger = new LogAggregator();
            logger.Initialize(level);

            if (loggerType.HasFlag(LoggerTypes.DebugOutput))
            {
                var debugLogger = new DebugOuputLogger();
                debugLogger.Initialize(level);
                logger.AddLogger(debugLogger);
            }
            if (loggerType.HasFlag(LoggerTypes.File))
            {
                var fileLogger = new FileLogger();
                fileLogger.Initialize(level, "log.txt");
                logger.AddLogger(fileLogger);
            }
            services.AddSingleton<ILogger>(logger);
            return services;
        }

        public static IServiceCollection AddLinqRepositoryManager<TDBManager, TRepositoryManager>(
            this IServiceCollection services,
            string connectionString,
            string repositoriesAssembly,
            bool registerTheRepositoresInContainer = true)
            where TDBManager : ILinqDBManager, new()
            where TRepositoryManager : ILinqRepositoryManager, new()
        {
            var mapper = services.BuildServiceProvider().GetService<IObjectMapper>();
            var dbManager = new TDBManager();
            dbManager.Initialize(connectionString);
            var repositoryManager = new TRepositoryManager();
            services.AddSingleton<ILinqDBManager>(dbManager);
            services.AddSingleton<IRepositoryManager>(repositoryManager);
            repositoryManager.Initialize(dbManager, mapper);
            repositoryManager.RegisterRepositories(repositoriesAssembly);
            if (registerTheRepositoresInContainer)
                foreach (var repository in repositoryManager.GetRepositories())
                {
                    services.AddSingleton(repository.ImplementedInterface, repository);
                }
            return services;
        }

        public static IServiceCollection AddMongoRepositoryManager<TRepositoryManager>(
            this IServiceCollection services,
            string connectionString,
            string dbName,
            string repositoriesAssembly,
            bool registerTheRepositoresInContainer = true)
            where TRepositoryManager : IMongoRepositoryManager, new()
        {
            var mapper = services.BuildServiceProvider().GetService<IObjectMapper>();
            IMongoRepositoryManager repositoryManager = new TRepositoryManager();
            var dbManager = new MongoDBManager(connectionString, dbName);
            repositoryManager.Initialize(dbManager, mapper);
            repositoryManager.RegisterRepositories(repositoriesAssembly);
            services.AddSingleton<IMongoDBManager>(dbManager);
            services.AddSingleton<IRepositoryManager>(repositoryManager);
            if (registerTheRepositoresInContainer)
                foreach (var repository in repositoryManager.GetRepositories())
                {
                    services.AddSingleton(repository.ImplementedInterface, repository);
                }
            return services;
        }
        public static IServiceCollection AddMongoRepositoryManager(
            this IServiceCollection services,
            string connectionString,
            string dbName,
            string repositoriesAssembly,
            bool registerTheRepositoresInContainer = true)
        {
            return services.AddMongoRepositoryManager<MongoRepositoryManager>(connectionString,
                                                                              dbName,
                                                                              repositoriesAssembly,
                                                                              registerTheRepositoresInContainer);
        }

        public static IApplicationBuilder UseMVCWithCoddeeRoutes(this IApplicationBuilder app)
        {
            return app.UseMvc(routes => { routes.MapRoute("default", "api/{controller}/{action}"); });
        }
    }
}