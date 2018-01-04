// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System.Data.SqlClient;
using System.Xml;
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
        private readonly ISolutionHelper _solutionHelper;

        public SqlLinqConfigurationViewModel()
        {
            Projects = AsyncObservableCollection<CsProjectConfig>.Create(new[]
            {
                new CsProjectConfig{Title = "Models",Folder = "Models"},
                new CsProjectConfig{Title = "Data",Folder="Repositories"},
                new CsProjectConfig{Title = "LinQ",Folder="Repositories"},
                new CsProjectConfig{Title = "Rest",Folder="Repositories"},
            });
        }

        public SqlLinqConfigurationViewModel(ISQLDBBrowser sqldbBrowser, ISolutionHelper solutionHelper)
            : this()
        {
            _sqldbBrowser = sqldbBrowser;
            _solutionHelper = solutionHelper;
        }

        public event ViewModelEventHandler ConfigurationCompleted;

        private AsyncObservableCollection<CsProjectConfig> _projects;
        public AsyncObservableCollection<CsProjectConfig> Projects
        {
            get { return _projects; }
            set { SetProperty(ref _projects, value); }
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

        public void Save()
        {
            if (IsDbValid && !string.IsNullOrWhiteSpace(DbmlPath))
            {
                _currentSolutionConfigFile.SetValue(ConfigKeys.SqlLinq_DbConnection, DbConnection);
                _currentSolutionConfigFile.SetValue(ConfigKeys.SqlLinq_DbmlPath, DbmlPath);
                foreach (var csProjectConfig in Projects)
                {
                    _currentSolutionConfigFile.SetValue($"ConfigKeys.SqlLinq_Projects_{csProjectConfig.Title}", (CsProject)csProjectConfig);
                }
                ConfigurationCompleted?.Invoke(this);
            }
        }

        protected override void SolutionLoaded(IConfigurationFile config)
        {
            base.SolutionLoaded(config);
            InitialDirectory = _solutionHelper.GetCurrentSolutionPath();

            {
                if (_currentSolutionConfigFile.TryGetValue(ConfigKeys.SqlLinq_DbConnection, out string val))
                    DbConnection = val;
            }
            {
                if (_currentSolutionConfigFile.TryGetValue(ConfigKeys.SqlLinq_DbmlPath, out string val))
                    DbmlPath = val;
            }

            foreach (var csProjectConfig in Projects)
            {
                {
                    if (_currentSolutionConfigFile.TryGetValue($"ConfigKeys.SqlLinq_Projects_{csProjectConfig.Title}", out CsProject val))
                        csProjectConfig.SetValues(val);
                }
            }
        }

        public void BrowseDb()
        {
            DbConnection = _sqldbBrowser.GetDatabaseConnectionString();
        }
    }

    public class CsProjectConfig : BindableBase
    {
        private string _title;
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        private string _projectPath;
        public string ProjectPath
        {
            get { return _projectPath; }
            set
            {
                SetProperty(ref _projectPath, value);
                SetDefaultNameSpace();
            }
        }

        private string _folder;
        public string Folder
        {
            get { return _folder; }
            set { SetProperty(ref _folder, value); }
        }

        private string _defaultNamespace;
        public string DefaultNamespace
        {
            get { return _defaultNamespace; }
            set { SetProperty(ref _defaultNamespace, value); }
        }

        private void SetDefaultNameSpace()
        {
            if (string.IsNullOrWhiteSpace(DefaultNamespace) && !string.IsNullOrWhiteSpace(DefaultNamespace))
            {
                var xml = new XmlDocument();
                xml.Load(ProjectPath);
                foreach (XmlElement propertyGroup in xml.GetElementsByTagName("PropertyGroup"))
                {
                    var rootNamespace = propertyGroup.GetElementsByTagName("RootNamespace");
                    if (rootNamespace.Count > 0)
                    {
                        DefaultNamespace = $"{rootNamespace.Item(0).LastChild.Value}.{Folder}";
                    }
                }
            }
        }

        public void SetValues(CsProject val)
        {
            DefaultNamespace = val.DefaultNamespace;
            Folder = val.Folder;
            ProjectPath = val.ProjectPath;
        }

        public static explicit operator CsProject(CsProjectConfig item)
        {
            return new CsProject
            {
                Title = item.Title,
                ProjectPath = item.ProjectPath,
                DefaultNamespace = item.DefaultNamespace,
                Folder = item.Folder
            };
        }
    }
}