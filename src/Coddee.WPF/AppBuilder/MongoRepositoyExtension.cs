// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using Coddee.Data.MongoDB;
using Microsoft.Practices.Unity;

namespace Coddee.WPF.AppBuilder
{
    public static class MongoRepositoyExtension
    {
        public static IWPFApplicationBuilder UseMongoDBRepository<TRepositoryManager>(
            this IWPFApplicationBuilder builder,
            string connection,
            string databaseName,
            string repositoriesAssembly,
            bool registerTheRepositoresInContainer)
            where TRepositoryManager : IMongoRepositoryManager, new()
        {
            builder.BuildActionsCoordinator.AddAction(DefaultBuildActions.RepositoryBuildAction((container) =>
            {

                IMongoRepositoryManager repositoryManager = new TRepositoryManager();
                repositoryManager.Initialize(new MongoDBManager(connection, databaseName), container.Resolve<IObjectMapper>());
                repositoryManager.RegisterRepositories(repositoriesAssembly);
                builder.WPFBuilder.SetRepositoryManager(repositoryManager, registerTheRepositoresInContainer);
            }));
            return builder;
        }

        public static IWPFApplicationBuilder UseMongoDBRepository(
            this IWPFApplicationBuilder builder,
            string connection,
            string databaseName,
            string repositoriesAssembly,
            bool registerTheRepositoresInContainer)
        {
            return builder.UseMongoDBRepository<MongoRepositoryManager>(connection,
                                                                        databaseName,
                                                                        repositoriesAssembly,
                                                                        registerTheRepositoresInContainer);
        }
    }
}