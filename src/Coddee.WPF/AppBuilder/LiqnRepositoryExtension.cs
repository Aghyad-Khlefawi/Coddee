// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using System.IO;
using Coddee.AppBuilder;
using Coddee.Crypto;
using Coddee.Data;
using Coddee.Data.LinqToSQL;
using Coddee.Loggers;
using Coddee.Security;
using Coddee.Services;
using Coddee.Services.Configuration;
using Coddee.SQL;
using Microsoft.Practices.Unity;

namespace Coddee.WPF.AppBuilder
{
    public static class LiqnRepositoryExtension
    {
        private const string EventsSource = "WPFApplicationBuilder";

        private static IConfigurationFile CreateConnectionsConfigFile(string key)
        {
            return new ConfigurationFile("conn",
                                    Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "conn.config"),
                                    new CryptoProvider
                                    {
                                        Decryptor = value=>EncryptionHelper.Decrypt(value,key),
                                        Encryptor = value=>EncryptionHelper.EncryptStringAsBase64(value,key)
                                    });
        }

        public static IWPFApplicationBuilder UseLinqRepositoryManager<TDBManager, TRepositoryManager>(
            this IWPFApplicationBuilder builder,
            string repositoriesAssembly,
            bool registerTheRepositoresInContainer,
            Action ConnectionStringNotFound = null,
            RepositoryConfigurations config = null)
            where TDBManager : ILinqDBManager, new()
            where TRepositoryManager : ILinqRepositoryManager, new()
        {

            builder.BuildActionsCoordinator.AddAction(DefaultBuildActions.RepositoryBuildAction((container) =>
            {
                var connectionString = GetSQLDBConnection(container);
                if (string.IsNullOrEmpty(connectionString))
                {
                    ConnectionStringNotFound();
                    return;
                }
                CreateRepositoryManager<TDBManager, TRepositoryManager>(builder, container, connectionString, repositoriesAssembly, registerTheRepositoresInContainer, config);
            }));
            return builder;
        }

        public static IWPFApplicationBuilder UseLinqRepositoryManager<TDBManager, TRepositoryManager>(
            this IWPFApplicationBuilder builder,
            string connectionString,
            string repositoriesAssembly,
            bool registerTheRepositoresInContainer,
            RepositoryConfigurations config = null)
            where TDBManager : ILinqDBManager, new()
            where TRepositoryManager : ILinqRepositoryManager, new()
        {
            builder.BuildActionsCoordinator.AddAction(DefaultBuildActions.RepositoryBuildAction((container) =>
            {
                CreateRepositoryManager<TDBManager, TRepositoryManager>(builder, container, connectionString, repositoriesAssembly, registerTheRepositoresInContainer, config);
            }));
            return builder;
        }

        private static void CreateRepositoryManager<TDBManager, TRepositoryManager>(IWPFApplicationBuilder builder, IUnityContainer container, string connectionString, string repositoriesAssembly, bool registerTheRepositoresInContainer, RepositoryConfigurations config = null)
            where TDBManager : ILinqDBManager, new()
            where TRepositoryManager : ILinqRepositoryManager, new()
        {
            var dbManager = new TDBManager();
            container.RegisterInstance<ILinqDBManager>(dbManager);
            dbManager.Initialize(connectionString);
            var repositoryManager = new TRepositoryManager();
            repositoryManager.Initialize(dbManager, container.Resolve<IObjectMapper>(), config);
            repositoryManager.RegisterRepositories(repositoriesAssembly);

            var logger = container.Resolve<ILogger>();
            container.RegisterInstance<IRepositoryManager>(repositoryManager);
            if (registerTheRepositoresInContainer)
                foreach (var repository in repositoryManager.GetRepositories())
                {
                    logger.Log(EventsSource,
                                $"Registering repository of type {repository.GetType().Name}",
                                LogRecordTypes.Debug);
                    container.RegisterInstance(repository.ImplementedInterface, repository);
                }
        }

        /// <summary>
        /// Uses SQLDBBrowser to let the use select an SQL Server database
        /// </summary>
        /// <returns></returns>
        private static string GetSQLDBConnection(IUnityContainer container)
        {
            var config = container.Resolve<IConfigurationManager>();
            var app = container.Resolve<IApplication>();
            string connection = null;
            var connectionsConfigFile = CreateConnectionsConfigFile(app.ApplicationID.ToString());
            config.AddConfigurationFile(connectionsConfigFile);
            if (config.TryGetValue(BuiltInConfigurationKeys.SQLDBConnection, out connection, connectionsConfigFile.Name) &&
                !string.IsNullOrEmpty(connection))
            {
                return connection;
            }
            var browser = container.Resolve<ISQLDBBrowser>();
            connection = browser.GetDatabaseConnectionString();
            config.SetValue(BuiltInConfigurationKeys.SQLDBConnection, connection, connectionsConfigFile.Name);
            return connection;
        }

    }
}