// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using Coddee.Services.Dialogs;
using Coddee.WPF;
using Coddee.WPF.Services.Dialogs;

namespace Coddee.Services
{
    public interface IDialogService
    {
        event EventHandler<IDialog> DialogStateChanged;


        Task<IDialog> CreateDialog(string title, UIElement content, DialogOptions options, params ActionCommand[] actions);
        Task<IDialog> CreateDialog(string title, UIElement content,  params ActionCommand[] actions);
        Task<IDialog> CreateDialog(string title, IEditorViewModel editor, DialogOptions options);
        Task<IDialog> CreateDialog(string title, IEditorViewModel editor);
        void Initialize(Region dialogRegion);


        IEnumerable<IDialog> GetActiveDialogs();
        IEnumerable<IDialog> GetMinimizedDialogs();
    }
}
