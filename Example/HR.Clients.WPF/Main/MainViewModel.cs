// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using System.Threading.Tasks;
using System.Windows;
using Coddee.Services.ApplicationSearch;
using Coddee.WPF;

namespace HR.Clients.WPF.Main
{
    public class MainViewModel : ViewModelBase<MainView>
    {
     
        protected override async Task OnInitialization()
        {
            try
            {
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