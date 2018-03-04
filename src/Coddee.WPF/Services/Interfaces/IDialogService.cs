// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using System.Collections.Generic;
using Coddee.Mvvm;
using Coddee.Services.Dialogs;
using Coddee.WPF;
using Coddee.WPF.Services.Dialogs;

namespace Coddee.Services
{
    /// <summary>
    /// A service for managing <see cref="IDialog"/> objects.
    /// </summary>
    public interface IDialogService
    {
        /// <summary>
        /// Triggered when a dialog state is changed.
        /// </summary>
        event EventHandler<IDialog> DialogStateChanged;

        /// <summary>
        /// Triggered when a dialog is closed.
        /// </summary>
        event EventHandler<IDialog> DialogClosed;


        /// <summary>
        /// Create a new dialog instance.
        /// </summary>
        /// <param name="title">The title of the dialog.</param>
        /// <param name="content">The content of the dialog.</param>
        /// <param name="options">Dialog options.</param>
        /// <param name="actions">The actions of the dialog.</param>
        IDialog CreateDialog(string title, object content, DialogOptions options, params ActionCommandBase[] actions);

        /// <summary>
        /// Create a new dialog instance.
        /// </summary>
        /// <param name="content">The content of the dialog.</param>
        /// <param name="options">Dialog options.</param>
        /// <param name="actions">The actions of the dialog.</param>
        IDialog CreateDialog(object content, DialogOptions options, params ActionCommandBase[] actions);

        /// <summary>
        /// Create a new dialog instance.
        /// </summary>
        /// <param name="title">The title of the dialog.</param>
        /// <param name="content">The content of the dialog.</param>
        /// <param name="actions">The actions of the dialog.</param>
        IDialog CreateDialog(string title, object content, params ActionCommandBase[] actions);

        /// <summary>
        /// Create a new dialog instance.
        /// </summary>
        /// <param name="content">The content of the dialog.</param>
        /// <param name="actions">The actions of the dialog.</param>
        IDialog CreateDialog(object content, params ActionCommandBase[] actions);

        /// <summary>
        /// Create a new dialog instance.
        /// </summary>
        /// <param name="title">The title of the dialog.</param>
        /// <param name="editor">The content of the dialog.</param>
        /// <param name="options">Dialog options.</param>
        IDialog CreateDialog(string title, IEditorViewModel editor, DialogOptions options);

        /// <summary>
        /// Create a new dialog instance.
        /// </summary>
        /// <param name="editor">The content of the dialog.</param>
        /// <param name="options">Dialog options.</param>
        IDialog CreateDialog(IEditorViewModel editor, DialogOptions options);

        /// <summary>
        /// Create a new dialog instance.
        /// </summary>
        /// <param name="title">The title of the dialog.</param>
        /// <param name="editor">The content of the dialog.</param>
        IDialog CreateDialog(string title, IEditorViewModel editor);

        /// <summary>
        /// Create a new dialog instance.
        /// </summary>
        /// <param name="editor">The content of the dialog.</param>
        IDialog CreateDialog(IEditorViewModel editor);


        /// <summary>
        /// Create a new dialog instance.
        /// </summary>
        /// <param name="presentable">The content of the dialog.</param>
        IDialog CreateDialog(IPresentable presentable);

        /// <summary>
        /// Create a new dialog instance.
        /// </summary>
        /// <param name="title">The title of the dialog.</param>
        /// <param name="presentable">The content of the dialog.</param>
        IDialog CreateDialog(string title,IPresentable presentable);

        /// <summary>
        /// Create a new dialog instance.
        /// </summary>
        /// <param name="presentable">The content of the dialog.</param>
        /// <param name="options">Dialog options.</param>
        IDialog CreateDialog(IPresentable presentable, DialogOptions options);

        /// <summary>
        /// Create a new dialog instance.
        /// </summary>
        /// <param name="title">The title of the dialog.</param>
        /// <param name="presentable">The content of the dialog.</param>
        /// <param name="options">Dialog options.</param>
        IDialog CreateDialog(string title,IPresentable presentable, DialogOptions options);

        /// <summary>
        /// Create a new dialog instance.
        /// </summary>
        /// <param name="presentable">The content of the dialog.</param>
        /// <param name="options">Dialog options.</param>
        /// <param name="actions">The actions of the dialog.</param>
        IDialog CreateDialog(IPresentable presentable, DialogOptions options, params ActionCommandBase[] actions);

        /// <summary>
        /// Create a new dialog instance.
        /// </summary>
        /// <param name="title">The title of the dialog.</param>
        /// <param name="presentable">The content of the dialog.</param>
        /// <param name="options">Dialog options.</param>
        /// <param name="actions">The actions of the dialog.</param>
        IDialog CreateDialog(string title,IPresentable presentable, DialogOptions options, params ActionCommandBase[] actions);

        /// <summary>
        /// Create a confirmation dialog.
        /// </summary>
        /// <param name="message">The dialog message.</param>
        /// <param name="yesAction">An action that will be executed when yes button is pressed</param>
        /// <param name="noAction">An action that will be executed when no button is pressed</param>
        /// <returns></returns>
        IDialog CreateConfirmation(string message, Action yesAction, Action noAction = null);

        /// <summary>
        /// Initialize the service.
        /// </summary>
        /// <param name="dialogRegion"></param>
        void Initialize(Region dialogRegion);

        /// <summary>
        /// Returns the currently active dialogs.
        /// </summary>
        /// <returns></returns>
        IEnumerable<IDialog> GetActiveDialogs();

        /// <summary>
        /// Returns the currently minimized dialogs.
        /// </summary>
        /// <returns></returns>
        IEnumerable<IDialog> GetMinimizedDialogs();
    }
}
