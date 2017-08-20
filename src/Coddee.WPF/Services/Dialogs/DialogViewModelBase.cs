// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using System.Windows;
using Coddee.Services.Dialogs;

namespace Coddee.WPF.Dialogs
{
    public class DialogViewModelBase<TView> : ViewModelBase<TView>, IDialog where TView : UIElement, new()
    {

        private int _zIndex;
        public int ZIndex
        {
            get { return _zIndex; }
            set { SetProperty(ref this._zIndex, value); }
        }

        public UIElement Container { get; set; }

        public event Action<IDialog> CloseRequested;

        protected void Close()
        {
            CloseRequested?.Invoke(this);
        }
    }
}