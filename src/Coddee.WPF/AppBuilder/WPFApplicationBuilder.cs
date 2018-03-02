// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using System.Linq;
using System.Threading;
using System.Windows;
using Coddee.AppBuilder;
using Coddee.ModuleDefinitions;
using Coddee.Mvvm;

namespace Coddee.WPF
{
    public interface IWPFApplicationBuilder : IApplicationBuilder
    {

    }

    public class WPFApplicationBuilder : WindowsApplicationBuilder, IWPFApplicationBuilder
    {

        private Application _systemApplication => ((WPFApplication)_app).GetSystemApplication();


        public WPFApplicationBuilder(WPFApplication app, IContainer container)
            : base(app, container)
        {
            _systemApplication.Startup += delegate { Start(); };
        }

        public override void Start()
        {
            _systemApplication.Dispatcher.Invoke(() =>
            {
                UISynchronizationContext.SetContext(SynchronizationContext.Current);
            });
            base.Start();
        }


        protected override void SetupDefaultBuildActions()
        {
            base.SetupDefaultBuildActions();

            BuildActionsCoordinator.AddAction(DefaultBuildActions.SetupViewModelBaseBuildAction(container =>
            {
                SetupViewModelBase();
            }));
        }


        protected override Type[] GetDefaultModules()
        {
            return CoreModuleDefinitions.Modules.Concat(WindowsModuleDefinitions.Modules.Concat(WPFModuleDefinitions.Modules)).ToArray();
        }

        protected virtual void SetupViewModelBase()
        {
            Log($"Setting up ViewModelBase.");
            UniversalViewModelBase.SetApplication((WPFApplication)_app);
            UniversalViewModelBase.SetContainer(_container);

            ViewModelBase.SetContainer(_container);
            ViewModelEvent.SetViewModelManager(_container.Resolve<IViewModelsManager>());
        }

        private StartupEventArgs _startupEventArgs;
        internal void SetStartupArgs(StartupEventArgs startupEventArgs)
        {
            _startupEventArgs = startupEventArgs;
        }
    }
}