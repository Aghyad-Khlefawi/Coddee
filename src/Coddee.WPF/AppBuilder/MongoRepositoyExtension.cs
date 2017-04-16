// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using Coddee.AppBuilder;
using Coddee.Data.MongoDB;

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
            builder.SetBuildAction(BuildActions.Repository,
                                   () =>
                                   {
                                       IMongoRepositoryManager repositoryManager = new TRepositoryManager();
                                       repositoryManager.Initialize(new MongoDBManager(connection, databaseName),builder.WPFBuilder.GetMapper());
                                       repositoryManager.RegisterRepositories(repositoriesAssembly);
                                       builder.WPFBuilder.SetRepositoryManager(repositoryManager,registerTheRepositoresInContainer);
                                   });
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