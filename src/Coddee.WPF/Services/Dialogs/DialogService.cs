// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Coddee.WPF;

namespace Coddee.Services.Dialogs
{
    public class DialogService : ViewModelBase<DialogServiceView>, IDialogService
    {
        public DialogService()
        {
            _dialogs = new Dictionary<IDialog, UIElement>();
        }

        private Region _dialogsRegion;
        private readonly Dictionary<IDialog, UIElement> _dialogs;
        private SolidColorBrush _dialogBorderBrush;

        public event EventHandler<IDialog> DialogDisplayed;
        public event EventHandler<IDialog> DialogClosed;

        public void Initialize(Region dialogsRegion, SolidColorBrush dialogBorderBrush)
        {
            _dialogsRegion = dialogsRegion;
            _dialogBorderBrush = dialogBorderBrush;
            _dialogsRegion.View(this);
        }

        public IDialog ShowContent(UIElement content, bool showCloseButton = false)
        {
            var dialog = CreateDialog<ContentDialogViewModel>();
            dialog.ShowCloseButton = showCloseButton;
            dialog.Content = content;
            return ShowDialog(dialog);
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

        public IDialog ShowEditorDialog(UIElement content, Func<Task<bool>> OnSave, Action OnCancel = null, HorizontalAlignment horizontalAlignment = HorizontalAlignment.Center)
        {
            var dialog = CreateDialog<EditorDialogViewModel>(horizontalAlignment);
            dialog.View.Presenter.HorizontalAlignment = horizontalAlignment;
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

        public IDialog ShowEditorDialog(IEditorViewModel editor, HorizontalAlignment horizontalAlignment = HorizontalAlignment.Center)
        {
            return ShowEditorDialog(editor.GetView(), editor.Save, editor.Cancel, horizontalAlignment);
        }


        public IDialog ShowDialog(IDialog dialog)
        {
            dialog.ZIndex = _dialogs.Count;
            _dialogs[dialog] = dialog.Container;
            _view.DialogsContainer.Children.Add(dialog.Container);
            DialogDisplayed?.Invoke(this,dialog);
            return dialog;
        }


        public TType CreateDialog<TType>(HorizontalAlignment horizontalAlignment = HorizontalAlignment.Center) where TType : IDialog
        {
            var container = new DialogContainer
            {
                DialogBorder = {Background = _dialogBorderBrush},
                Presenter = {HorizontalAlignment = horizontalAlignment}
            };
            return CreateDialog<TType>(container,container.Presenter);
        }

        public TType CreateDialog<TType>(UserControl container,ContentPresenter presenter) where TType : IDialog
        {
            var dialog = Resolve<TType>();
            return CreateDialog(dialog, container, presenter);
        }

        public TType CreateDialog<TType>(TType content, UserControl container, ContentPresenter presenter) where TType : IDialog
        {
            content.CloseRequested += CloseDialog;
            presenter.Content = content.GetView();
            content.Container = container;
            return content;
        }

        public void CloseDialog(IDialog dialog)
        {
            _view.DialogsContainer.Children.Remove(_dialogs[dialog]);
            _dialogs.Remove(dialog);
            DialogClosed?.Invoke(this,dialog);
        }
    }
}