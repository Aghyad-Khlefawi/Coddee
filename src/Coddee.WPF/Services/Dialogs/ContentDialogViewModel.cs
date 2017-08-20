// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System.Windows;
using Coddee.WPF.Dialogs;

namespace Coddee.Services.Dialogs
{
    public class ContentDialogViewModel : DialogViewModelBase<ContentDialogView>
    {
        private UIElement _content;
        public UIElement Content
        {
            get { return _content; }
            set { SetProperty(ref this._content, value); }
        }
    }
}