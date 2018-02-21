// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Coddee.Mvvm;
using Coddee.Services;
using Coddee.WPF.Commands;
using Coddee.WPF.DebugTool.Shell;

namespace Coddee.WPF.DebugTool
{
    /// <summary>
    /// <see cref="IDebugTool"/> implementation.
    /// </summary>
    public class DebugToolViewModel : ViewModelBase<DebugToolView>, IDebugTool
    {
        /// <inheritdoc />
        public DebugToolViewModel()
        {

        }

        /// <inheritdoc />
        public DebugToolViewModel(ViewModelExplorerViewModel viewModelExplorer, ShellToolsViewModel shellTools)
        {
            _viewModelExplorer = viewModelExplorer;
            _shellTools = shellTools;
        }

        private ShellToolsViewModel _shellTools;

        /// <summary>
        /// <see cref="ShellToolsViewModel"/>
        /// </summary>
        public ShellToolsViewModel ShellTools
        {
            get { return _shellTools; }
            set { SetProperty(ref this._shellTools, value); }
        }

        private ViewModelExplorerViewModel _viewModelExplorer;
        
        /// <summary>
        /// <see cref="ViewModelExplorerViewModel"/>
        /// </summary>
        public ViewModelExplorerViewModel ViewModelExplorer
        {
            get { return _viewModelExplorer; }
            set { SetProperty(ref this._viewModelExplorer, value); }
        }

        /// <inheritdoc />
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

    /// <summary>
    /// A ViewModel class for navigating ViewModel hierarchy.
    /// </summary>
    public class ViewModelNavigationItem : BindableBase
    {
        /// <inheritdoc />
        public ViewModelNavigationItem(ViewModelInfo viewModelInfo)
        {
            ViewModelInfo = viewModelInfo;
            Name = viewModelInfo.ViewModel.__Name;
            
        }

        /// <summary>
        /// The information object of the ViewModel.
        /// </summary>
        public ViewModelInfo ViewModelInfo { get; set; }

        private string _name;

        /// <summary>
        /// The displayed name .
        /// </summary>
        public string Name
        {
            get { return _name; }
            set { SetProperty(ref this._name, value); }
        }
        
        /// <summary>
        /// Triggered when the navigation to ViewModel is required.
        /// </summary>
        public event EventHandler<ViewModelNavigationItem> OnNavigate;

        /// <summary>
        /// Navigate to this ViewModel.
        /// </summary>
        public ICommand NavigateCommand => new RelayCommand(Navigate);

        private void Navigate()
        {
            OnNavigate?.Invoke(this, this);
        }

    }



}