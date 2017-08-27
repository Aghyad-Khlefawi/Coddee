// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Windows;
using Coddee.AppBuilder;
using Coddee.ModuleDefinitions;
using Coddee.Services;

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

        protected override void LogCoddeeVersions()
        {
            base.LogCoddeeVersions();
            _logger.Log(_eventsSource,
                        $"using Coddee.WPF: {FileVersionInfo.GetVersionInfo(Assembly.Load("Coddee.WPF").Location).ProductVersion}");
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
            ViewModelBase.SetApplication((WPFApplication)_app);
            ViewModelBase.SetContainer(_container);
        }
    }
}