// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Coddee.CodeTools.Sql.Queries;
using Coddee.Collections;
using Coddee.Data;
using Coddee.WPF.Commands;

namespace Coddee.CodeTools.Components.Data
{
    public class TableImportArguments
    {
        public bool ImprotModel { get; set; }
        public bool ImprotRepository { get; set; }
        public bool ImprotLinqRepository { get; set; }
        public bool ImprotRestRepository { get; set; }
        public static TableImportArguments All = new TableImportArguments {ImprotRestRepository = true, ImprotLinqRepository = true, ImprotModel = true, ImprotRepository = true};
    }
    public class TableImportArgumentsViewModel : VsViewModelBase
    {
        private readonly SqlTableViewModel _table;

        public TableImportArgumentsViewModel(SqlTableViewModel table)
        {
            _table = table;
            TableName = table.TableName;
            SingularName = table.SingularName;
            BaseRepositoryTypes = new List<Type>
            {
                typeof(ICRUDRepository<,>),
                typeof(IReadOnlyRepository<,>),
                typeof(IIndexedRepository<,>),
                typeof(IRepository),
            };
            SelectedBaseRepositoryType = BaseRepositoryTypes[0];
            Columns = AsyncObservableCollection<ColumnImportArgumentsViewModel>.Create(_table.Columns.Select(CreateColumn));

        }

        public string SingularName { get; set; }
        public string ModelName => $"{SingularName}{_solution.ModelProjectConfiguration.Prefix}";

        private bool _isCustomLinqBase;
        public bool IsCustomLinqBase
        {
            get { return _isCustomLinqBase; }
            set { SetProperty(ref _isCustomLinqBase, value); }
        }

        private AsyncObservableCollection<ColumnImportArgumentsViewModel> _columns;
        public AsyncObservableCollection<ColumnImportArgumentsViewModel> Columns
        {
            get { return _columns; }
            set { SetProperty(ref _columns, value); }
        }

        private bool _importRestRepository = true;
        public bool ImportRestRepository
        {
            get { return _importRestRepository; }
            set { SetProperty(ref _importRestRepository, value); }
        }

        private bool _importLinqRepository = true;
        public bool ImportLinqRepository
        {
            get { return _importLinqRepository; }
            set { SetProperty(ref _importLinqRepository, value); }
        }

        private string _tableName;
        public string TableName
        {
            get { return _tableName; }
            set { SetProperty(ref _tableName, value); }
        }

        private bool _importTable = true;
        public bool ImportTable
        {
            get { return _importTable; }
            set
            {
                SetProperty(ref _importTable, value);
                ImportModel = value;
                ImportRepository = value;
                ImportLinqRepository = value;
                ImportRestRepository = value;
            }
        }

        private bool _importModel = true;
        public bool ImportModel
        {
            get { return _importModel; }
            set { SetProperty(ref _importModel, value); }
        }
        private bool _importRepository = true;
        public bool ImportRepository
        {
            get { return _importRepository; }
            set { SetProperty(ref _importRepository, value); }
        }

        private List<Type> _baseRepositoryTypes;
        public List<Type> BaseRepositoryTypes
        {
            get { return _baseRepositoryTypes; }
            set { SetProperty(ref _baseRepositoryTypes, value); }
        }

        private Type _selectedBaseRepositoryType;
        public Type SelectedBaseRepositoryType
        {
            get { return _selectedBaseRepositoryType; }
            set { SetProperty(ref _selectedBaseRepositoryType, value); }
        }
        private bool _isExpanded;
        public bool IsExpanded
        {
            get { return _isExpanded; }
            set { SetProperty(ref _isExpanded, value); }
        }
        private IReactiveCommand _toggleExpandCommand;
        public IReactiveCommand ToggleExpandCommand
        {
            get { return _toggleExpandCommand ?? (_toggleExpandCommand = CreateReactiveCommand(ToggleExpand)); }
            set { SetProperty(ref _toggleExpandCommand, value); }
        }

        public void ToggleExpand()
        {
            IsExpanded = !IsExpanded;
        }
        private ColumnImportArgumentsViewModel CreateColumn(SqlColumn arg)
        {
            var argumentsViewModel = new ColumnImportArgumentsViewModel(arg);
            return argumentsViewModel;
        }

        public Type GetPrimaryKeyType()
        {
            if (Columns.Count(e => e.IsPrimaryKey) != 1)
                return null;
            return Columns.First(e => e.IsPrimaryKey).Type;
        }
    }
}