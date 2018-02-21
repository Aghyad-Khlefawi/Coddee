// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using System.Collections.Generic;
using System.Windows;
using Coddee.Mvvm;
using Coddee.Services.Dialogs;
using Coddee.WPF;
using Coddee.WPF.Services.Dialogs;

namespace Coddee.Services
{
    public interface IDialogService
    {
        event EventHandler<IDialog> DialogStateChanged;
        event EventHandler<IDialog> DialogClosed;


        IDialog CreateDialog(string title, object content, DialogOptions options, params ActionCommandBase[] actions);
        IDialog CreateDialog(object content, DialogOptions options, params ActionCommandBase[] actions);
        IDialog CreateDialog(string title, object content, params ActionCommandBase[] actions);
        IDialog CreateDialog(object content, params ActionCommandBase[] actions);
        IDialog CreateDialog(string title, IEditorViewModel editor, DialogOptions options);
        IDialog CreateDialog(IEditorViewModel editor, DialogOptions options);
        IDialog CreateDialog(string title, IEditorViewModel editor);
        IDialog CreateDialog(IEditorViewModel editor);

        IDialog CreateDialog(IPresentable presentable);
        IDialog CreateDialog(string title,IPresentable presentable);
        IDialog CreateDialog(IPresentable presentable, DialogOptions options);
        IDialog CreateDialog(string title,IPresentable presentable, DialogOptions options);
        IDialog CreateDialog(IPresentable presentable, DialogOptions options, params ActionCommandBase[] actions);
        IDialog CreateDialog(string title,IPresentable presentable, DialogOptions options, params ActionCommandBase[] actions);

        IDialog CreateConfirmation(string message, Action yesAction, Action noAction = null);
        void Initialize(Region dialogRegion);


        IEnumerable<IDialog> GetActiveDialogs();
        IEnumerable<IDialog> GetMinimizedDialogs();
    }
}
