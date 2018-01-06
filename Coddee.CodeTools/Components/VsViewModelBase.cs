// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System.Threading.Tasks;
using System.Windows;
using Coddee.Services;
using Coddee.WPF;

namespace Coddee.CodeTools.Components
{
    public abstract class VsViewModelBase : ViewModelBase
    {
        protected static IConfigurationFile _currentSolutionConfigFile;
        protected ISolutionHelper _solutionHelper;
        protected override async Task OnInitialization()
        {
            await base.OnInitialization();
            _eventDispatcher.GetEvent<SolutionLoadedEvent>().Subscribe(SolutionLoaded);
            _solutionHelper = Resolve<ISolutionHelper>();
        }

        protected virtual void SolutionLoaded(IConfigurationFile config)
        {
            _currentSolutionConfigFile = config;
        }
    }

    public abstract class VsViewModelBase<TView> : VsViewModelBase, IPresentable<TView> where TView : UIElement, new()
    {
        public TView View => (TView) GetView();

        protected override void RegisterViews()
        {
            base.RegisterViews();
            RegisterViewType<TView>(0);
        }

        protected override void OnViewCreated(UIElement e)
        {
            base.OnViewCreated(e);
            if (e is TView defaultView)
                OnDefaultViewCreated(defaultView);
        }

        protected TView CreateView()
        {
            return (TView) CreateView(0, typeof(TView));
        }

        protected virtual void OnDefaultViewCreated(TView view)
        {
        }

        public TView GetDefaultView()
        {
            return (TView)GetView(0);
        }
    }
}
