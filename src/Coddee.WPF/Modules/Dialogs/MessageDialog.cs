// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System.Windows.Input;
using Coddee.WPF.Commands;
using Coddee.WPF.Dialogs;

namespace Coddee.WPF.Modules.Dialogs
{
    public class MessageDialog : DialogViewModelBase<MessageDialogView>
    {

        private string _message;
        public string Message
        {
            get { return _message; }
            set { SetProperty(ref this._message, value); }
        }

        public ICommand OkCommand => new RelayCommand(Ok);

        private void Ok()
        {
            Close();
        }

    }
}