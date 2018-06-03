// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using Coddee.AppBuilder;
using Coddee.Data;
using Coddee.Data.LinqToSQL;
using Coddee.Data.MongoDB;
using Coddee.Loggers;
using Coddee.Windows.AppBuilder;
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
                                                   LoggerOptions options)
        {
            var logger = new LogAggregator();
            logger.Initialize(options.Level);
            logger.AllowedTypes = options.LoggerType;

            if (options.LoggerType.HasFlag(LoggerTypes.DebugOutput))
            {
                var debugLogger = new DebugOuputLogger();
                debugLogger.Initialize(options.Level);
                logger.AddLogger(debugLogger, LoggerTypes.DebugOutput);
            }

            if (options.LoggerType.HasFlag(LoggerTypes.File))
            {
                var fileLogger = new FileLogger();
                fileLogger.Initialize(options.Level, options.LogFilePath, options.UseFileCompression);
                logger.AddLogger(fileLogger, LoggerTypes.File);
            }
            services.AddSingleton<ILogger>(logger);
            return services;
        }

        /// <summary>
        /// Register an <see cref="IContainer"/> object
        /// </summary>
        public static IContainer AddContainer(
            this IServiceCollection services)
        {
            return new AspCoreContainer(services);
        }
        /// <summary>
        /// Configure the application to use a <see cref="SingletonRepositoryManager"/> that returns the same repository instance on each call.
        /// </summary>
        public static void AddSingletonRepositoryManager(this IServiceCollection services)
        {
            services.AddSingleton<IRepositoryManager, SingletonRepositoryManager>();
        }

        /// <summary>
        /// Configure the application to use a <see cref="TransientRepositoryManager"/> that returns the a new repository instance on each call.
        /// </summary>
        public static void AddTransientRepositoryManager(this IServiceCollection services)
        {
            services.AddSingleton<IRepositoryManager>(new TransientRepositoryManager());
        }

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

        public static IServiceCollection AddMongoRepositoryManager(
            this IServiceCollection services,
            string connectionString,
            string dbName,
            string repositoriesAssembly)
        {
            if (services.All(e => e.ServiceType != typeof(IRepositoryManager)))
                services.AddSingleton<IRepositoryManager>(new SingletonRepositoryManager());

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
        public static IApplicationBuilder UseCoddeeDynamicApi(this IApplicationBuilder appBuilder, Func<IIdentity, object> setContext)
        {
            appBuilder.UseMiddleware<CoddeeDynamicApi>(setContext);
            return appBuilder;
        }
        public static IApplicationBuilder UseCoddeeDynamicApi2(this IApplicationBuilder appBuilder)
        {
            appBuilder.UseMiddleware<CoddeeDynamicApi2>();
            return appBuilder;
        }
        public static IApplicationBuilder UseCoddeeDynamicApi(this IApplicationBuilder appBuilder)
        {
            appBuilder.UseMiddleware<CoddeeDynamicApi>(null);
            return appBuilder;
        }
        public static IServiceCollection AddDynamicApi(this IServiceCollection services)
        {
            return AddDynamicApiInternal(services, null);
        }

        public static IServiceCollection AddDynamicApi(this IServiceCollection services, IEnumerable<Type> controllers)
        {
            return AddDynamicApiInternal(services, controllers);
        }
        private static IServiceCollection AddDynamicApiInternal(IServiceCollection services,IEnumerable<Type> controllers)
        {
            var serviceProvider = services.BuildServiceProvider();
            var container = serviceProvider.GetService<IContainer>();
            var manager = new DynamicApiControllersManager();
            controllers.ForEach(manager.RegisterController);
            container.RegisterInstance(manager);

            var api = new CoddeeDynamicApi2(container);
            api.CacheRegisteredControllers();
            container.RegisterInstance(api);
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