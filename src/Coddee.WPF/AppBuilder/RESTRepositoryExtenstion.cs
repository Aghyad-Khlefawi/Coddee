// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using Coddee.AppBuilder;
using Coddee.Data.Rest;

namespace Coddee.WPF.AppBuilder
{
    public static class RESTRepositoryExtenstion
    {
        public static IWPFApplicationBuilder UseRESTRepositoryManager(
            this IWPFApplicationBuilder builder,
            string apiBaseURL,
            Action unauthorizedRequestHandler,
            string repositoriesAssembly,
            bool registerTheRepositoresInContainer)
        {
            builder.SetBuildAction(BuildActions.Repository, () =>
            {
                var repositoryManager = new RESTRepositoryManager();
                repositoryManager.Initialize(apiBaseURL, unauthorizedRequestHandler, builder.WPFBuilder.GetMapper());
                repositoryManager.RegisterRepositories(repositoriesAssembly);
                builder.WPFBuilder.SetRepositoryManager(repositoryManager, registerTheRepositoresInContainer);
            });
            return builder;
        }

        public static IWPFApplicationBuilder UseRESTRepositoryManager<TRepositoryManager>(
            this IWPFApplicationBuilder builder,
            string apiBaseURL,
            Action unauthorizedRequestHandler,
            string repositoriesAssembly,
            bool registerTheRepositoresInContainer)
            where TRepositoryManager : IRESTRepositoryManager, new()
        {
            builder.SetBuildAction(BuildActions.Repository, () =>
            {
                var repositoryManager = new TRepositoryManager();
                repositoryManager.Initialize(apiBaseURL, unauthorizedRequestHandler, builder.WPFBuilder.GetMapper());
                repositoryManager.RegisterRepositories(repositoriesAssembly);
                builder.WPFBuilder.SetRepositoryManager(repositoryManager, registerTheRepositoresInContainer);
            });
            return builder;
        }
    }
}