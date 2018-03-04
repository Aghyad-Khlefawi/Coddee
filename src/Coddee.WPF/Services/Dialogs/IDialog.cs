// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using Coddee.Collections;
using Coddee.Mvvm;
using Coddee.WPF.Services.Dialogs;

namespace Coddee.Services.Dialogs
{
    /// <summary>
    /// A UI dialog.
    /// </summary>
    public interface IDialog : IPresentable
    {
        /// <summary>
        /// Triggered when the dialog is closed.
        /// </summary>
        event EventHandler Closed;

        /// <summary>
        /// Triggered when the dialog state is changed.
        /// </summary>
        event EventHandler<DialogState> StateChanged;


        /// <summary>
        /// The commands available in the dialog.
        /// </summary>
        AsyncObservableCollection<ActionCommandWrapper> Commands { get; }

        /// <summary>
        /// The z-index of the dialog.
        /// </summary>
        int ZIndex { get; }

        /// <summary>
        /// the title will be displayed on the dialog tool-bar
        /// </summary>
        string Title { get; set; }

        /// <summary>
        /// The current state of the dialog.
        /// </summary>
        DialogState State { get; }

        /// <summary>
        /// Change the <see cref="State"/> of the dialog.
        /// </summary>
        /// <param name="newState"></param>
        void SetState(DialogState newState);


        /// <summary>
        /// Show the dialog to the user.
        /// </summary>
        void Show();

        /// <summary>
        /// Close the dialog.
        /// </summary>
        void Close();
    }
}
