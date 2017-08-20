// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using System.Windows;
using Coddee.WPF;

namespace Coddee.Services.Dialogs
{
    public interface IDialog:IPresentable
    {
        int ZIndex { get; set; }
        UIElement Container { get; set; }
        event Action<IDialog> CloseRequested;
    }
}
