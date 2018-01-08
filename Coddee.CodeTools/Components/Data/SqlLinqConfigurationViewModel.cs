// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using System.Xml;
using Coddee.Collections;
using Coddee.Data.LinqToSQL;
using Coddee.Services;
using Coddee.SQL;
using Coddee.WPF;
using Coddee.WPF.Commands;

namespace Coddee.CodeTools.Components.Data
{
    public class SqlLinqConfigurationViewModel : VsViewModelBase<SqlLinqConfigurationView>
    {
        private readonly ISQLDBBrowser _sqldbBrowser;

        public SqlLinqConfigurationViewModel()
        {

        }

        public SqlLinqConfigurationViewModel(ISQLDBBrowser sqldbBrowser, ISolutionHelper solutionHelper)
            : this()
        {
            _sqldbBrowser = sqldbBrowser;
            _solutionHelper = solutionHelper;
        }


        public event ViewModelEventHandler ConfigurationCompleted;

        private ModelProjectConfiguration _modelConfigurations;
        public ModelProjectConfiguration ModelConfigurations
        {
            get { return _modelConfigurations; }
            set
            {
                SetProperty(ref _modelConfigurations, value);
                if (_solution != null)
                    _solution.ModelProjectConfiguration = value;
            }
        }

        private DataProjectConfiguration _dataConfigurations;
        public DataProjectConfiguration DataConfigurations
        {
            get { return _dataConfigurations; }
            set
            {
                SetProperty(ref _dataConfigurations, value);
                if (_solution != null)
                    _solution.DataProjectConfiguration = value;
                    
            }
        }

        private string _dbmlPath;
        public string DbmlPath
        {
            get { return _dbmlPath; }
            set { SetProperty(ref _dbmlPath, value); }
        }

        private string _initialDirectory;
        public string InitialDirectory
        {
            get { return _initialDirectory; }
            set { SetProperty(ref _initialDirectory, value); }
        }

