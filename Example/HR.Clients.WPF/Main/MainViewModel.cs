// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Coddee.Services;
using Coddee.Services.ApplicationSearch;
using Coddee.WPF;
using HR.Clients.WPF.Modules;

namespace HR.Clients.WPF.Main
{
    public class MainViewModel : ViewModelBase<MainView>
    {
        private readonly IApplicationModulesManager _modules;

        public MainViewModel()
        {
            
        }
        public MainViewModel(IApplicationModulesManager modules)
        {
            _modules = modules;
        }
     
        protected override async Task OnInitialization()
        {
            try
            {
                await _modules.InitializeModules(_modules.RegisterModule(typeof(HRModule)).ToArray());

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