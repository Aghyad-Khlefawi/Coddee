// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using System.Windows;
using System.Windows.Input;
using Coddee.WPF.Commands;
using Coddee.WPF.Dialogs;

namespace Coddee.Services.Dialogs
{
    public class EditorDialogViewModel : DialogViewModelBase<EditorDialogView>
    {
        public event Action OnSave;
        public event Action OnCancel;

        public ICommand SaveCommand => new RelayCommand(Save);
        public ICommand CancelCommand => new RelayCommand(Cancel);

        private UIElement _content;
        public UIElement Content
        {
            get { return _content; }
            set { SetProperty(ref this._content, value); }
        }

        private void Cancel()
        {
            OnCancel?.Invoke();
        }

        private void Save()
        {
            OnSave?.Invoke();
        }
    }
}