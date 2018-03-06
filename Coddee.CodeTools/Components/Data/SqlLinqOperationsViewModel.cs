// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
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
using Coddee.Mvvm;
using Coddee.Services;
using Coddee.WPF;
using Coddee.WPF.Commands;

namespace Coddee.CodeTools.Components.Data
{
    public class SqlLinqOperationsViewModel : VsViewModelBase<SqlLinqOperationsView>
    {
        private static PluralizationService _pluralizationServices;

        private ImportWizardViewModel _importWizardViewModel;
        private DbmlCompare _dbmlCompare;

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
        private IReactiveCommand _importModelsCommand;
        public IReactiveCommand ImportModelsCommand
        {
            get { return _importModelsCommand ?? (_importModelsCommand = CreateReactiveCommand(ImportModels)); }
            set { SetProperty(ref _importModelsCommand, value); }
        }
        private IReactiveCommand _importRepositoriesCommand;
        public IReactiveCommand ImportRepositoriesCommand
        {
            get { return _importRepositoriesCommand ?? (_importRepositoriesCommand = CreateReactiveCommand(ImportRepositories)); }
            set { SetProperty(ref _importRepositoriesCommand, value); }
        }
        private IReactiveCommand _importLinqCommand;
        public IReactiveCommand ImportLinqCommand
        {
            get { return _importLinqCommand ?? (_importLinqCommand = CreateReactiveCommand(ImportLinq)); }
            set { SetProperty(ref _importLinqCommand, value); }
        }
        private IReactiveCommand _importRestCommand;
        public IReactiveCommand ImportRestCommand
        {
            get { return _importRestCommand ?? (_importRestCommand = CreateReactiveCommand(ImportRest)); }
            set { SetProperty(ref _importRestCommand, value); }
        }

        public async void ImportRest()
        {
            void ImportAllInternal()
            {
                _importWizardViewModel.SetTables(new TableImportArguments{ImprotRestRepository = true},Tables.ToArray());
            }
            await ToggleBusyAsync(Task.Run(new Action(ImportAllInternal)));
            _importWizardViewModel.Show();
        }
        public async void ImportLinq()
        {
            void ImportAllInternal()
            {
                _importWizardViewModel.SetTables(new TableImportArguments { ImprotLinqRepository = true }, Tables.ToArray());
            }
            await ToggleBusyAsync(Task.Run(new Action(ImportAllInternal)));
            _importWizardViewModel.Show();
        }
        public async void ImportRepositories()
        {
            void ImportAllInternal()
            {
                _importWizardViewModel.SetTables(new TableImportArguments { ImprotRepository = true }, Tables.ToArray());
            }
            await ToggleBusyAsync(Task.Run(new Action(ImportAllInternal)));
            _importWizardViewModel.Show();
        }
        public async void ImportModels()
        {
            void ImportAllInternal()
            {
                _importWizardViewModel.SetTables(new TableImportArguments { ImprotModel = true }, Tables.ToArray());
            }
            await ToggleBusyAsync(Task.Run(new Action(ImportAllInternal)));
            _importWizardViewModel.Show();
        }
        public async void ImportAll()
        {
            void ImportAllInternal()
            {
                _importWizardViewModel.SetTables(TableImportArguments.All, Tables.ToArray());
            }
            await ToggleBusyAsync(Task.Run(new Action(ImportAllInternal)));
            _importWizardViewModel.Show();
        }
        protected override async Task OnInitialization()
        {
            await base.OnInitialization();
            Tables = AsyncObservableCollection<SqlTableViewModel>.Create();
            _pluralizationServices = PluralizationService.CreateService(CultureInfo.GetCultureInfo("en-us"));
            _importWizardViewModel = await InitializeViewModel<ImportWizardViewModel>();
            _dbmlCompare = new DbmlCompare();
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
                        Tables = new List<SqlTable>()
                    };
                    var tables = await GetDbTables(conn, SqlDatabase);
                    var compareRes = _dbmlCompare.Compare(_solution.DatabaseConfigurations.DbmlPath, tables);
                    SqlDatabase.Tables.AddRange(tables);
                    conn.Dispose();
                    Tables.ClearAndFill(await SqlDatabase.Tables.OrderBy(e => e.TableName).Select(CreateSqlTableViewModel));
                    TablesCount = Tables.Count;
                    ValidateTables(Tables);

                }
            }

            await ToggleBusyAsync(Task.Run(GetInfoInternal));
        }

        public async Task<SqlTableViewModel> CreateSqlTableViewModel(SqlTable table)
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
            var models = new DirectoryInfo(!string.IsNullOrEmpty(_solution.ModelProjectConfiguration.GeneratedCodeFolder)? Path.Combine(_solution.ModelProjectConfiguration.ProjectFolder, _solution.ModelProjectConfiguration.GeneratedCodeFolder): _solution.ModelProjectConfiguration.ProjectFolder).GetFiles();
            var repositories = new DirectoryInfo(!string.IsNullOrEmpty(_solution.DataProjectConfiguration.GeneratedCodeFolder) ? Path.Combine(_solution.DataProjectConfiguration.ProjectFolder, _solution.DataProjectConfiguration.GeneratedCodeFolder): _solution.DataProjectConfiguration.ProjectFolder).GetFiles();
            var linqRepositories = new DirectoryInfo(!string.IsNullOrEmpty(_solution.LinqProjectConfiguration.GeneratedCodeFolder) ? Path.Combine(_solution.LinqProjectConfiguration.ProjectFolder, _solution.LinqProjectConfiguration.GeneratedCodeFolder) : _solution.DataProjectConfiguration.ProjectFolder).GetFiles();
            var restRepositories = new DirectoryInfo(!string.IsNullOrEmpty(_solution.RestProjectConfiguration.GeneratedCodeFolder) ? Path.Combine(_solution.RestProjectConfiguration.ProjectFolder, _solution.RestProjectConfiguration.GeneratedCodeFolder) : _solution.RestProjectConfiguration.ProjectFolder).GetFiles();


            foreach (var table in sqlTables)
            {
                table.SingularName = _pluralizationServices.Singularize(table.TableName);
                table.IsModelValid = models.Any(e => e.Name == table.GetModelFileName());
                table.IsRepositoryInterfaceValid = repositories.Any(e => e.Name == table.GetRepsotioryInterfaceFileName());
                table.IsLinqRepositoryValid = linqRepositories.Any(e => e.Name == table.GetRepsotioryFileName());
                table.IsRestRepositoryValid = restRepositories.Any(e => e.Name == table.GetRepsotioryFileName());
            }
        }

        private async Task<IEnumerable<SqlTable>> GetDbTables(SqlConnection conn, SqlDatabase db)
        {
            await SqlQueries.UseDatabase.ExecuteNonQuery(conn, db.DatabaseName);
            var tables = await SqlQueries.GetDatabaseTables.Execute(conn);
            foreach (var table in tables)
            {
                table.Columns = new List<SqlColumn>(await SqlQueries.GetTableColumns.Execute(conn, table.TableName).ForEach(e => e.Table = table));
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
            if (_solution.DatabaseConfigurations?.IsDbValid ?? false)
            {
                var connection = _solution.DatabaseConfigurations.DbConnection;
                var sqlConnBuilder = new SqlConnectionStringBuilder(connection);
                DatabaseName = $"{sqlConnBuilder.InitialCatalog} ({sqlConnBuilder.DataSource})";
            }
        }
    }
}