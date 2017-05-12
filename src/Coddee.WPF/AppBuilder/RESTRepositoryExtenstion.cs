// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using Coddee.AppBuilder;
using Coddee.Data.REST;
using Coddee.WPF.Modules.Interfaces;
using Microsoft.Practices.Unity;

namespace Coddee.WPF.AppBuilder
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
        public static IWPFApplicationBuilder UseRESTRepositoryManager(
            this IWPFApplicationBuilder builder,
            Func<IConfigurationManager,RESTRepositoryManagerConfig> config)
        {
            return builder.UseRESTRepositoryManager<RESTRepositoryManager>(config);
        }

        public static IWPFApplicationBuilder UseRESTRepositoryManager<TRepositoryManager>(
            this IWPFApplicationBuilder builder,
            Func<IConfigurationManager, RESTRepositoryManagerConfig> config)
            where TRepositoryManager : IRESTRepositoryManager, new()
        {
            builder.SetBuildAction(BuildActions.Repository, () =>
            {
                var repositoryManager = new TRepositoryManager();
                var configRes = config(builder.WPFBuilder.GetContainer().Resolve<IConfigurationManager>());
                repositoryManager.Initialize(configRes.ApiUrl, configRes.UnauthorizedRequestHandler, builder.WPFBuilder.GetMapper());
                repositoryManager.RegisterRepositories(configRes.RepositoriesAssembly);
                builder.WPFBuilder.SetRepositoryManager(repositoryManager, configRes.RegisterTheRepositoresInContainer);
            });
            return builder;
        }
    }
}