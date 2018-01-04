// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System.Threading.Tasks;
using System.Windows;
using Coddee.Services;
using Coddee.WPF;

namespace Coddee.CodeTools.Components
{
    public abstract class VsViewModelBase<TView> : ViewModelBase<TView> where TView : UIElement, new()
    {
        protected IConfigurationFile _currentSolutionConfigFile;

        protected override async Task OnInitialization()
        {
            await base.OnInitialization();
            _eventDispatcher.GetEvent<SolutionLoadedEvent>().Subscribe(SolutionLoaded);
        }

        protected virtual void SolutionLoaded(IConfigurationFile config)
        {
            _currentSolutionConfigFile = config;
        }
    }
}
