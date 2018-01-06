// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Coddee.Loggers;
using Coddee.ModuleDefinitions;
using Coddee.Services;
using Coddee.Services.Configuration;
using Coddee.Unity;
using Coddee.Windows.AppBuilder;
using Coddee.WPF;

namespace Coddee.CodeTools
{
    using System.Runtime.InteropServices;
    using Microsoft.VisualStudio.Shell;

    /// <summary>
    /// This class implements the tool window exposed by this package and hosts a user control.
    /// </summary>
    /// <remarks>
    /// In Visual Studio tool windows are composed of a frame (implemented by the shell) and a pane,
    /// usually implemented by the package implementer.
    /// <para>
    /// This class derives from the ToolWindowPane class provided from the MPF in order to use its
    /// implementation of the IVsUIElementPane interface.
    /// </para>
    /// </remarks>
    [Guid("267a7439-6669-459d-962f-fefee1509676")]
    public class CodeToolsMainView : ToolWindowPane
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CodeToolsMainView"/> class.
        /// </summary>
        public CodeToolsMainView() : base(null)
        {
            this.Caption = "Coddee tools";

            // This is the user control hosted by the tool window; Note that, even if this class implements IDisposable,
            // we are not calling Dispose on this object. This is because ToolWindowPane calls Dispose on
            // the object returned by the Content property.


            UISynchronizationContext.SetContext(SynchronizationContext.Current);

            _container = new CoddeeUnityContainer();
            _container.RegisterInstance<ILogger, LogAggregator>();
            BuilderHelper.RegisterLoggers(LoggerTypes.ApplicationConsole, LogRecordTypes.Debug, _container);
            var modules = _container.Resolve<ApplicationModulesManager>();
            _logger = (LogAggregator)_container.Resolve<ILogger>();
            _vsHelper = _container.RegisterInstance<VsHelper, VsHelper>();
            _container.RegisterInstance<ISolutionEventsHelper>(_vsHelper);
            _container.RegisterInstance<ISolutionHelper>(_vsHelper);


            modules.RegisterModule(CoreModuleDefinitions.Modules);
            modules.RegisterModule(WindowsModuleDefinitions.Modules);
            modules.RegisterModule(WPFModuleDefinitions.Modules);
            modules.InitializeAutoModules().Wait();

            //Initialize View model
            Application.Current.Resources.Add("ApplicationAccentColorLighter",new SolidColorBrush((Color)ColorConverter.ConvertFromString("#444F5A")));
            Application.Current.Resources.Add("ApplicationAccentColor", new SolidColorBrush((Color)ColorConverter.ConvertFromString("#283645")));
            Application.Current.Resources.Add("ApplicationAccentColorDarker", new SolidColorBrush((Color)ColorConverter.ConvertFromString("#151824")));
        }

        private LogAggregator _logger;
        private IContainer _container;
        private readonly VsHelper _vsHelper;
        protected override void Initialize()
        {
            base.Initialize();
            _vsHelper.GetService = GetService;
            _vsHelper.Initialize();

            ViewModelBase.SetContainer(_container);
            var vm = _container.Resolve<IViewModelsManager>().CreateViewModel<CodeToolsMainViewModel>(null);

            var view = vm.GetDefaultView();
            var applicationConsole = _container.Resolve<IApplicationConsole>();
            applicationConsole.Initialize(view, _logger.MinimumLevel);
            applicationConsole.SetToggleCondition(e => e.Key == Key.F12);
            _logger.AddLogger(applicationConsole.GetLogger(), LoggerTypes.ApplicationConsole);

            var dialog = _container.Resolve<IDialogService>();
            dialog.Initialize(WPF.DefaultShell.DefaultRegions.DialogRegion);
            vm.Initialize();
            this.Content = view;
        }

    }
}
