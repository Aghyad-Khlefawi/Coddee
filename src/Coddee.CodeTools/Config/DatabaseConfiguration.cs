// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System.Data.SqlClient;
using Coddee.WPF;

namespace Coddee.CodeTools.Config
{
    public class DatabaseConfiguration:BindableBase
    {
        private string _dbmlPath;
        public string DbmlPath
        {
            get { return _dbmlPath; }
            set { SetProperty(ref _dbmlPath, value); }
        }
        private string _dbConnection;
        public string DbConnection
        {
            get { return _dbConnection; }
            set
            {
                SetProperty(ref _dbConnection, value);
                var connection = new SqlConnectionStringBuilder(DbConnection);
                IsDbValid = true;
                DbTitle = $"{connection.InitialCatalog} ({connection.DataSource})";
            }
        }

        private string _dbTitle;
        public string DbTitle
        {
            get { return _dbTitle; }
            set { SetProperty(ref _dbTitle, value); }
        }

        private bool _isDbValid;
        public bool IsDbValid
        {
            get { return _isDbValid; }
            set { SetProperty(ref _isDbValid, value); }
        }

        public static explicit operator DatabaseConfigurationSerializable(DatabaseConfiguration config)
        {
            return new DatabaseConfigurationSerializable
            {
                DbmlPath = config.DbmlPath,
                DbConnection = config.DbConnection,
            };
        }
        public static explicit operator DatabaseConfiguration(DatabaseConfigurationSerializable config)
        {
            return new DatabaseConfiguration
            {
                DbmlPath = config.DbmlPath,
                DbConnection = config.DbConnection,
            };
        }
    }
    public class DatabaseConfigurationSerializable
    {
        public string DbmlPath { get; set; }
        public string DbConnection { get; set; }
    }
}
