// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System.Collections.Generic;
using Coddee.CodeTools.Sql.Queries;
using Coddee.WPF;
using Coddee.WPF.Commands;

namespace Coddee.CodeTools.Components.Data
{
    public class SqlTableViewModel : ViewModelBase
    {
        private readonly ImportWizardViewModel _importWizardViewModel;

        public SqlTableViewModel()
        {
            
        }

        public SqlTableViewModel(ImportWizardViewModel importWizardViewModel)
        {
            _importWizardViewModel = importWizardViewModel;
        }
        
        public string TableName { get; set; }
        public string SchemaName { get; set; }
        public List<SqlTableColumn> Columns { get; set; }
        public string SingularName { get; set; }

        private bool _isModelValid;
        public bool IsModelValid
        {
            get { return _isModelValid; }
            set { SetProperty(ref _isModelValid, value); }
        }

        private bool _isRepositoryInterfaceValid;
        public bool IsRepositoryInterfaceValid
        {
            get { return _isRepositoryInterfaceValid; }
            set { SetProperty(ref _isRepositoryInterfaceValid, value); }
        }

        private bool _isLinqRepositoryValid;
        public bool IsLinqRepositoryValid
        {
            get { return _isLinqRepositoryValid; }
            set { SetProperty(ref _isLinqRepositoryValid, value); }
        }

        private bool _isRestRepositoryValid;
        public bool IsRestRepositoryValid
        {
            get { return _isRestRepositoryValid; }
            set { SetProperty(ref _isRestRepositoryValid, value); }
        }

        private IReactiveCommand _configureTableCommand;
        public IReactiveCommand ConfigureTableCommand
        {
            get { return _configureTableCommand ?? (_configureTableCommand = CreateReactiveCommand(ConfigureTable)); }
            set { SetProperty(ref _configureTableCommand, value); }
        }

        public async void ConfigureTable()
        {
            await _importWizardViewModel.SetTables(this);
            _importWizardViewModel.Show();
        }

        public string GetModelFileName()
        {
            return $"{SingularName}.cs";
        }
        public string GetRepsotioryInterfaceFileName()
        {
            return $"I{SingularName}Repository.cs";
        }

        public string GetRepsotioryFileName()
        {
            return $"{SingularName}Repository.cs";
        }
    }
}
