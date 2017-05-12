// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;

namespace Coddee
{
    public class EditorSaveArgs<TModel> : EventArgs
    {
        public EditorSaveArgs(OperationType operationType, TModel item)
        {
            OperationType = operationType;
            Item = item;
        }

        public OperationType OperationType { get; set; }
        public TModel Item { get; set; }
    }
}