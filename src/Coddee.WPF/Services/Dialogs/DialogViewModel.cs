// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Coddee.Services.Dialogs;

namespace Coddee.WPF.Services.Dialogs
{
    /// <summary>
    /// Base class for dialogs.
    /// </summary>
    /// <typeparam name="TView"></typeparam>
    public class DialogViewModelBase<TView> : ViewModelBase<TView> where TView : UIElement, new()
    {
        /// <summary>
        /// The dialog object.
        /// </summary>
        protected IDialog _dialog;

        /// <summary>
        /// The dialog title.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Show the dialog.
        /// </summary>
        public void Show()
        {
            if (_dialog == null)
                _dialog = _dialogService.CreateDialog(Title, this, GetOptions(), GetCommands()?.ToArray());
            _dialog.Show();
        }

        /// <summary>
        /// Close the dialog.
        /// </summary>
        public void Close()
        {
            _dialog.Close();
        }

        /// <summary>
        /// Get the current dialog options.
        /// </summary>
        /// <returns></returns>
        protected virtual DialogOptions GetOptions()
        {
            return DialogOptions.Default;
        }

        /// <summary>
        /// Get the dialog commands.
        /// </summary>
        /// <returns></returns>
        protected virtual IEnumerable<ActionCommandBase> GetCommands()
        {
            return null;
        }
    }
}
