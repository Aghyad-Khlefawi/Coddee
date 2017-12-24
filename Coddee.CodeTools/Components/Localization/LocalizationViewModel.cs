// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System.IO;
using System.Threading.Tasks;
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
        public LocalizationViewModel(ISolutionEventsHelper solutionEventsHelper,ISolutionHelper solutionHelper)
        {
            _solutionHelper = solutionHelper;
        }

   
        private IReactiveCommand _browseResxCommand;
        public IReactiveCommand BrowseResxCommand
        {
            get { return _browseResxCommand ?? (_browseResxCommand = CreateReactiveCommand(BrowseResx)); }
            set { SetProperty(ref _browseResxCommand, value); }
        }
        private IReactiveCommand _browseKeysCommand;
        public IReactiveCommand BrowseKeysCommand
        {
            get { return _browseKeysCommand ?? (_browseKeysCommand = CreateReactiveCommand(BrowseKeys)); }
            set { SetProperty(ref _browseKeysCommand, value); }
        }


        private string _resxFile;
        public string ResxFile
        {
            get { return _resxFile; }
            set
            {
                SetProperty(ref _resxFile, value);
                ValidateResxFile(value);
            }
        }

        private string _keysFile;
        public string KeysFile
        {
            get { return _keysFile; }
            set
            {
                SetProperty(ref _keysFile, value);
                ValidateKeysFile(value);
            }
        }
        private bool _isKeysValueValid;
        public bool IsKeysValueValid
        {
            get { return _isKeysValueValid; }
            set { SetProperty(ref _isKeysValueValid, value); }
        }

        private bool _isResxFileValid;
        public bool IsResxFileValid
        {
            get { return _isResxFileValid; }
            set { SetProperty(ref _isResxFileValid, value); }
        }
        private void ValidateKeysFile(string value)
        {
            var file = new FileInfo(value);
            IsKeysValueValid = file.Exists && file.Extension.ToLower() == ".cs";

            if (IsKeysValueValid)
            {
                if (!_loading)
                {
                    _config.SetValue(ConfigKeys.LocalizationWatcher_KeysFile,value);
                }
            }
            Watch();
        }

        private void ValidateResxFile(string value)
        {
            var file = new FileInfo(value);
            IsResxFileValid = file.Exists && file.Extension.ToLower() == ".resx";

            if (IsResxFileValid)
            {
                if (!_loading)
                {
                    _config.SetValue(ConfigKeys.LocalizationWatcher_ResxFile, value);
                }
            }

            Watch();
        }

        private void Watch()
        {
            if (IsKeysValueValid && IsResxFileValid)
            {
                
            }
        }

        protected override async Task OnInitialization()
        {
            await base.OnInitialization();
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
            if (config.TryGetValue(ConfigKeys.LocalizationWatcher_ResxFile, out string resxFile))
                ResxFile = resxFile;
            
            _loading = false;
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
    }
}