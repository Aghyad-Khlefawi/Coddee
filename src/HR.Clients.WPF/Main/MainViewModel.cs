// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Coddee.Collections;
using Coddee.WPF;
using Coddee.WPF.Modules;
using Coddee.WPF.Modules.Console;
using HR.Clients.WPF.Companies;
using HR.Clients.WPF.States;
using HR.Data.Models;

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

        protected override async Task OnInitialization()
        {
            try
            {
                StatesViewModel = await InitializeViewModel<StatesViewModel>();
                CompaniesViewModel = await InitializeViewModel<CompaniesViewModel>();
                var console = Resolve<IApplicationConsole>();
                var treeCommand = new ConsoleCommand
                {
                    Name = "vmtree",
                    Description = "ViewModels tree"
                };
                console.AddCommands(treeCommand);
                console.AddCommandHandler(treeCommand.Name, OnTreeCommand);
                throw new  AccessViolationException();
            }
            catch (Exception e)
            {
                LogError(e);
            }
        }

        private void OnTreeCommand(object sender, ConsoleCommandArgs e)
        {
            var shellvm = Resolve<IShellViewModel>();
            var builder = new StringBuilder('\n');
            AppendChildren(shellvm, builder, 0, 1, 0, true);
            e.Result.Add(builder.ToString());
            e.Handled = true;
        }

        private void AppendChildren(IViewModel vm,
                                    StringBuilder builder,
                                    int level,
                                    int count,
                                    int index,
                                    bool prevCompleted)
        {
            var completed = false;
            if (level > 0)
            {
                for (int i = 0; i < level; i++)
                {
                    if (i > 0)
                    {
                        if (!(prevCompleted && i == level - 1))
                        {
                            builder.Append('│');
                            builder.Append(' ', 5);
                        }
                        else
                        {
                            builder.Append(' ', 8);
                        }
                    }
                    else
                    {
                        builder.Append(' ', 4);
                    }
                }
                if (count - 1 == index)
                {
                    builder.Append('└');
                    completed = true;
                }
                else
                {
                    builder.Append('├');
                }
                builder.Append('─', 3);
            }
            builder.Append(vm.GetType().Name + "\n");
            level++;
            for (int i = 0; i < vm.ChildViewModels.Count; i++)
            {
                var childViewModel = vm.ChildViewModels[i];
                AppendChildren(childViewModel, builder, level, vm.ChildViewModels.Count, i, completed);
            }
        }
    }
}