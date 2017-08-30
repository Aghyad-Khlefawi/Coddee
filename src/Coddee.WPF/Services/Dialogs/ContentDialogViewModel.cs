// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System.Windows;
using System.Windows.Input;
using Coddee.WPF.Commands;
using Coddee.WPF.Dialogs;

namespace Coddee.Services.Dialogs
{
    public class ContentDialogViewModel : DialogViewModelBase<ContentDialogView>
    {
        public ContentDialogViewModel()
        {
            CloseCommand = new RelayCommand(Close);
        }

        private UIElement _content;
        public UIElement Content
        {
            get { return _content; }
            set { SetProperty(ref this._content, value); }
        }

        private bool _showCloseButton;
        public bool ShowCloseButton
        {
            get { return _showCloseButton; }
            set { SetProperty(ref this._showCloseButton, value); }
        }

        public ICommand CloseCommand { get; }
    }
}