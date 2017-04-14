// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using System.Windows.Input;
using Coddee.WPF.Commands;
using Coddee.WPF.Dialogs;

namespace Coddee.WPF.Modules.Dialogs
{
    public class ConfirmationDialogViewModel : DialogViewModelBase<ConfirmationDialogView>
    {
        public event Action OnYes;
        public event Action OnNo;

        public ICommand YesCommand => new RelayCommand(Yes);
        public ICommand NoCommand => new RelayCommand(No);

        private string _message;
        public string Message
        {
            get { return _message; }
            set { SetProperty(ref this._message, value); }
        }

        private void No()
        {
            OnNo?.Invoke();
        }

        private void Yes()
        {
            OnYes?.Invoke();
        }
    }
}