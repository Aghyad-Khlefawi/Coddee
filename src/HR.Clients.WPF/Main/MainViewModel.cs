// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using System.Threading.Tasks;
using Coddee.Services;
using Coddee.WPF;
using HR.Clients.WPF.Companies;
using HR.Clients.WPF.States;

namespace HR.Clients.WPF.Main
{
    public class MainViewModel : ViewModelBase<MainView>
    {
        public MainViewModel(IConfigurationManager conf)
        {
            if (IsDesignMode())
            {
            }
        }

        private string _text = "Text";
        public string Text
        {
            get { return _text; }
            set { SetProperty(ref this._text, value); }
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

        protected override async Task OnInitialization()
        {
            try
            {
                StatesViewModel = await InitializeViewModel<StatesViewModel>();
                CompaniesViewModel = await InitializeViewModel<CompaniesViewModel>();

                Text = null;
            }
            catch (Exception e)
            {
                LogError(e);
            }
        }

    }
}