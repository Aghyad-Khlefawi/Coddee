// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using Coddee;
using Coddee.Collections;
using Coddee.Services;
using Coddee.Services.ApplicationSearch;
using Coddee.WPF;
using HR.Clients.WPF.Companies;
using HR.Clients.WPF.States;
using HR.Data.Models;
using HR.Data.Repositories;

namespace HR.Clients.WPF.Main
{
    public class MainViewModel : ViewModelBase<MainView>
    {
      
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

      
        protected override async Task OnInitialization()
        {
            try
            {
                StatesViewModel = await InitializeViewModel<StatesViewModel>();
                CompaniesViewModel = await InitializeViewModel<CompaniesViewModel>();
                var shellVM = Resolve<IDefaultShellViewModel>();
                var applicationQuickSearch = Resolve<IApplicationQuickSearch>();
                await applicationQuickSearch.Initialize();
                ExecuteOnUIContext(() =>
                {
                    var searchView = applicationQuickSearch.GetView();
                    if (searchView is FrameworkElement searchViewElem)
                        searchViewElem.HorizontalAlignment = HorizontalAlignment.Right;
                    shellVM.SetToolbarContent((UIElement)searchView);
                });

            }
            catch (Exception e)
            {
                LogError(e);
            }
        }

        
    }
}