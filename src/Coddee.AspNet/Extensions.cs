// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Coddee.Attributes;
using Coddee.Data;
using Coddee.Data.LinqToSQL;
using Coddee.Data.MongoDB;
using Coddee.Loggers;
using Coddee.Windows.Mapper;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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
                logger.AddLogger(debugLogger, LoggerTypes.DebugOutput);
            }
            if (loggerType.HasFlag(LoggerTypes.File))
            {
                var fileLogger = new FileLogger();
                fileLogger.Initialize(level, "log.txt");
                logger.AddLogger(fileLogger, LoggerTypes.File);
            }
            services.AddSingleton<ILogger>(logger);
            return services;
        }

        public static IRepositoryManager AddLinqRepositoryManager<TDBManager>(
            this IServiceCollection services,
            string connectionString,
            string repositoriesAssembly,
            RepositoryConfigurations config = null)
            where TDBManager : ILinqDBManager, new()
        {
            var repositoryManager = new RepositoryManager();
            if (services.All(e => e.ServiceType != typeof(IRepositoryManager)))
                services.AddSingleton<IRepositoryManager>(repositoryManager);

            var serviceProvider = services.BuildServiceProvider();
            var mapper = serviceProvider.GetService<IObjectMapper>();
            var dbManager = new TDBManager();
            dbManager.Initialize(connectionString);
            
            services.AddSingleton<ILinqDBManager>(dbManager);
            services.AddSingleton<IRepositoryManager>(repositoryManager);

            repositoryManager.AddRepositoryInitializer(new LinqRepositoryInitializer(dbManager, mapper, config));

            repositoryManager.RegisterRepositories(repositoriesAssembly);
            foreach (var repository in repositoryManager.GetRepositories())
            {
                services.AddSingleton(repository.ImplementedInterface, repository);
            }
            return repositoryManager;
        }

        public static IServiceCollection AddMongoRepositoryManager(
            this IServiceCollection services,
            string connectionString,
            string dbName,
            string repositoriesAssembly)
        {
            if (services.All(e => e.ServiceType != typeof(IRepositoryManager)))
                services.AddSingleton<IRepositoryManager>(new RepositoryManager());

            var serviceProvider = services.BuildServiceProvider();
            var repositoryManager = serviceProvider.GetService<IRepositoryManager>();
            var mapper = serviceProvider.GetService<IObjectMapper>();

            var dbManager = new MongoDBManager(connectionString, dbName);
            repositoryManager.AddRepositoryInitializer(new MongoRepositoryInitializer(dbManager, mapper));
            repositoryManager.RegisterRepositories(repositoriesAssembly);
            services.AddSingleton<IMongoDBManager>(dbManager);
            services.AddSingleton<IRepositoryManager>(repositoryManager);
            foreach (var repository in repositoryManager.GetRepositories())
            {
                services.AddSingleton(repository.ImplementedInterface, repository);
            }
            return services;
        }
        public static IApplicationBuilder UseMVCWithCoddeeRoutes(this IApplicationBuilder app, string apiPrefix)
        {
            return app.UseMvc(routes => { routes.MapRoute("default", $"{apiPrefix}/{{controller}}/{{action}}"); });
        }
        public static IApplicationBuilder UseMVCWithCoddeeRoutes(this IApplicationBuilder app)
        {
            return app.UseMVCWithCoddeeRoutes("api");
        }
        public static IApplicationBuilder UseCoddeeDynamicApi(this IApplicationBuilder appBuilder)
        {
            appBuilder.UseMiddleware<CoddeeDynamicApi>();
            return appBuilder;
        }
        public static IServiceCollection AddDynamicApi(this IServiceCollection services)
        {
            var manager = new CoddeeControllersManager(services);
            services.AddSingleton(manager);
            return services;
        }
        public static IServiceCollection AddDynamicApi(this IServiceCollection services, Action<CoddeeControllersManager> config)
        {
            var manager = new CoddeeControllersManager(services);
            config(manager);
            services.AddSingleton(manager);
            return services;
        }
    }
}