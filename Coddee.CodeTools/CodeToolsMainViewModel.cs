// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using System.IO;
using System.Threading.Tasks;
using Coddee.CodeTools.Components;
using Coddee.CodeTools.Components.Localization;
using Coddee.Services;
using Coddee.Services.Configuration;
using Coddee.WPF;

namespace Coddee.CodeTools
{
    public class CodeToolsMainViewModel : ViewModelBase<CodeToolsMainViewControl>
    {
        private readonly ISolutionHelper _solutionHelper;
        private readonly IConfigurationManager _configurationManager;

        public CodeToolsMainViewModel()
        {

        }
        public CodeToolsMainViewModel(ISolutionHelper solutionHelper, ISolutionEventsHelper solutionEventsHelper, IConfigurationManager configurationManager)
        {
            _solutionHelper = solutionHelper;
            _configurationManager = configurationManager;
            solutionEventsHelper.SolutionLoaded += OnSolutionLoaded;
            solutionEventsHelper.SolutionClosed += OnSolutionClosed;
        }

        private LocalizationViewModel _localizationVM;

        public LocalizationViewModel LocalizationVM
        {
            get { return _localizationVM; }
            set { SetProperty(ref _localizationVM, value); }
        }

        private bool _isSolutionLoaded;

        public bool IsSolutionLoaded
        {
            get { return _isSolutionLoaded; }
            set
            {
                SetProperty(ref _isSolutionLoaded, value);
                if (value)
                    LoadSolutionProperties();
            }
        }

        private void LoadSolutionProperties()
        {
            var fileName = $"{_solutionHelper.GetCurrentSolutionName()}.json";
            IConfigurationFile file = new ConfigurationFile(fileName, Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Coddee", "CoddeeTools", fileName));
            _configurationManager.Initialize(file);
            _eventDispatcher.GetEvent<SolutionLoadedEvent>().Raise(file);
        }

        protected override async Task OnInitialization()
        {
            await base.OnInitialization();
            LocalizationVM = await InitializeViewModel<LocalizationViewModel>();
            IsSolutionLoaded = _solutionHelper.IsSolutionLoaded();

        }

        private void OnSolutionLoaded()
        {
            IsSolutionLoaded = true;
        }

        private void OnSolutionClosed()
        {
            IsSolutionLoaded = false;
        }
    }
}
