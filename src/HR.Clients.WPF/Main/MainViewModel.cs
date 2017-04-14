// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using System.Threading.Tasks;
using Coddee.WPF;
using Coddee.WPF.Modules.Console;
using HR.Clients.WPF.Companies;
using HR.Clients.WPF.States;

namespace HR.Clients.WPF.Main
{
    public class MainViewModel : ViewModelBase<MainView>
    {
        public MainViewModel()
        {
            if (IsDesignMode())
            {
            }
        }

        private StatesViewModel _statesViewModel;
        public StatesViewModel StatesViewModel
        {
            get { return _statesViewModel; }
            set { SetProperty(ref this._statesViewModel, value); }
        }
        private CompaniesViewModel _companiesViewModel;
        public CompaniesViewModel CompaniesViewModel
        {
            get { return _companiesViewModel; }
            set { SetProperty(ref this._companiesViewModel, value); }
        }

        private string _username;
        public string Username
        {
            get { return _username; }
            set { SetProperty(ref this._username, value); }
        }

        public override async Task Initialize()
        {
            await base.Initialize();
            try
            {
                StatesViewModel = Resolve<StatesViewModel>();
                await StatesViewModel.Initialize();

                CompaniesViewModel = Resolve<CompaniesViewModel>();
                await CompaniesViewModel.Initialize();
            }
            catch (Exception e)
            {
                LogError(e);
            }
        }
    }
}