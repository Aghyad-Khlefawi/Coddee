// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using System.Collections.Generic;
using System.Linq;
using Coddee.AppBuilder;
using Coddee.AspNetCore.Sync;
using Coddee.Data;
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
        /// Register Coddee dynamic API.
        /// </summary>
        public static IServiceCollection AddDynamicApi(this IServiceCollection services)
        {
            return AddDynamicApi(services, null, null);
        }

        /// <summary>
        /// Configure the application to use SignalR hub form repository sync.
        /// </summary>
        public static void AddRepositorySyncHub(this IServiceCollection services)
        {
            services.AddSingleton(new HubAuthorizationProvider());
            services.AddSignalR();
        }

        /// <summary>
        /// Add SignalR middleware and configure repository sync.
        /// </summary>
        public static IApplicationBuilder UseRepositorySyncHub(this IApplicationBuilder appBuilder)
        {
            appBuilder.UseSignalR(routes =>
            {
                routes.MapHub<RepositorySyncHub>("/repoSync");
            });
            return appBuilder;
        }

        /// <summary>
        /// Register Coddee dynamic API.
        /// </summary>
        public static IServiceCollection AddDynamicApi(this IServiceCollection services, IEnumerable<Type> controllers)
        {
            return AddDynamicApi(services, null, controllers);
        }

        /// <summary>
        /// Register Coddee dynamic API.
        /// </summary>
        public static IServiceCollection AddDynamicApi(this IServiceCollection services, Action<DynamicApiConfigurations> config, IEnumerable<Type> controllers)
        {
            var serviceProvider = services.BuildServiceProvider();
            var container = serviceProvider.GetService<IContainer>();
            var manager = new DynamicApiControllersManager(container);

            DynamicApiConfigurations configInstance = DynamicApiConfigurations.Default();
            config?.Invoke(configInstance);
            container.RegisterInstance(configInstance);

            controllers.ForEach(manager.RegisterController);
            container.RegisterInstance(manager);

            var api = new DynamicApi(container);
            api.CacheActions();
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