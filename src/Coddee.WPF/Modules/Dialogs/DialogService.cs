// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace Coddee.WPF.Modules.Dialogs
{
    public class DialogService : ViewModelBase<DialogServiceView>, IDialogService
    {
        public DialogService()
        {
            _dialogs = new Dictionary<IDialog, UIElement>();
        }

        private Region _dialogsRegion;
        private Dictionary<IDialog, UIElement> _dialogs;
        private SolidColorBrush _dialogBorderBrush;

        public void Initialize(Region dialogsRegion, SolidColorBrush dialogBorderBrush)
        {
            _dialogsRegion = dialogsRegion;
            _dialogBorderBrush = dialogBorderBrush;
            _dialogsRegion.View(this);
        }

        public IDialog ShowMessage(string message)
        {
            var dialog = CreateDialog<MessageDialog>();
            dialog.Message = message;
            return ShowDialog(dialog);
        }

        public IDialog ShowConfirmation(string message, Action OnYes, Action OnNo = null)
        {
            var dialog = CreateDialog<ConfirmationDialogViewModel>();
            dialog.Message = message;
            dialog.OnYes += () =>
            {
                OnYes?.Invoke();
                CloseDialog(dialog);
            };
            dialog.OnNo += () =>
            {
                OnNo?.Invoke();
                CloseDialog(dialog);
            };

            return ShowDialog(dialog);
        }

        public IDialog ShowEditorDialog(UIElement content, Func<Task<bool>> OnSave, Action OnCancel = null)
        {
            var dialog = CreateDialog<EditorDialogViewModel>();
            dialog.Content = content;
            dialog.OnSave += async () =>
            {
                if (await OnSave?.Invoke())
                    CloseDialog(dialog);
            };
            dialog.OnCancel += () =>
            {
                OnCancel?.Invoke();
                CloseDialog(dialog);
            };
            return ShowDialog(dialog);
        }

        public IDialog ShowEditorDialog(IEditorViewModel editor)
        {
            return ShowEditorDialog(editor.GetView(), editor.Save, editor.Cancel);
        }


        public IDialog ShowDialog(IDialog dialog)
        {
            dialog.ZIndex = _dialogs.Count;
            _dialogs[dialog] = dialog.Container;
            _view.DialogsContainer.Children.Add(dialog.Container);
            return dialog;
        }


        public TType CreateDialog<TType>() where TType : IDialog
        {
            var dialog = Resolve<TType>();
            dialog.CloseRequested += CloseDialog;
            var container = new DialogContainer
            {
                DialogBorder = {Background = _dialogBorderBrush},
                Presenter = {Content = dialog.GetView()}
            };
            dialog.Container = container;
            return dialog;
        }

        public void CloseDialog(IDialog dialog)
        {
            _view.DialogsContainer.Children.Remove(_dialogs[dialog]);
            _dialogs.Remove(dialog);
        }
    }
}