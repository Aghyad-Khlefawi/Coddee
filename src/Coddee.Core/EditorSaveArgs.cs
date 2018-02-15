// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;

namespace Coddee
{
    /// <summary>
    /// Event arguments of a save operation
    /// </summary>
    /// <typeparam name="TModel"></typeparam>
    public class EditorSaveArgs<TModel> : EventArgs
    {
        /// <inheritdoc />
        public EditorSaveArgs(OperationType operationType, TModel item)
        {
            OperationType = operationType;
            Item = item;
        }

        /// <summary>
        /// The executed opperation type
        /// </summary>
        public OperationType OperationType { get; set; }

        /// <summary>
        /// The effected item
        /// </summary>
        public TModel Item { get; set; }
    }
}