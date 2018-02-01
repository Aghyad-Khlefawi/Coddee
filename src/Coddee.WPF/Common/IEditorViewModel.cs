// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using System.Threading.Tasks;
using System.Windows;

namespace Coddee.WPF
{

    
    public interface IEditorViewModel : IViewModel,IPresentable
    {
        string Title { get; set; }
        string FullTitle { get; }

        void Add();
        void Cancel();
        Task Save();
        void Show();
    }

    public interface IEditorViewModel<TModel> : IEditorViewModel where TModel : new()
    {
        TModel EditedItem { get; set; }

        event EventHandler<EditorSaveArgs<TModel>> Canceled;
        event EventHandler<EditorSaveArgs<TModel>> Saved;

        void Edit(TModel item);
    }

    public interface IEditorViewModel<TView,TModel> : IEditorViewModel<TModel>,IPresentable<TView> where TModel : new() 
        where TView : UIElement
    {
        
    }
}