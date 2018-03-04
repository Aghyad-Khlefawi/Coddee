// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Input;
using Coddee.WPF.Commands;

namespace Coddee.SQL
{
    /// <summary>
    /// Represent an SQL server instance
    /// </summary>
    public class SQLServer : BindableBase
    {
        /// <inheritdoc />
        public SQLServer(EventHandler<IEnumerable<string>> Connected)
        {
            ServerConnected += Connected;
        }

        private string _connection ;
        private event EventHandler<IEnumerable<string>> ServerConnected;

        /// <summary>
        /// The name of the server.
        /// </summary>
        public string Name { get; set; }

        private string _username;

        /// <summary>
        /// The login username
        /// </summary>
        public string Username
        {
            get { return _username; }
            set { SetProperty(ref this._username, value); }
        }

        private string _password;
        /// <summary>
        /// The login password
        /// </summary>
        public string Password
        {
            get { return _password; }
            set { SetProperty(ref this._password, value); }
        }

        private bool _integratedSecurity = true;
        /// <summary>
        /// Login using integrated security.
        /// </summary>
        public bool IntegratedSecurity
        {
            get { return _integratedSecurity; }
            set { SetProperty(ref this._integratedSecurity, value); }
        }

        /// <summary>
        /// Connect to the server.
        /// </summary>
        public ICommand ConnectCommand => new RelayCommand(Connect);

        private void Connect()
        {
            if (IntegratedSecurity || !string.IsNullOrEmpty(Username) && !string.IsNullOrEmpty(Password))
            {
                try
                {
                    ServerConnected?.Invoke(this, GetDatabaseList(false));
                }
                catch
                {
                    MessageBox.Show("Invalid login.");
                }
            }
        }

        /// <summary>
        /// Returns a list of the databases available on this server
        /// </summary>
        /// <param name="getSystemDbs">If set to true then the system databases(master,model...) will be included in the list</param>
        public List<string> GetDatabaseList(bool getSystemDbs = true)
        {
            var builder = new SqlConnectionStringBuilder
            {
                IntegratedSecurity = IntegratedSecurity,
                DataSource = Name
            };
            if (!string.IsNullOrEmpty(Username) && !string.IsNullOrEmpty(Password))
            {
                builder.UserID = Username;
                builder.Password = Password;
            }
            _connection = builder.ToString();
            return GetDatabaseList(_connection, getSystemDbs);
        }

        /// <summary>
        /// Returns a list of the databases available on the target server
        /// </summary>
        /// <param name="connection">The target server connection string</param>
        /// <param name="getSystemDbs">If set to true then the system databases(master,model...) will be included in the list</param>
        public static List<string> GetDatabaseList(string connection, bool getSystemDbs = true)
        {
            List<string> list = new List<string>();
            using (SqlConnection con = new SqlConnection(connection))
            {
                con.Open();
                var command = getSystemDbs
                    ? "SELECT name from sys.databases"
                    : "SELECT name FROM sys.databases WHERE database_id > 4 AND is_distributor = 0; ";
                using (SqlCommand cmd = new SqlCommand(command, con))
                {
                    using (IDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            list.Add(dr[0].ToString());
                        }
                    }
                }
            }
            return list;
        }

        /// <summary>
        /// Returns the connection string of the current database.
        /// </summary>
        /// <returns></returns>
        public string GetConnectedDatabase()
        {
            return _connection;
        }
    }
}