// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using Coddee.Collections;
using Coddee.Mvvm;
using Coddee.WPF;
using Coddee.WPF.Services.Dialogs;

namespace Coddee.Services.Dialogs
{
    public interface IDialog : IPresentable
    {
        event EventHandler Closed;
        event EventHandler<DialogState> StateChanged;
        AsyncObservableCollection<ActionCommandWrapper> Commands { get; }

        int ZIndex { get; }
        string Title { get; set; }
        DialogState State { get; }

        void SetState(DialogState newState);
        void Show();
        void Close();
    }
}
