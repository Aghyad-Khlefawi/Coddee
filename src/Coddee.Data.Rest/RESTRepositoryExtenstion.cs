// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using System.Collections.Generic;
using Coddee.Data;
using Coddee.Data.REST;
using Coddee.Loggers;
using Coddee.Services;


namespace Coddee.AppBuilder
{
    /// <summary>
    /// Provides the required configurations to use REST repositories.
    /// </summary>
    public class RESTInitializerConfig
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="apiUrl">The API base URL</param>
        /// <param name="unauthorizedRequestHandler">An action called when an unauthorized response code received</param>
        /// <param name="repositoriesAssembly">The assembly name containing the repository <remarks>Without extension</remarks></param>
        /// <param name="registerTheRepositoresInContainer">Register the repositories in the dependency container</param>
        public RESTInitializerConfig(string apiUrl, Action unauthorizedRequestHandler, string repositoriesAssembly, bool registerTheRepositoresInContainer = true)
        {
            ApiUrl = apiUrl;
            UnauthorizedRequestHandler = unauthorizedRequestHandler;
            RepositoriesAssembly = repositoriesAssembly;
            RegisterTheRepositoresInContainer = registerTheRepositoresInContainer;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="apiUrl">The API base URL</param>
        /// <param name="unauthorizedRequestHandler">An action called when an unauthorized response code received</param>
        /// <param name="repositoriesTypes">The repositories to register The key is the interface type and the value is the implementation type</param>
        /// <param name="repositoriesAssembly">The assembly name containing the repository <remarks>Without extension</remarks></param>
        /// <param name="registerTheRepositoresInContainer">Register the repositories in the dependency container</param>
        public RESTInitializerConfig(string apiUrl, Action unauthorizedRequestHandler, KeyValuePair<Type, Type>[] repositoriesTypes, string repositoriesAssembly, bool registerTheRepositoresInContainer = true)
        : this(apiUrl, unauthorizedRequestHandler, repositoriesAssembly, registerTheRepositoresInContainer)
        {
            RepositoriesTypes = repositoriesTypes;
        }

        /// <summary>
        /// The API base URL
        /// </summary>
        public string ApiUrl { get; set; }

        /// <summary>
        /// An action called when an unauthorized response code received
        /// </summary>
        public Action UnauthorizedRequestHandler { get; set; }

        /// <summary>
        /// The assembly name containing the repository
        /// <remarks>Without extension</remarks>
        /// </summary>
        public string RepositoriesAssembly { get; set; }

        /// <summary>
        /// Register the repositories in the dependency container
        /// </summary>
        public bool RegisterTheRepositoresInContainer { get; set; }


        /// <summary>
        /// The repository types that will be registered
        /// </summary>
        public KeyValuePair<Type,Type>[] RepositoriesTypes { get; }

    }

    /// <summary>
    /// Builder extension for Rest repositories.
    /// </summary>
    public static class RESTRepositoryExtenstion
    {
        private const string EventsSource = "ApplicationBuilder";

        /// <summary>
        /// Register REST repositories and adds a <see cref="RESTRepositoryInitializer"/> to the <see cref="IRepositoryManager"/>
        /// </summary>
        public static TBuilder UseRESTRepositories<TBuilder>(
            this TBuilder builder,
            Func<IContainer, RESTInitializerConfig> config)
            where TBuilder : IApplicationBuilder
        {
            builder.BuildActionsCoordinator.AddAction(DefaultBuildActions.RESTRepositoryBuildAction((container) =>
            {
                if (!container.IsRegistered<IRepositoryManager>())
                    throw new ApplicationBuildException("RepositoryManager is not registered. call UseSingletonRepositoryManager or UseTransientRepositoryManager to configuration the repository manager.");

                var repositoryManager = container.Resolve<IRepositoryManager>();

                var configRes = config(container);
                repositoryManager.AddRepositoryInitializer(new RESTRepositoryInitializer(configRes.ApiUrl, configRes.UnauthorizedRequestHandler, container.Resolve<IObjectMapper>()));

                if (!string.IsNullOrEmpty(configRes.RepositoriesAssembly))
                    repositoryManager.RegisterRepositories(configRes.RepositoriesAssembly);
                if (configRes.RepositoriesTypes != null)
                    repositoryManager.RegisterRepositories(configRes.RepositoriesTypes);

                var logger = container.Resolve<ILogger>();
                container.RegisterInstance<IRepositoryManager>(repositoryManager);
                if (configRes.RegisterTheRepositoresInContainer)
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