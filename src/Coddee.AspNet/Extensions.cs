// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using System.Collections.Generic;
using System.Linq;
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
    /// <summary>
    /// Extensions methods for ASP Core.
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Register an instance of <see cref="ILObjectsMapper"/>.
        /// </summary>
        public static IServiceCollection AddILObjectMapper(this IServiceCollection services)
        {
            var mapper = new ILObjectsMapper();
            services.AddSingleton<IObjectMapper>(mapper);
            return services;
        }

        /// <summary>
        /// Configure the application logging service.
        /// </summary>
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

        /// <summary>
        /// Configure and register Mongo DB repositories.
        /// </summary>
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

        /// <summary>
        /// Use the default routing used by the library.
        /// </summary>
        public static IApplicationBuilder UseMVCWithCoddeeRoutes(this IApplicationBuilder app, string apiPrefix)
        {
            return app.UseMvc(routes => { routes.MapRoute("default", $"{apiPrefix}/{{controller}}/{{action}}"); });
        }

        /// <inheritdoc cref="UseMVCWithCoddeeRoutes(Microsoft.AspNetCore.Builder.IApplicationBuilder,string)"/>
        public static IApplicationBuilder UseMVCWithCoddeeRoutes(this IApplicationBuilder app)
        {
            return app.UseMVCWithCoddeeRoutes("api");
        }

        /// <summary>
        /// Register Coddee dynamic API.
        /// </summary>
        public static IServiceCollection AddDynamicApi(this IServiceCollection services)
        {
            return AddDynamicApi(services, DynamicApiConfigurations.Default, null);
        }

        /// <summary>
        /// Register Coddee dynamic API.
        /// </summary>
        public static IServiceCollection AddDynamicApi(this IServiceCollection services, IEnumerable<Type> controllers)
        {
            return AddDynamicApi(services, DynamicApiConfigurations.Default, controllers);
        }

        /// <summary>
        /// Register Coddee dynamic API.
        /// </summary>
        public static IServiceCollection AddDynamicApi(this IServiceCollection services, DynamicApiConfigurations config, IEnumerable<Type> controllers)
        {
            var serviceProvider = services.BuildServiceProvider();
            var container = serviceProvider.GetService<IContainer>();
            var manager = new DynamicApiControllersManager();

            container.RegisterInstance(config);

            controllers.ForEach(manager.RegisterController);
            container.RegisterInstance(manager);

            var api = new DynamicApi(container);
            api.CacheRegisteredControllers();
            container.RegisterInstance(api);
            return services;
        }

        /// <summary>
        /// Add Coddee <see cref="DynamicApi"/> middleware
        /// </summary>
        public static IApplicationBuilder UseCoddeeDynamicApi(this IApplicationBuilder appBuilder)
        {
            appBuilder.UseMiddleware<DynamicApi>();
            return appBuilder;
        }
    }
}