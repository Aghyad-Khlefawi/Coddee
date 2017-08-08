// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using System.IO;
using Coddee.AppBuilder;
using Coddee.Crypto;
using Coddee.Security;
using Coddee.Services;
using Coddee.Services.Configuration;
using Coddee.SQL;
using Microsoft.Practices.Unity;

namespace Coddee.WPF.Modules.SQLBrowser
{
    public class SQLConnectHelper
    {
        public static string GetSQLDBConnection(IUnityContainer container)
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
        private static IConfigurationFile CreateConnectionsConfigFile(string key)
        {
            return new ConfigurationFile("conn",
                                         Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "conn.config"),
                                         new CryptoProvider
                                         {
                                             Decryptor = value => EncryptionHelper.Decrypt(value, key),
                                             Encryptor = value => EncryptionHelper.EncryptStringAsBase64(value, key)
                                         });
        }
    }
}
