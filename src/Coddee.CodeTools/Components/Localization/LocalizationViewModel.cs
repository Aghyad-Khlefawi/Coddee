// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using Coddee.Collections;
using Coddee.Services;
using Coddee.WPF;
using Coddee.WPF.Commands;
using Microsoft.Win32;

namespace Coddee.CodeTools.Components.Localization
{
    public class LocalizationViewModel : VsViewModelBase<LocalizationView>
    {
        private readonly object _updating = new object();
        private  HashSet<string> _keys;

        public LocalizationViewModel()
        {

        }

        public LocalizationViewModel(ISolutionHelper solutionHelper)
        {
            _solutionHelper = solutionHelper;
        }



        private IReactiveCommand _browseKeysCommand;
        public IReactiveCommand BrowseKeysCommand
        {
            get { return _browseKeysCommand ?? (_browseKeysCommand = CreateReactiveCommand(BrowseKeys)); }
            set { SetProperty(ref _browseKeysCommand, value); }
        }

        private AsyncObservableCollection<LocalizationResourceFile> _localizationResourceFiles;
        public AsyncObservableCollection<LocalizationResourceFile> LocalizationResourceFiles
        {
            get { return _localizationResourceFiles; }
            set { SetProperty(ref _localizationResourceFiles, value); }
        }


        private string _keysFile;
        public string KeysFile
        {
            get { return _keysFile; }
            set
            {
                SetProperty(ref _keysFile, value);
                ValidateKeysFile(value);
                KeysFileName = Path.GetFileName(value);
            }
        }

        private string _keysFileName;
        public string KeysFileName
        {
            get { return _keysFileName; }
            set { SetProperty(ref _keysFileName, value); }
        }

        private bool _isKeysValueValid;
        public bool IsKeysValueValid
        {
            get { return _isKeysValueValid; }
            set { SetProperty(ref _isKeysValueValid, value); }
        }

        private IReactiveCommand _addFileCommand;
        public IReactiveCommand AddFileCommand
        {
            get { return _addFileCommand ?? (_addFileCommand = CreateReactiveCommand(AddFile)); }
            set { SetProperty(ref _addFileCommand, value); }
        }

        private bool _isWatching;
        public bool IsWatching
        {
            get { return _isWatching; }
            set { SetProperty(ref _isWatching, value); }
        }

        public async void AddFile()
        {
            LocalizationResourceFile file = await CreateLocalizationResourceFile();
            LocalizationResourceFiles.Add(file);
        }



        private bool File_FileLocationSet(LocalizationResourceFile file)
        {
            if (LocalizationResourceFiles.Count(e => e.ResxFile == file.ResxFile) > 1)
            {
                return false;
            }

            if (_loading)
                return true;

            _config.SetValue(ConfigKeys.LocalizationWatcher_ResxFiles, LocalizationResourceFiles.Select(e => new LocalizationResourceFileConfig
            {
                FileLocation = e.ResxFile
            }).ToList());
            Watch();
            return true;
        }

        private void ValidateKeysFile(string value)
        {
            var file = new FileInfo(value);
            IsKeysValueValid = file.Exists && file.Extension.ToLower() == ".cs";

            if (IsKeysValueValid)
            {
                if (!_loading)
                {
                    _config.SetValue(ConfigKeys.LocalizationWatcher_KeysFile, value);
                }
            }

            Watch();
        }

        private List<FileSystemWatcher> _filesWatchers;
        private void Watch()
        {
            if (IsKeysValueValid && LocalizationResourceFiles.All(e => e.IsResxFileValid))
            {
                if (_filesWatchers.Any())
                {
                    foreach (var filesWatcher in _filesWatchers)
                    {
                        filesWatcher.Dispose();
                    }
                    _filesWatchers.Clear();
                }

                foreach (var file in LocalizationResourceFiles)
                {
                    var fw = new FileSystemWatcher
                    {
                        Path = file.Directory,
                        Filter = $"{file.ResxFileName}*",//,
                        EnableRaisingEvents = true,
                        NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite
                                       | NotifyFilters.FileName | NotifyFilters.DirectoryName
                    };
                    fw.Changed += FileChanged;
                    _filesWatchers.Add(fw);
                }

                IsWatching = true;
            }
        }


