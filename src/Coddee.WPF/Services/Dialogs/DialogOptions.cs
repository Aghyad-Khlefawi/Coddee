// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System.Windows;
using Coddee.Services.Dialogs;

namespace Coddee.WPF.Services.Dialogs
{
    /// <summary>
    /// <see cref="IDialog"/> presentation options.
    /// </summary>
    public struct DialogOptions
    {
        /// <summary>
        /// Default dialog options
        /// </summary>
        public static DialogOptions Default = new DialogOptions
        {
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center,
            ContentHorizontalAlignment = HorizontalAlignment.Center,
            ContentVerticalAlignment = VerticalAlignment.Stretch,
            CanMinimize = false,
            InitialState = DialogState.Active
        };

        /// <summary>
        /// A dialog that is stretch horizontally
        /// </summary>
        public static DialogOptions StretchedContent = new DialogOptions
        {
            HorizontalAlignment = HorizontalAlignment.Stretch,
            VerticalAlignment = VerticalAlignment.Center,
            ContentHorizontalAlignment = HorizontalAlignment.Stretch,
            ContentVerticalAlignment = VerticalAlignment.Stretch,
            CanMinimize = false,
            InitialState = DialogState.Active
        };

        /// <summary>
        /// Default dialog options that can be minimized.
        /// </summary>
        public static DialogOptions DefaultMinimizable = new DialogOptions
        {
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center,
            ContentHorizontalAlignment = HorizontalAlignment.Center,
            ContentVerticalAlignment = VerticalAlignment.Stretch,
            CanMinimize = true,
            InitialState = DialogState.Active
        };

        /// <summary>
        /// A dialog that is stretch horizontally that can be minimized.
        /// </summary>
        public static DialogOptions StretchedContentMinimizable = new DialogOptions
        {
            HorizontalAlignment = HorizontalAlignment.Stretch,
            VerticalAlignment = VerticalAlignment.Center,
            ContentHorizontalAlignment = HorizontalAlignment.Stretch,
            ContentVerticalAlignment = VerticalAlignment.Stretch,
            CanMinimize = true,
            InitialState = DialogState.Active
        };

        /// <summary>
        /// The vertical alignment of the dialog content.
        /// </summary>
        public VerticalAlignment ContentVerticalAlignment { get; set; }

        /// <summary>
        /// The horizontal alignment of the dialog content.
        /// </summary>
        public HorizontalAlignment ContentHorizontalAlignment { get; set; }

        /// <summary>
        /// The vertical alignment of the dialog.
        /// </summary>
        public VerticalAlignment VerticalAlignment { get; set; }

        /// <summary>
        /// The horizontal alignment of the dialog.
        /// </summary>
        public HorizontalAlignment HorizontalAlignment { get; set; }

        /// <summary>
        /// The initial state of the dialog.
        /// </summary>
        public DialogState InitialState { get; set; }

        /// <summary>
        /// If true the dialog can be minimized.
        /// </summary>
        public bool CanMinimize { get; set; }
    }
}
