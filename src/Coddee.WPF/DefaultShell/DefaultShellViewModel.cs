// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Coddee.WPF.Commands;

namespace Coddee.WPF.DefaultShell
{
    /// <summary>
    /// The default shell viewModel
    /// this viewModel will be used if you don't specify a custom shell at the application build
    /// </summary>
    public class DefaultShellViewModel : ViewModelBase<DefaultShellView>, IDefaultShellViewModel
    {
        /// <summary>
        /// Design time constructor
        /// </summary>
        public DefaultShellViewModel()
        {
            if (IsDesignMode())
            {
                ApplicationName = "HR application";
            }
        }

        #region ToolBar properties

        protected string _username;
        public string Username
        {
            get { return _username; }
            set { SetProperty(ref this._username, value); }
        }

        private string _applicationName;
        public string ApplicationName
        {
            get { return _applicationName; }
            set { SetProperty(ref this._applicationName, value); }
        }

        protected bool _hasError;
        public bool HasError
        {
            get { return _hasError; }
            set { SetProperty(ref this._hasError, value); }
        }

        public ICommand MinimizeCommand => new RelayCommand(Minimize);
        public ICommand ExitCommand => new RelayCommand(Exit);

        #endregion


        private bool _useNavigation;
        public bool UseNavigation
        {
            get { return _useNavigation; }
            set { SetProperty(ref this._useNavigation, value); }
        }

        /// <summary>
        /// Called on exit command
        /// </summary>
        protected virtual void Exit()
        {
            _app.GetSystemApplication().Shutdown();
        }

        /// <summary>
        /// Called on minimize command
        /// </summary>
        protected virtual void Minimize()
        {
            View.WindowState = WindowState.Minimized;
        }

        private IPresentableViewModel _mainViewModel;

        protected override async Task OnInitialization()
        {
            await base.OnInitialization();
            await _mainViewModel.Initialize();
        }
        public IPresentableViewModel SetMainContent(Type defaultPresentable, bool useNavigation)
        {
            _globalVariables.TryGetValue(Globals.Username, out _username);
            _applicationName = _globalVariables.GetValue<string>(Globals.ApplicationName);


            UseNavigation = useNavigation;
            _mainViewModel = (IPresentableViewModel)CreateViewModel(defaultPresentable);
            DefaultRegions.ApplicationMainRegion.View((IPresentable)_mainViewModel);
            return _mainViewModel;
        }

        public IPresentableViewModel GetMainContent()
        {
            return _mainViewModel;
        }
    }
}