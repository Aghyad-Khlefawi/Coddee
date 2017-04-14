// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using System.Windows;
using System.Windows.Media;

namespace Coddee.WPF.Modules.Dialogs
{
    public interface IDialogService
    {
        void Initialize(Region dialogsRegion, SolidColorBrush dialogBorderBrush);

        IDialog ShowMessage(string message);
        IDialog ShowConfirmation(string message,Action OnYes,Action OnNo = null);
        IDialog ShowEditorDialog(UIElement content, Action OnSave, Action OnCancel = null);

        IDialog ShowDialog(IDialog dialog);
        TType CreateDialog<TType>() where TType : IDialog;

        void CloseDialog(IDialog dialog);
    }
}
