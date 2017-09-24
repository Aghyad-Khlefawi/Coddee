// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Coddee.Collections;
using Coddee.Services;
using Coddee.Services.ViewModelManager;
using Coddee.WPF.Commands;
using Coddee.WPF.DebugTool.Shell;

namespace Coddee.WPF.DebugTool
{
    public class DebugToolViewModel : ViewModelBase<DebugToolView>, IDebugTool
    {

        public DebugToolViewModel()
        {

        }

        public DebugToolViewModel(ViewModelExplorerViewModel viewModelExplorer, ShellToolsViewModel shellTools)
        {
            _viewModelExplorer = viewModelExplorer;
            _shellTools = shellTools;
        }

        private ShellToolsViewModel _shellTools;
        public ShellToolsViewModel ShellTools
        {
            get { return _shellTools; }
            set { SetProperty(ref this._shellTools, value); }
        }

        private ViewModelExplorerViewModel _viewModelExplorer;
        public ViewModelExplorerViewModel ViewModelExplorer
        {
            get { return _viewModelExplorer; }
            set { SetProperty(ref this._viewModelExplorer, value); }
        }

        protected override async Task OnInitialization()
        {
            await base.OnInitialization();
            AttachWindowKeyDown();
            await _viewModelExplorer.Initialize();
            await _shellTools.Initialize();
        }

        void Load()
        {
            _viewModelExplorer.Load();
            _loaded = true;
        }
        #region Window visibility
        /// <summary>
        /// The condition to toggle the tool window
        /// </summary>
        private Func<KeyEventArgs, bool> _toggleCondition;

        private bool _windowVisible;
        private bool _loaded;


        private void AttachWindowKeyDown()
        {
            var shellWindow = (Window)Resolve<IShell>();
            shellWindow.KeyDown += (sender, args) =>
            {
                if (_toggleCondition(args))
                    ToggleWindow();
            };
        }

        private void ToggleWindow()
        {
            _windowVisible = !_windowVisible;
            if (_windowVisible)
            {
                CreateView();
                View.Show();
                if (!_loaded)
                    Load();
            }
            else
                View.Close();
        }

        public void SetToggleCondition(Func<KeyEventArgs, bool> toggleCondition)
        {
            _toggleCondition = toggleCondition;
        }
        #endregion

    }

   

    public class ViewModelNavigationItem : BindableBase
    {
        public ViewModelNavigationItem(ViewModelInfo viewModelInfo)
        {
            ViewModelInfo = viewModelInfo;
            Name = viewModelInfo.ViewModel.__Name;
        }

        public ViewModelInfo ViewModelInfo { get; set; }

        private string _name;
        public string Name
        {
            get { return _name; }
            set { SetProperty(ref this._name, value); }
        }

        public event EventHandler<ViewModelNavigationItem> OnNavigate;
        public ICommand NavigateCommand => new RelayCommand(Navigate);

        private void Navigate()
        {
            OnNavigate?.Invoke(this, this);
        }

    }



}