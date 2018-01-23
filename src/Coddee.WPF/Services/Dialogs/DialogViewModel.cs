// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Coddee.Services.Dialogs;

namespace Coddee.WPF.Services.Dialogs
{
    public class DialogViewModelBase<TView> : ViewModelBase<TView> where TView : UIElement, new()
    {
        protected IDialog _dialog;
        public string Title { get; set; }

        public void Show()
        {
            if (_dialog == null)
                _dialog = _dialogService.CreateDialog(Title, this, GetOptions(), GetCommands()?.ToArray());
            _dialog.Show();
        }

        public void Close()
        {
            _dialog.Close();
        }

        protected virtual DialogOptions GetOptions()
        {
            return DialogOptions.Default;
        }

        protected virtual IEnumerable<ActionCommandBase> GetCommands()
        {
            return null;
        }
    }
}
