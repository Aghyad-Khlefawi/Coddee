// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using Coddee.CodeTools.Config;
using Coddee.Collections;
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

        private LinqProjectConfiguration _linqConfigurations;
        public LinqProjectConfiguration LinqConfigurations
        {
            get { return _linqConfigurations; }
            set
            {
                SetProperty(ref _linqConfigurations, value);
                if (_solution != null)
                    _solution.LinqProjectConfiguration = value;

            }
        }
        private RestProjectConfiguration _restConfigurations;
        public RestProjectConfiguration RestConfigurations
        {
            get { return _restConfigurations; }
            set
            {
                SetProperty(ref _restConfigurations, value);
                if (_solution != null)
                    _solution.RestProjectConfiguration = value;
            }
        }


        private string _initialDirectory;
        public string InitialDirectory
        {
            get { return _initialDirectory; }
            set { SetProperty(ref _initialDirectory, value); }
        }


        private DatabaseConfiguration _databaseConfigurations;
        public DatabaseConfiguration DatabaseConfigurations
        {
            get { return _databaseConfigurations; }
            set
            {
                SetProperty(ref _databaseConfigurations, value);
                if (_solution != null)
                    _solution.DatabaseConfigurations = value;
            }
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


        public void Save()
        {

            _currentSolutionConfigFile.SetValue(ConfigKeys.SqlLinq_Dbml, (DatabaseConfigurationSerializable)DatabaseConfigurations);
            _currentSolutionConfigFile.SetValue(ConfigKeys.SqlLinq_Projects_Models, (ModelProjectConfigurationSerializable)ModelConfigurations);
            _currentSolutionConfigFile.SetValue(ConfigKeys.SqlLinq_Projects_Data, (DataProjectConfigurationSerializable)DataConfigurations);
            _currentSolutionConfigFile.SetValue(ConfigKeys.SqlLinq_Projects_LinQ, (LinqProjectConfigurationSerializable)LinqConfigurations);
            _currentSolutionConfigFile.SetValue(ConfigKeys.SqlLinq_Projects_Rest, (RestProjectConfigurationSerializable)RestConfigurations);

            ConfigurationCompleted?.Invoke(this);
            _eventDispatcher.GetEvent<SqlLinqConfigurationUpdatedEvents>().Raise(_solution);
        }

        protected override void SolutionLoaded(IConfigurationFile config)
        {
            base.SolutionLoaded(config);
            InitialDirectory = _solutionHelper.GetCurrentSolutionPath();



            {
                if (_currentSolutionConfigFile.TryGetValue(ConfigKeys.SqlLinq_Dbml, out DatabaseConfigurationSerializable val))
                    DatabaseConfigurations = (DatabaseConfiguration)val;
                else
                    DatabaseConfigurations = new DatabaseConfiguration();
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
            {
                if (_currentSolutionConfigFile.TryGetValue(ConfigKeys.SqlLinq_Projects_LinQ, out LinqProjectConfigurationSerializable val))
                    LinqConfigurations = (LinqProjectConfiguration)val;
                else
                    LinqConfigurations = new LinqProjectConfiguration();
            }
            {
                if (_currentSolutionConfigFile.TryGetValue(ConfigKeys.SqlLinq_Projects_Rest, out RestProjectConfigurationSerializable val))
                    RestConfigurations = (RestProjectConfiguration)val;
                else
                    RestConfigurations = new RestProjectConfiguration();
            }
        }

        public void BrowseDb()
        {
            DatabaseConfigurations.DbConnection = _sqldbBrowser.GetDatabaseConnectionString();
        }
    }

}