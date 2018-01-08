﻿// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Coddee.CodeTools.Components.Data.Generators;
using Coddee.CodeTools.Sql.Queries;
using Coddee.Collections;
using Coddee.Data;
using Coddee.Data.LinqToSQL;
using Coddee.WPF;
using Coddee.WPF.Services.Dialogs;

namespace Coddee.CodeTools.Components.Data
{
    public class ImportWizardViewModel : VsViewModelBase<ImportWizardView>
    {
        private ModelCodeGenerator _modelGenerator;
        private RepositoryInterfaceCodeGenerator _repositoryInterfaceGenerator;
        private AsyncObservableCollection<TableImportArgumentsViewModel> _importArgumentes;
        public AsyncObservableCollection<TableImportArgumentsViewModel> ImportArgumentes
        {
            get { return _importArgumentes; }
            set { SetProperty(ref _importArgumentes, value); }
        }

        protected override async Task OnInitialization()
        {
            await base.OnInitialization();
            ImportArgumentes = AsyncObservableCollection<TableImportArgumentsViewModel>.Create();
            _modelGenerator = new ModelCodeGenerator();
            _repositoryInterfaceGenerator = new RepositoryInterfaceCodeGenerator();
        }

        protected override void OnDesignMode()
        {
            base.OnDesignMode();
            ImportArgumentes = new AsyncObservableCollection<TableImportArgumentsViewModel>
            {
                new TableImportArgumentsViewModel(new SqlTableViewModel
                {
                    TableName = "Table1"
                })
                {
                    Columns = new AsyncObservableCollection<ColumnImportArgumentsViewModel>
                    {
                        new ColumnImportArgumentsViewModel(new SqlTableColumn
                        {
                            ColumnName = "ID",
                            ColumnType = "uniqueidentifier",
                            IsPrimaryKey = true,
                        }),
                        new ColumnImportArgumentsViewModel(new SqlTableColumn
                        {
                            ColumnName = "Column1",
                            ColumnType = "nvarchar",
                            IsPrimaryKey = false,
                        })
                    }
                },
                new TableImportArgumentsViewModel(new SqlTableViewModel
                {
                    TableName = "Table2",
                })
            };
        }

        public async Task SetTables(params SqlTableViewModel[] tables)
        {
            var linqBaseTypes = GetLinqBaseType();
            _globalVariables.GetVariable<LinqBaseTypeGlobal>().SetValue(linqBaseTypes);
            ImportArgumentes.ClearAndFill(await tables.Select(async e => await CreateImportArgs(e)));
            foreach (var importArgument in ImportArgumentes)
            {
                if (importArgument.Columns.Count(e => e.IsPrimaryKey) != 1)
                    importArgument.ImportTable = false;
            }
        }

        private async Task<TableImportArgumentsViewModel> CreateImportArgs(SqlTableViewModel sqlTableViewModel)
        {
            var importArgs = new TableImportArgumentsViewModel(sqlTableViewModel);
            await importArgs.Initialize();
            return importArgs;
        }

        public void Show()
        {
            _dialogService.ShowDialog(this, DialogOptions.StretchedContent, new CloseActionCommand("Close"), new ActionCommand("Generate", Generate));
        }

