// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using Coddee.Services;
using Coddee.Services.ApplicationSearch;
using Coddee.WPF;
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

        private object _isVisible;
        public object IsVisible
        {
            get { return _isVisible; }
            set { SetProperty(ref _isVisible, value); }
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

                var shellVM = Resolve<IDefaultShellViewModel>();
                var applicationSearch = Resolve<IApplicationSearchService>();

                applicationSearch.IndexItems("Companies", new List<SearchItem>
                {
                    new SearchItem(0,"Company1",null,"Company","company1",Geometry.Parse("M7,0C3.1,0,0,3.1,0,7c0,3.8,3.1,7,7,7s7-3.1,7-7C13.9,3.1,10.8,0,7,0z M7,12.5C3.9,12.5,1.4,10,1.4,7c0-3.1,2.5-5.6,5.6-5.6c3.1,0,5.6,2.5,5.6,5.6C12.5,10,10,12.5,7,12.5z M7.3,3.5h-1v4.2l3.6,2.2L10.5,9L7.3,7.1V3.5z"),NavigateTo),
                    new SearchItem(0,"Company2",null,"Company","company2",Geometry.Parse("M7,0C3.1,0,0,3.1,0,7c0,3.8,3.1,7,7,7s7-3.1,7-7C13.9,3.1,10.8,0,7,0z M7,12.5C3.9,12.5,1.4,10,1.4,7c0-3.1,2.5-5.6,5.6-5.6c3.1,0,5.6,2.5,5.6,5.6C12.5,10,10,12.5,7,12.5z M7.3,3.5h-1v4.2l3.6,2.2L10.5,9L7.3,7.1V3.5z"),NavigateTo),
                    new SearchItem(0,"Company3",null,"Company","company3",Geometry.Parse("M7,0C3.1,0,0,3.1,0,7c0,3.8,3.1,7,7,7s7-3.1,7-7C13.9,3.1,10.8,0,7,0z M7,12.5C3.9,12.5,1.4,10,1.4,7c0-3.1,2.5-5.6,5.6-5.6c3.1,0,5.6,2.5,5.6,5.6C12.5,10,10,12.5,7,12.5z M7.3,3.5h-1v4.2l3.6,2.2L10.5,9L7.3,7.1V3.5z"),NavigateTo),
                    new SearchItem(0,"Company4",null,"Company","company4",Geometry.Parse("M7,0C3.1,0,0,3.1,0,7c0,3.8,3.1,7,7,7s7-3.1,7-7C13.9,3.1,10.8,0,7,0z M7,12.5C3.9,12.5,1.4,10,1.4,7c0-3.1,2.5-5.6,5.6-5.6c3.1,0,5.6,2.5,5.6,5.6C12.5,10,10,12.5,7,12.5z M7.3,3.5h-1v4.2l3.6,2.2L10.5,9L7.3,7.1V3.5z"),NavigateTo),
                });

                var applicationQuickSearch = Resolve<IApplicationQuickSearch>();
                await applicationQuickSearch.Initialize();
                ExecuteOnUIContext(() =>
                {
                    var searchView = applicationQuickSearch.GetView();
                    if (searchView is FrameworkElement searchViewElem)
                        searchViewElem.HorizontalAlignment = HorizontalAlignment.Right;
                    shellVM.SetToolbarContent(searchView);
                });
                Text = null;
                _toast.ShowToast("Start",ToastType.Information);
                IsVisible = 2;
              
            }
            catch (Exception e)
            {
                LogError(e);
            }
        }

        private void NavigateTo(object sender, SearchItem e)
        {
            ToastSuccess(e.Title);
        }
    }
}