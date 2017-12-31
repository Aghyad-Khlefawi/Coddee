// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System.Windows;

namespace Coddee.WPF.Services.Dialogs
{
    public struct DialogOptions
    {
        public static DialogOptions Default = new DialogOptions
        {
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center,
            ContentHorizontalAlignment = HorizontalAlignment.Center,
            ContentVerticalAlignment = VerticalAlignment.Stretch,
            CanMinimize = false,
            InitialState = DialogState.Active
        };

        public static DialogOptions DefaultMinimizable = new DialogOptions
        {
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center,
            ContentHorizontalAlignment = HorizontalAlignment.Center,
            ContentVerticalAlignment = VerticalAlignment.Stretch,
            CanMinimize = true,
            InitialState = DialogState.Active
        };

        public VerticalAlignment ContentVerticalAlignment { get; set; }
        public HorizontalAlignment ContentHorizontalAlignment { get; set; }
        public VerticalAlignment VerticalAlignment { get; set; }
        public HorizontalAlignment HorizontalAlignment { get; set; }
        public DialogState InitialState { get; set; }
        public bool CanMinimize { get; set; }
    }
}
