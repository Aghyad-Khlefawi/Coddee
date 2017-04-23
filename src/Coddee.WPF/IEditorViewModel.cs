// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using System.Collections.Generic;
using System.Windows;

namespace Coddee.WPF
{

    
    public interface IEditorViewModel : IViewModel,IPresentable
    {
        OperationType OperationType { get; set; }
        
        void Add();
        void Cancel();
        void Save();
        IEnumerable<string> Validate();
    }

    public interface IEditorViewModel<TView,TModel> : IEditorViewModel,IPresentable<TView> where TModel : new() 
        where TView : UIElement
    {
        TModel EditedItem { get; set; }

        event EventHandler<EditorSaveArgs<TModel>> Canceled;
        event EventHandler<EditorSaveArgs<TModel>> Saved;

        void Edit(TModel item);
    }
}