// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using System.Threading.Tasks;

namespace Coddee.Mvvm
{
    /// <summary>
    /// A ViewModel that provide the ability to add and edit a model object.
    /// </summary>
    public interface IEditorViewModel : IViewModel, IPresentable
    {
        /// <summary>
        /// a short title of the editor.
        /// </summary>
        string Title { get; set; }

        /// <summary>
        /// A title for the editor.
        /// </summary>
        string FullTitle { get; }

        /// <summary>
        /// Prepare the editor to add a new object.
        /// </summary>
        void Add();

        /// <summary>
        /// Clear the editor old values
        /// </summary>
        void Clear();

        /// <summary>
        /// Cancel the current operation.
        /// </summary>
        void Cancel();

        /// <summary>
        /// Save the edited object.
        /// </summary>
        Task Save();

        /// <summary>
        /// Show the editor to the user.
        /// </summary>
        void Show();
    }

    /// <summary>
    /// A ViewModel that provide the ability to add and edit a model object.
    /// </summary>
    public interface IEditorViewModel<TModel> : IEditorViewModel where TModel : new()
    {
        /// <summary>
        /// The item currently being edited.
        /// </summary>
        TModel EditedItem { get; set; }

        /// <summary>
        /// Triggered when the operation is canceled.
        /// </summary>
        event EventHandler<EditorSaveArgs<TModel>> Canceled;

        /// <summary>
        /// Triggered when the item is saved.
        /// </summary>
        event EventHandler<EditorSaveArgs<TModel>> Saved;

        /// <summary>
        /// Prepare the editor to edit an object.
        /// </summary>
        /// <param name="item"></param>
        void Edit(TModel item);
    }

    /// <summary>
    /// A ViewModel that provide the ability to add and edit a model object.
    /// </summary>
    public interface IEditorViewModel<TView, TModel> : IEditorViewModel<TModel>, IPresentable<TView> where TModel : new()
                                                                                                     where TView : class
    {

    }

}