        private List<Type> GetLinqBaseType()
        {
            var res = new List<Type>();
            try
            {
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
                                res.Add(type);
                            }
                        }
                    }
                }
                return res;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private void Generate()
        {
            foreach (var table in ImportArgumentes)
            {
                if (table.ImportModel)
                {
                    var project = _solution.ModelProjectConfiguration;
                    var file = Path.Combine(project.ProjectFolder, project.GeneratedCodeFolder, $"{table.SingularName}.cs");
                    GenerateFile(file, project, table, _modelGenerator);
                }
                if (table.ImportRepository)
                {
                    var project = _solution.DataProjectConfiguration;
                    var file = Path.Combine(project.ProjectFolder, project.GeneratedCodeFolder, $"I{table.SingularName}Repository.cs");
                    GenerateFile(file, project, table, _repositoryInterfaceGenerator);
                }
            }
        }

        void GenerateFile(string file, ProjectConfiguration project, TableImportArgumentsViewModel table, TypeCodeGenerator generator)
        {
            if (!File.Exists(file))
            {
                using (var stream = File.Create(file))
                {
                    generator.Generate(_solution, project, table, stream);
                }
                _solution.AddExistedFileToProject(project.ProjectPath, file);
            }
            else
            {
                var data = generator.Generate(_solution, project, table);
                File.WriteAllBytes(file, data);
            }
        }
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
        }

        public string SingularName { get; set; }

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
        private Type _selectedBaseRepositoryType;
        public Type SelectedBaseRepositoryType
        {
            get { return _selectedBaseRepositoryType; }
            set { SetProperty(ref _selectedBaseRepositoryType, value); }
        }
        protected override async Task OnInitialization()
        {
            await base.OnInitialization();
            Columns = await _table.Columns.Select(CreateColumn).ToAsyncObservableCollection();
            if (_globalVariables.GetVariable<LinqBaseTypeGlobal>().TryGetValue(out IEnumerable<Type> types))
            {
                if (types != null)
                {
                    LinqBaseRepositoryTypes = new List<Type>(types);
                    if (SelectedBaseRepositoryType == typeof(ICRUDRepository<,>))
                    {
                        if (_currentSolutionConfigFile.TryGetValue(ConfigKeys.SqlLinq_LinqCrudBase, out string type))
                        {
                            SelectedLinqBaseRepositoryType = LinqBaseRepositoryTypes.FirstOrDefault(e => e.Name == type);
                            IsCustomLinqBase = true;
                        }
                    }
                    else
                    {
                        if (SelectedBaseRepositoryType == typeof(IReadOnlyRepository<,>))
                        {
                            if (_currentSolutionConfigFile.TryGetValue(ConfigKeys.SqlLinq_LinqReadonlyBase, out string type))
                            {
                                SelectedLinqBaseRepositoryType = LinqBaseRepositoryTypes.FirstOrDefault(e => e.Name == type);
                                IsCustomLinqBase = true;
                            }
                        }
                    }
                }
            }
        }

        private async Task<ColumnImportArgumentsViewModel> CreateColumn(SqlTableColumn arg)
        {
            var argumentsViewModel = new ColumnImportArgumentsViewModel(arg);
            await argumentsViewModel.Initialize();
            return argumentsViewModel;
        }

        public Type GetPrimaryKeyType()
        {
            if (Columns.Count(e => e.IsPrimaryKey) != 1)
                return null;
            return Columns.First(e => e.IsPrimaryKey).Type;
        }
    }

    public class ColumnImportArgumentsViewModel : ViewModelBase
    {
        public ColumnImportArgumentsViewModel(SqlTableColumn column)
        {
            Name = column.ColumnName;
            SqlType = column.ColumnType;
            IsPrimaryKey = column.IsPrimaryKey;
            Type = GetCSharpType(column);
        }

        private Type GetCSharpType(SqlTableColumn column)
        {
            switch (column.ColumnType)
            {
                case "smallint":
                case "tinyint":
                case "int":
                    return typeof(Int32);
                case "nchar":
                case "varchar":
                case "nvarchar":
                    return typeof(string);
                case "bit":
                    return typeof(bool);
                case "datetime":
                case "date":
                case "datetime2":
                    return typeof(DateTime);
                case "char":
                    return typeof(char);
                case "float":
                    return typeof(float);
                case "decimal":
                    return typeof(decimal);
                case "uniqueidentifier":
                    return typeof(Guid);
                case "varbinary":
                    return typeof(byte[]);
            }
            return typeof(object);
        }

        private bool _isPrimaryKey;
        public bool IsPrimaryKey
        {
            get { return _isPrimaryKey; }
            set { SetProperty(ref _isPrimaryKey, value); }
        }
        private string _name;
        public string Name
        {
            get { return _name; }
            set { SetProperty(ref _name, value); }
        }
        private string _sqlType;
        public string SqlType
        {
            get { return _sqlType; }
            set { SetProperty(ref _sqlType, value); }
        }
        private Type _type;
        public Type Type
        {
            get { return _type; }
            set { SetProperty(ref _type, value); }
        }

        private bool _importColumn = true;
        public bool ImportColumn
        {
            get { return _importColumn; }
            set { SetProperty(ref _importColumn, value); }
        }
    }

}