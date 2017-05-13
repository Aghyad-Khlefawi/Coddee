// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;
using Coddee.Collections;
using Coddee.WPF;
using Coddee.WPF.Commands;
using Coddee.WPF.SQLBrowser;

namespace Coddee.SQL
{
    /// <summary>
    /// SQL server browser.
    /// Provides a GUI for the use to selected an SQL server database
    /// </summary>
    public class SQLDBBrowse : ViewModelBase<SQLDBBrowserView>, ISQLDBBrowser
    {
        public SQLDBBrowse()
        {
            _customServer = new SQLServer(ServerConnected);
        }

        private string _connection = null;
        private SQLServer _connectedServer;


        private SQLServer _customServer;
        public SQLServer CustomServer
        {
            get { return _customServer; }
            set { SetProperty(ref this._customServer, value); }
        }

        private bool _isBusy;
        public bool IsBusy
        {
            get { return _isBusy; }
            set { SetProperty(ref this._isBusy, value); }
        }

        private AsyncObservableCollection<SQLServer> _serversList;
        public AsyncObservableCollection<SQLServer> ServersList
        {
            get { return _serversList; }
            set { SetProperty(ref this._serversList, value); }
        }

        private AsyncObservableCollection<string> _databases;
        public AsyncObservableCollection<string> Databases
        {
            get { return _databases; }
            set
            {
                SetProperty(ref this._databases, value); 
                if(value!=null)
                    DatabasesView = CollectionViewSource.GetDefaultView(value);
            }
        }

        private ICollectionView _databasesView;
        public ICollectionView DatabasesView
        {
            get { return _databasesView; }
            set { SetProperty(ref this._databasesView, value); }
        }

        private string _searchValue;
        public string SearchValue
        {
            get { return _searchValue; }
            set
            {
                SetProperty(ref this._searchValue, value);
                if (value != null)
                    FilterDatabases(value);
            }
        }

        private void FilterDatabases(string value)
        {
            var searchValue = value.ToLower();
            DatabasesView.Filter = e =>
            {
                var db = (string) e;
                return db.ToLower().Contains(searchValue);
            };
        }

        public ICommand UseDatabaseCommand => new RelayCommand(UseDatabase);

        /// <summary>
        /// Return a connection string based on the currently selected database
        /// </summary>
        private void UseDatabase()
        {
            if (_connectedServer != null && Databases.SelectedItem != null)
            {
                _connection = new SqlConnectionStringBuilder(_connectedServer.GetConnectedDatabase())
                {
                    InitialCatalog = Databases.SelectedItem
                }.ToString();
                _view.DialogResult = true;
                _view.Close();
            }
        }

        /// <summary>
        /// Show the GUI to select a new database
        /// </summary>
        public string GetDatabaseConnectionString()
        {
            IsBusy = true;

            Databases = AsyncObservableCollection<string>.Create();
            ServersList = AsyncObservableCollection<SQLServer>.Create();

            GetView();
            GetServers();
            CreateView();
            _view.ShowDialog();
            return _connection;
        }

        /// <summary>
        /// Returns all the available servers on the local network
        /// </summary>
        private async void GetServers()
        {
            await Task.Run(() =>
            {
                var servers = SQLNativeBrowser.GetServersNative();
                if (servers != null)
                    ServersList.Fill(servers.Select(e => new SQLServer(ServerConnected)
                    {
                        Name = e
                    }));
                IsBusy = false;
            });
        }

        /// <summary>
        /// Triggered on the connect command
        /// </summary>
        /// <param name="sender">The SQL server object</param>
        /// <param name="databases">The databases on the connected server</param>
        void ServerConnected(object sender, IEnumerable<string> databases)
        {
            _connectedServer = (SQLServer) sender;

            Databases.Clear();
            Databases.Fill(databases.OrderBy(e => e));
        }
    }
}