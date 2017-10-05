// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Coddee.Services.Dialogs;
using Coddee.WPF;

namespace Coddee.Services
{
    public interface IDialogService
    {
        event EventHandler<IDialog> DialogDisplayed;
        event EventHandler<IDialog> DialogClosed;

        void Initialize(Region dialogsRegion, SolidColorBrush dialogBorderBrush);

        IDialog ShowContent(IPresentable content, bool showCloseButton=false, HorizontalAlignment horizontalAlignment = HorizontalAlignment.Center);
        IDialog ShowContent(UIElement content, bool showCloseButton=false, HorizontalAlignment horizontalAlignment = HorizontalAlignment.Center);
        IDialog ShowMessage(string message);
        IDialog ShowConfirmation(string message, Action OnYes, Action OnNo = null);
        IDialog ShowEditorDialog(UIElement content, Func<Task<bool>> OnSave, Action OnCancel = null, HorizontalAlignment horizontalAlignment = HorizontalAlignment.Center);
        IDialog ShowEditorDialog(IEditorViewModel editor, HorizontalAlignment horizontalAlignment = HorizontalAlignment.Center);

        IDialog ShowDialog(IDialog dialog);
        TType CreateDialog<TType>(HorizontalAlignment horizontalAlignment = HorizontalAlignment.Center) where TType : IDialog;
        TType CreateDialog<TType>(UserControl container, ContentPresenter presenter) where TType : IDialog;
        TType CreateDialog<TType>(TType content, UserControl container, ContentPresenter presenter) where TType : IDialog;

        void CloseDialog(IDialog dialog);
    }
}
