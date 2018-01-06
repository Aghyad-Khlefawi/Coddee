﻿// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System.Collections.Generic;
using System.Data.Entity.Design.PluralizationServices;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Coddee.CodeTools.Sql;
using Coddee.CodeTools.Sql.Queries;
using Coddee.Collections;
using Coddee.Services;
using Coddee.WPF;
using Coddee.WPF.Commands;

namespace Coddee.CodeTools.Components.Data
{
    public class SqlLinqOperationsViewModel : VsViewModelBase<SqlLinqOperationsView>
    {
        private static PluralizationService _pluralizationServices;

        public SqlLinqOperationsViewModel()
        {

        }

        private ImportWizardViewModel _importWizardViewModel;

        public event ViewModelEventHandler ConfigureCliked;

        private AsyncObservableCollection<SqlTableViewModel> _tables;
        public AsyncObservableCollection<SqlTableViewModel> Tables
        {
            get { return _tables; }
            set { SetProperty(ref _tables, value); }
        }

        private string _databaseName;
        public string DatabaseName
        {
            get { return _databaseName; }
            set { SetProperty(ref _databaseName, value); }
        }

        private IReactiveCommand _configureCommand;
        public IReactiveCommand ConfigureCommand
        {
            get { return _configureCommand ?? (_configureCommand = CreateReactiveCommand(Configure)); }
            set { SetProperty(ref _configureCommand, value); }
        }

        private IReactiveCommand _getInfoCommand;
        public IReactiveCommand GetInfoCommand
        {
            get { return _getInfoCommand ?? (_getInfoCommand = CreateReactiveCommand(async () => await GetInfo())); }
            set { SetProperty(ref _getInfoCommand, value); }
        }

        protected override async Task OnInitialization()
        {
            await base.OnInitialization();
            Tables = AsyncObservableCollection<SqlTableViewModel>.Create();
            _pluralizationServices = PluralizationService.CreateService(CultureInfo.GetCultureInfo("en-us"));
            _importWizardViewModel = await InitializeViewModel<ImportWizardViewModel>();
        }
        public void Configure()
        {
            ConfigureCliked?.Invoke(this);
        }

        protected override void OnDesignMode()
        {
            base.OnDesignMode();
            Tables = new AsyncObservableCollection<SqlTableViewModel>
            {
                new SqlTableViewModel
                {
                    TableName = "Table1",
                    SchemaName = "Schema"
                },new SqlTableViewModel
                {
                    TableName = "Table3",
                    SchemaName = "Schema"
                },new SqlTableViewModel
                {
                    TableName = "Table2",
                    SchemaName = "Schema"
                }
            };
        }

        public async Task GetInfo()
        {
            IsBusy = true;
            if (_currentSolutionConfigFile.TryGetValue(ConfigKeys.SqlLinq_DbConnection, out string connection))
            {
                var sqlConnBuilder = new SqlConnectionStringBuilder(connection);
                var conn = new SqlConnection(connection);
                var SqlDatabase = new SqlDatabase
                {
                    DatabaseName = sqlConnBuilder.InitialCatalog,
                    ConnectionString = connection,
                    Tables = new List<SqlDbTable>()
                };
                var tables = await GetDbTables(conn, SqlDatabase);
                SqlDatabase.Tables.AddRange(tables);
                conn.Dispose();
                Tables.ClearAndFill(await SqlDatabase.Tables.Select(CreateSqlTableViewModel));
                ValidateTables(Tables);

            }
            IsBusy = false;
        }

        public async Task<SqlTableViewModel> CreateSqlTableViewModel(SqlDbTable table)
        {
            var sqlTableViewModel = new SqlTableViewModel(_importWizardViewModel)
            {
                TableName = table.TableName,
                SchemaName = table.SchemaName,
                Columns = table.Columns
            };
            await sqlTableViewModel.Initialize();
            return sqlTableViewModel;
        }

        private void ValidateTables(IEnumerable<SqlTableViewModel> sqlTables)
        {
            CsProject modeslProj = _currentSolutionConfigFile.GetValue<CsProject>(ConfigKeys.SqlLinq_Projects_Models);
            var directory = new DirectoryInfo(Path.Combine(Path.GetDirectoryName(modeslProj.ProjectPath), modeslProj.Folder));
            var models = directory.GetFiles();
            foreach (var table in sqlTables)
            {
                table.SingularName = _pluralizationServices.Singularize(table.TableName);
                table.IsModelValid = models.Any(e => e.Name == $"{table.SingularName}.cs");
            }
        }

        private async Task<IEnumerable<SqlDbTable>> GetDbTables(SqlConnection conn, SqlDatabase db)
        {
            await SqlQueries.UseDatabase.ExecuteNonQuery(conn, db.DatabaseName);
            var tables = await SqlQueries.GetDatabaseTables.Execute(conn);
            foreach (var table in tables)
            {
                table.Columns = new List<SqlTableColumn>(await SqlQueries.GetTableColumns.Execute(conn, table.TableName));
            }
            return tables;
        }

        protected override void SolutionLoaded(IConfigurationFile config)
        {
            base.SolutionLoaded(config);
            if (_currentSolutionConfigFile.TryGetValue(ConfigKeys.SqlLinq_DbConnection, out string connection))
            {
                var sqlConnBuilder = new SqlConnectionStringBuilder(connection);
                DatabaseName = $"{sqlConnBuilder.InitialCatalog} ({sqlConnBuilder.DataSource})";
            }
        }
    }
}