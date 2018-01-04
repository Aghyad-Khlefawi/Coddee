// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System.Threading.Tasks;
using Coddee.Services;
using Coddee.WPF;
using Coddee.WPF.Commands;

namespace Coddee.CodeTools.Components.Data
{
    public class SqlLinqViewModel : VsViewModelBase<SqlLinqView>
    {
        private SqlLinqConfigurationViewModel _configurationViewModel;
        private SqlLinqOperationsViewModel _operationsViewModel;

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
            set { SetProperty(ref _dbConnection, value); }
        }

        private bool _isConfigured;
        public bool IsConfigured
        {
            get { return _isConfigured; }
            set { SetProperty(ref _isConfigured, value); }
        }

        private bool _isConfiguring;
        public bool IsConfiguring
        {
            get { return _isConfiguring; }
            set { SetProperty(ref _isConfiguring, value); }
        }

        private IReactiveCommand _configureCommand;
        public IReactiveCommand ConfigureCommand
        {
            get { return _configureCommand ?? (_configureCommand = CreateReactiveCommand(Configure)); }
            set { SetProperty(ref _configureCommand, value); }
        }

        private IViewModel _contentVm;
        public IViewModel ContentVm
        {
            get { return _contentVm; }
            set { SetProperty(ref _contentVm, value); }
        }

        protected override async Task OnInitialization()
        {
            await base.OnInitialization();
            _operationsViewModel = CreateViewModel<SqlLinqOperationsViewModel>();
            _operationsViewModel.ConfigureCliked += e => Configure();

            _configurationViewModel = CreateViewModel<SqlLinqConfigurationViewModel>();
            _configurationViewModel.ConfigurationCompleted += ConfigurationCompleted;
            await InitializeChildViewModels();
        }

        private void ConfigurationCompleted(IViewModel sender)
        {
            ContentVm = _operationsViewModel;
            ReadConfig();
        }

        public void Configure()
        {
            IsConfiguring = true;
            ContentVm = _configurationViewModel;
        }

        protected override void SolutionLoaded(IConfigurationFile config)
        {
            base.SolutionLoaded(config);
            ReadConfig();
        }

        private void ReadConfig()
        {
            if (_currentSolutionConfigFile.TryGetValue(ConfigKeys.SqlLinq_DbmlPath, out string dbmlPath) &&
                _currentSolutionConfigFile.TryGetValue(ConfigKeys.SqlLinq_DbConnection, out string dbConnection))
            {
                IsConfigured = true;
                DbConnection = dbConnection;
                DbmlPath = dbmlPath;
                ContentVm = _operationsViewModel;
            }
            else
            {
                IsConfigured = false;
            }
        }
    }
}