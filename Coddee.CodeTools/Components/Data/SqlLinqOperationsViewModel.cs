// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using Coddee.Collections;
using Coddee.WPF;
using Coddee.WPF.Commands;

namespace Coddee.CodeTools.Components.Data
{
    public class SqlLinqOperationsViewModel : VsViewModelBase<SqlLinqOperationsView>
    {
        public SqlLinqOperationsViewModel()
        {
            
        }

        public event ViewModelEventHandler ConfigureCliked;

      

        private IReactiveCommand _configureCommand;
        public IReactiveCommand ConfigureCommand
        {
            get { return _configureCommand ?? (_configureCommand = CreateReactiveCommand(Configure)); }
            set { SetProperty(ref _configureCommand, value); }
        }
        public void Configure()
        {
            ConfigureCliked?.Invoke(this);
        }
    }
}