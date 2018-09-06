// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Coddee.CodeTools.Components.Data.Generators;
using Coddee.CodeTools.Sql.Queries;
using Coddee.Collections;
using Coddee.Mvvm;
using Coddee.Services.Dialogs;
using Coddee.WPF;
using Coddee.WPF.Services.Dialogs;

namespace Coddee.CodeTools.Components.Data
{
    public class ImportWizardViewModel : VsViewModelBase<ImportWizardView>
    {
        private ModelCodeGenerator _modelGenerator;
        private RepositoryInterfaceCodeGenerator _repositoryInterfaceGenerator;
        private LinqRepositoryCodeGenerator _linqRepositoryGenerator;
        private RestRepositoryCodeGenerator _restRepositoryGenerator;

        public event ViewModelEventHandler ImportCompleted;

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
            _linqRepositoryGenerator = new LinqRepositoryCodeGenerator();
            _restRepositoryGenerator = new RestRepositoryCodeGenerator();
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
                        new ColumnImportArgumentsViewModel(new SqlColumn
                        {
                            ColumnName = "ID",
                            ColumnType = "uniqueidentifier",
                            IsPrimaryKey = true,
                        }),
                        new ColumnImportArgumentsViewModel(new SqlColumn
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

        public void SetTables(TableImportArguments importOptions, params SqlTableViewModel[] tables)
        {
            ImportArgumentes.ClearAndFill(tables.Select(CreateImportArgs));
            foreach (var importArgument in ImportArgumentes)
            {
                importArgument.ImportModel = importOptions.ImprotModel;
                importArgument.ImportRepository = importOptions.ImprotRepository;
                importArgument.ImportLinqRepository = importOptions.ImprotLinqRepository;
                importArgument.ImportRestRepository = importOptions.ImprotRestRepository;

                if (importArgument.Columns.Count(e => e.IsPrimaryKey) != 1)
                    importArgument.ImportTable = false;
            }
        }

        private TableImportArgumentsViewModel CreateImportArgs(SqlTableViewModel sqlTableViewModel)
        {
            var importArgs = new TableImportArgumentsViewModel(sqlTableViewModel);
            return importArgs;
        }

        private IDialog _dialog;
        public void Show()
        {
            _dialog = _dialogService.ShowDialog("Generate files", this, DialogOptions.StretchedContent, new CloseActionCommand("Close"), new ActionCommand("Generate", Generate));
        }

        private async void Generate()
        {
            await ToggleBusyAsync(Task.Run(() =>
            {
                foreach (var table in ImportArgumentes)
                {
                    if (table.ImportModel)
                    {
                        GenerateFile(_solution.ModelProjectConfiguration, table, _modelGenerator, table.ModelName);
                    }
                    if (table.ImportRepository)
                    {
                        GenerateFile(_solution.DataProjectConfiguration, table, _repositoryInterfaceGenerator, $"I{table.SingularName}Repository");
                    }
                    if (table.ImportLinqRepository)
                    {
                        GenerateFile(_solution.LinqProjectConfiguration, table, _linqRepositoryGenerator, $"{table.SingularName}Repository");
                    }
                    if (table.ImportRestRepository)
                    {
                        GenerateFile(_solution.RestProjectConfiguration, table, _restRepositoryGenerator, $"{table.SingularName}Repository");
                    }
                }
            }));

            ImportCompleted?.Invoke(this);
            ToastSuccess("Files generated successfully");
            _dialog.Close();
        }

        void GenerateFile(ProjectConfiguration project, TableImportArgumentsViewModel table, TypeCodeGenerator generator, string filename)
        {
            var file = !string.IsNullOrEmpty(project.GeneratedCodeFolder) ? Path.Combine(project.ProjectFolder, project.GeneratedCodeFolder, $"{filename}.cs") : Path.Combine(project.ProjectFolder, $"{filename}.cs");

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
}