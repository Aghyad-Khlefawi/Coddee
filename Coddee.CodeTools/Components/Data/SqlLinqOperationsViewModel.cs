// Copyright (c) Aghyad khlefawi. All rights reserved.  
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
        private IReactiveCommand _importAllCommand;
        public IReactiveCommand ImportAllCommand
        {
            get { return _importAllCommand ?? (_importAllCommand = CreateReactiveCommand(ImportAll)); }
            set { SetProperty(ref _importAllCommand, value); }
        }
        private int _tablesCount;
        public int TablesCount
        {
            get { return _tablesCount; }
            set { SetProperty(ref _tablesCount, value); }
        }
        public async void ImportAll()
        {
            Task ImportAllInternal()
            {
                return _importWizardViewModel.SetTables(Tables.ToArray());
            }
            await ToggleBusyAsync(Task.Run(ImportAllInternal));
            _importWizardViewModel.Show();
        }
        protected override async Task OnInitialization()
        {
            await base.OnInitialization();
            Tables = AsyncObservableCollection<SqlTableViewModel>.Create();
            _pluralizationServices = PluralizationService.CreateService(CultureInfo.GetCultureInfo("en-us"));
            _importWizardViewModel = await InitializeViewModel<ImportWizardViewModel>();
            _eventDispatcher.GetEvent<SqlLinqConfigurationUpdatedEvents>().Subscribe(e =>
            {
                GetDbTitle();
            });
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
            async Task GetInfoInternal()
            {
                if (_solution.DatabaseConfigurations.IsDbValid)
                {
                    var connection = _solution.DatabaseConfigurations.DbConnection;
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
                    Tables.ClearAndFill(await SqlDatabase.Tables.OrderBy(e => e.TableName).Select(CreateSqlTableViewModel));
                    TablesCount = Tables.Count;
                    ValidateTables(Tables);

                }
            }

            await ToggleBusyAsync(Task.Run(GetInfoInternal));
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
            var models = new DirectoryInfo(Path.Combine(_solution.ModelProjectConfiguration.ProjectFolder, _solution.ModelProjectConfiguration.GeneratedCodeFolder)).GetFiles();
            var repositories = new DirectoryInfo(Path.Combine(_solution.DataProjectConfiguration.ProjectFolder, _solution.DataProjectConfiguration.GeneratedCodeFolder)).GetFiles();
            var linqRepositories = new DirectoryInfo(Path.Combine(_solution.LinqProjectConfiguration.ProjectFolder, _solution.DataProjectConfiguration.GeneratedCodeFolder)).GetFiles();
            var restRepositories = new DirectoryInfo(Path.Combine(_solution.RestProjectConfiguration.ProjectFolder, _solution.DataProjectConfiguration.GeneratedCodeFolder)).GetFiles();


            foreach (var table in sqlTables)
            {
                table.SingularName = _pluralizationServices.Singularize(table.TableName);
                table.IsModelValid = models.Any(e => e.Name == table.GetModelFileName());
                table.IsRepositoryInterfaceValid = repositories.Any(e => e.Name == table.GetRepsotioryInterfaceFileName());
                table.IsLinqRepositoryValid = linqRepositories.Any(e => e.Name == table.GetRepsotioryFileName());
                table.IsRestRepositoryValid = restRepositories.Any(e => e.Name == table.GetRepsotioryFileName());
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
            GetDbTitle();
        }

        private void GetDbTitle()
        {
            if (_solution.DatabaseConfigurations.IsDbValid)
            {
                var connection = _solution.DatabaseConfigurations.DbConnection;
                var sqlConnBuilder = new SqlConnectionStringBuilder(connection);
                DatabaseName = $"{sqlConnBuilder.InitialCatalog} ({sqlConnBuilder.DataSource})";
            }
        }
    }
}