// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Windows.Input;
using Coddee.Collections;
using Coddee.Services;
using Coddee.WPF;
using Coddee.WPF.Commands;
using Microsoft.Win32;

namespace Coddee.CodeTools.Components.Localization
{
    public class LocalizationViewModel : ViewModelBase<LocalizationView>
    {
        private readonly ISolutionHelper _solutionHelper;

        public LocalizationViewModel()
        {

        }

        public LocalizationViewModel(ISolutionEventsHelper solutionEventsHelper, ISolutionHelper solutionHelper)
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


        private void Watch()
        {
            if (IsKeysValueValid)
            {

            }
        }

        protected override async Task OnInitialization()
        {
            await base.OnInitialization();
            LocalizationResourceFiles = AsyncObservableCollection<LocalizationResourceFile>.Create();
            _eventDispatcher.GetEvent<SolutionLoadedEvent>().Subscribe(SolutionLoaded);
        }

        private bool _loading;
        private IConfigurationFile _config;

        private void SolutionLoaded(IConfigurationFile config)
        {
            _loading = true;
            _config = config;

            if (config.TryGetValue(ConfigKeys.LocalizationWatcher_KeysFile, out string keysFile))
                KeysFile = keysFile;
            if (config.TryGetValue(ConfigKeys.LocalizationWatcher_ResxFiles, out List<LocalizationResourceFileConfig> resourceFiles))
                LocalizationResourceFiles.ClearAndFill(resourceFiles.Select(CreateLocalizationResourceFile));
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
        }
    }
}