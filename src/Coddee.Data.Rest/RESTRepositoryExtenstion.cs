// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using Coddee.Data;
using Coddee.Data.REST;
using Coddee.Loggers;
using Coddee.Services;


namespace Coddee.AppBuilder
{
    public class RESTRepositoryManagerConfig
    {
        public RESTRepositoryManagerConfig(string apiUrl, Action unauthorizedRequestHandler, string repositoriesAssembly, bool registerTheRepositoresInContainer = true)
        {
            ApiUrl = apiUrl;
            UnauthorizedRequestHandler = unauthorizedRequestHandler;
            RepositoriesAssembly = repositoriesAssembly;
            RegisterTheRepositoresInContainer = registerTheRepositoresInContainer;
        }

        public string ApiUrl { get; set; }
        public Action UnauthorizedRequestHandler { get; set; }
        public string RepositoriesAssembly { get; set; }
        public bool RegisterTheRepositoresInContainer { get; set; }
    }
    public static class RESTRepositoryExtenstion
    {
        private const string EventsSource = "ApplicationBuilder";
        public static IApplicationBuilder UseRESTRepositoryManager(
            this IApplicationBuilder builder,
            Func<IConfigurationManager, RESTRepositoryManagerConfig> config)
        {
            return builder.UseRESTRepositoryManager<RESTRepositoryManager>(config);
        }

        public static IApplicationBuilder UseRESTRepositoryManager<TRepositoryManager>(
            this IApplicationBuilder builder,
            Func<IConfigurationManager, RESTRepositoryManagerConfig> config)
            where TRepositoryManager : IRESTRepositoryManager, new()
        {
            builder.BuildActionsCoordinator.AddAction(DefaultBuildActions.RepositoryBuildAction((container) =>
            {
                var repositoryManager = new TRepositoryManager();
                var configRes = config(container.Resolve<IConfigurationManager>());
                repositoryManager.Initialize(configRes.ApiUrl, configRes.UnauthorizedRequestHandler, container.Resolve<IObjectMapper>());
                repositoryManager.RegisterRepositories(configRes.RepositoriesAssembly);

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