        private void FileChanged(object sender, FileSystemEventArgs e)
        {
            lock (_updating)
            {
                _keys.Clear();
                Thread.Sleep(300);
                foreach (var localizationResourceFile in LocalizationResourceFiles)
                {
                    var xmlDoc = new XmlDocument();

                    using (var sr = new StreamReader(localizationResourceFile.ResxFile))
                    {
                        xmlDoc.Load(sr);
                        foreach (XmlNode node in xmlDoc.GetElementsByTagName("data"))
                        {
                            var key = node.Attributes["name"].Value;
                            if (!_keys.Contains(key))
                                _keys.Add(key);
                        }
                    }
                }

                CodeNamespace globalNamespace = new CodeNamespace();
                var compileUnit = new CodeCompileUnit();
                var tagetClass = new CodeTypeDeclaration("LocalizationKeys")
                {
                    Attributes = MemberAttributes.Public,
                    IsClass = true
                };

                foreach (var key in _keys)
                {
                    //var prop = new CodeSnippetTypeMember { Text = $"\tpublic static string {key} = nameof({key});\n" };
                    var stat = new CodeMemberField(typeof(string),key);
                    stat.Attributes = MemberAttributes.Static | MemberAttributes.Public;
                    stat.InitExpression = new CodeSnippetExpression($"nameof({key})");
                    tagetClass.Members.Add(stat);
                }
                globalNamespace.Types.Add(tagetClass);
                compileUnit.Namespaces.Add(globalNamespace);
                CodeDomProvider provider = CodeDomProvider.CreateProvider("CSharp");
                CodeGeneratorOptions options = new CodeGeneratorOptions();
                options.BlankLinesBetweenMembers = false;
                options.BracingStyle = "C";
                using (var sourceWriter = new StringWriter(new StringBuilder()))
                {
                    provider.GenerateCodeFromCompileUnit(compileUnit, sourceWriter, options);
                    sourceWriter.Flush();
                    File.WriteAllText(KeysFile, sourceWriter.ToString());
                }
            }
        }

        protected override async Task OnInitialization()
        {
            await base.OnInitialization();
            LocalizationResourceFiles = AsyncObservableCollection<LocalizationResourceFile>.Create();
            _filesWatchers = new List<FileSystemWatcher>();
            _keys = new HashSet<string>();
        }

        private bool _loading;
        private IConfigurationFile _config;

        protected override void SolutionLoaded(IConfigurationFile config)
        {
            _loading = true;
            _config = config;

            if (config.TryGetValue(ConfigKeys.LocalizationWatcher_KeysFile, out string keysFile))
                KeysFile = keysFile;
            if (config.TryGetValue(ConfigKeys.LocalizationWatcher_ResxFiles, out List<LocalizationResourceFileConfig> resourceFiles))
                LocalizationResourceFiles.ClearAndFill(resourceFiles.Select(CreateLocalizationResourceFile));
            Watch();
            _loading = false;
        }

        private LocalizationResourceFile CreateLocalizationResourceFile(LocalizationResourceFileConfig arg)
        {
            var file = CreateViewModel<LocalizationResourceFile>();
            file.FileLocationSet += File_FileLocationSet;
            file.ResxFile = arg.FileLocation;
            return file;
        }
        private async Task<LocalizationResourceFile> CreateLocalizationResourceFile()
        {
            var file = await InitializeViewModel<LocalizationResourceFile>();
            file.FileLocationSet += File_FileLocationSet;
            return file;
        }
        public void BrowseKeys()
        {
            var dialog = new OpenFileDialog();
            dialog.Filter = "CSharp file|*.cs";
            dialog.InitialDirectory = _solutionHelper.GetCurrentSolutionPath();
            if (dialog.ShowDialog().GetValueOrDefault())
            {
                KeysFile = dialog.FileName;
            }
        }


    }

    public class LocalizationResourceFileConfig
    {
        public string FileLocation { get; set; }
        public string Culture { get; set; }
    }

    public class LocalizationResourceFile : ViewModelBase
    {
        private readonly ISolutionHelper _solutionHelper;

        public LocalizationResourceFile(ISolutionHelper solutionHelper)
        {
            _solutionHelper = solutionHelper;
        }

        public event Func<LocalizationResourceFile, bool> FileLocationSet;

        private string _resxFile;
        public string ResxFile
        {
            get { return _resxFile; }
            set
            {
                SetProperty(ref _resxFile, value);
                ValidateResxFile(value);
                ResxFileName = Path.GetFileName(value);
            }
        }

        private string _resxFileName;
        public string ResxFileName
        {
            get { return _resxFileName; }
            set { SetProperty(ref _resxFileName, value); }
        }

        private bool _isResxFileValid;
        public bool IsResxFileValid
        {
            get { return _isResxFileValid; }
            set { SetProperty(ref _isResxFileValid, value); }
        }

        private IReactiveCommand _browseResxCommand;
        public IReactiveCommand BrowseResxCommand
        {
            get { return _browseResxCommand ?? (_browseResxCommand = CreateReactiveCommand(BrowseResx)); }
            set { SetProperty(ref _browseResxCommand, value); }
        }
        public string Directory { get; set; }
        public void BrowseResx()
        {
            var dialog = new OpenFileDialog();
            dialog.Filter = "Resx file|*.Resx";
            dialog.InitialDirectory = _solutionHelper.GetCurrentSolutionPath();
            if (dialog.ShowDialog().GetValueOrDefault())
            {
                ResxFile = dialog.FileName;
            }
        }

        private void ValidateResxFile(string value)
        {
            var file = new FileInfo(value);
            IsResxFileValid = file.Exists && file.Extension.ToLower() == ".resx" && (FileLocationSet == null || FileLocationSet.Invoke(this));
            if (IsResxFileValid)
                Directory = Path.GetDirectoryName(value);
        }
    }
}