        private string _dbConnection;
        public string DbConnection
        {
            get { return _dbConnection; }
            set
            {
                SetProperty(ref _dbConnection, value);
                SqlConnectionStringBuilder connection = new SqlConnectionStringBuilder(DbConnection);
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

        private IReactiveCommand _browseDbCommand;
        public IReactiveCommand BrowseDbCommand
        {
            get { return _browseDbCommand ?? (_browseDbCommand = CreateReactiveCommand(BrowseDb)); }
            set { SetProperty(ref _browseDbCommand, value); }
        }

        private IReactiveCommand _saveCommand;
        public IReactiveCommand SaveCommand
        {
            get { return _saveCommand ?? (_saveCommand = CreateReactiveCommand(Save)); }
            set { SetProperty(ref _saveCommand, value); }
        }

        private bool _isCustomLinqBase;
        public bool IsCustomLinqBase
        {
            get { return _isCustomLinqBase; }
            set
            {
                SetProperty(ref _isCustomLinqBase, value);
                if (value && LinqBaseRepositoryTypes == null)
                    LoadLinqBaseTypes();
            }
        }

        private bool _isCustomLinqReadonlyBase;
        public bool IsCustomLinqReadonlyBase
        {
            get { return _isCustomLinqReadonlyBase; }
            set
            {
                SetProperty(ref _isCustomLinqReadonlyBase, value);
                if (value && LinqBaseRepositoryTypes == null)
                    LoadLinqBaseTypes();
            }
        }

        private List<Type> _linqBaseRepositoryTypes;
        public List<Type> LinqBaseRepositoryTypes
        {
            get { return _linqBaseRepositoryTypes; }
            set { SetProperty(ref _linqBaseRepositoryTypes, value); }
        }

        private Type _selectedLinqBaseRepositoryType;
        public Type SelectedLinqBaseRepositoryType
        {
            get { return _selectedLinqBaseRepositoryType; }
            set { SetProperty(ref _selectedLinqBaseRepositoryType, value); }
        }

        private Type _selectedLinqReadonlyBaseRepositoryType;
        public Type SelectedLinqReadonlyBaseRepositoryType
        {
            get { return _selectedLinqReadonlyBaseRepositoryType; }
            set { SetProperty(ref _selectedLinqReadonlyBaseRepositoryType, value); }
        }
        protected override void OnDesignMode()
        {
            base.OnDesignMode();
            ModelConfigurations = new ModelProjectConfiguration
            {
                AdditionalProperties = new AsyncObservableCollection<ModelAdditionalProperty>
                {
                    new ModelAdditionalProperty
                    {
                        Type = "System.String",
                        Name = "DisplayMember"
                    },
                    new ModelAdditionalProperty
                    {
                        Type = "System.String",
                        Name = "DisplayMember2"
                    }
                }
            };

        }

        private void LoadLinqBaseTypes()
        {
            LinqBaseRepositoryTypes = new List<Type>();
            if (_currentSolutionConfigFile.TryGetValue(ConfigKeys.SqlLinq_Projects_LinQ, out CsProject proj))
            {
                var activeConfig = _solutionHelper.GetActiveConfiguration();
                var assemblyPath = proj.GetAssemblyName();
                if (!string.IsNullOrWhiteSpace(assemblyPath))
                {
                    var path = Path.Combine(Path.GetDirectoryName(proj.ProjectPath), "bin", activeConfig, assemblyPath + ".dll");
                    var assembly = Assembly.LoadFrom(path);
                    foreach (var type in assembly.GetTypes())
                    {
                        if (type.IsGenericType && type.BaseType.GetGenericTypeDefinition() == typeof(CRUDLinqRepositoryBase<,,,>) ||
                            type.IsGenericType && type.BaseType.GetGenericTypeDefinition() == typeof(ReadOnlyLinqRepositoryBase<,,,>) ||
                            type.IsGenericType && type.BaseType.GetGenericTypeDefinition() == typeof(LinqRepositoryBase<,,,>) ||
                            type.IsGenericType && type.BaseType.GetGenericTypeDefinition() == typeof(LinqRepositoryBase<,>) ||
                            type.IsGenericType && type.BaseType.GetGenericTypeDefinition() == typeof(LinqRepositoryBase<>))
                        {
                            LinqBaseRepositoryTypes.Add(type);
                        }
                    }
                }
            }
        }

        protected override async Task OnInitialization()
        {
            await base.OnInitialization();

        }

        public void Save()
        {
            if (IsDbValid && !string.IsNullOrWhiteSpace(DbmlPath))
            {
                _currentSolutionConfigFile.SetValue(ConfigKeys.SqlLinq_DbConnection, DbConnection);
                _currentSolutionConfigFile.SetValue(ConfigKeys.SqlLinq_DbmlPath, DbmlPath);

                _currentSolutionConfigFile.SetValue(ConfigKeys.SqlLinq_Projects_Models, (ModelProjectConfigurationSerializable)ModelConfigurations);
                _currentSolutionConfigFile.SetValue(ConfigKeys.SqlLinq_Projects_Data, (DataProjectConfigurationSerializable)DataConfigurations);

                ConfigurationCompleted?.Invoke(this);
            }
        }

        protected override void SolutionLoaded(IConfigurationFile config)
        {
            base.SolutionLoaded(config);
            InitialDirectory = _solutionHelper.GetCurrentSolutionPath();
            LinqBaseRepositoryTypes = null;

            {
                if (_currentSolutionConfigFile.TryGetValue(ConfigKeys.SqlLinq_DbConnection, out string val))
                    DbConnection = val;
            }

            {
                if (_currentSolutionConfigFile.TryGetValue(ConfigKeys.SqlLinq_DbmlPath, out string val))
                    DbmlPath = val;
            }

            {
                if (_currentSolutionConfigFile.TryGetValue(ConfigKeys.SqlLinq_Projects_Models, out ModelProjectConfigurationSerializable val))
                    ModelConfigurations = (ModelProjectConfiguration)val;
                else
                    ModelConfigurations = new ModelProjectConfiguration();
            }
            {
                if (_currentSolutionConfigFile.TryGetValue(ConfigKeys.SqlLinq_Projects_Data, out DataProjectConfigurationSerializable val))
                    DataConfigurations = (DataProjectConfiguration)val;
                else
                    DataConfigurations = new DataProjectConfiguration();
            }
        }

        public void BrowseDb()
        {
            DbConnection = _sqldbBrowser.GetDatabaseConnectionString();
        }
    }

}