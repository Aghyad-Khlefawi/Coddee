// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;

namespace Coddee.Data
{
    /// <summary>
    /// Event arguments for the <see cref="IRepository{TModel,TKey}.ItemsChanged"/> Event
    /// </summary>
    /// <typeparam name="T">The model object type</typeparam>
    public class RepositoryChangeEventArgs<T>:EventArgs
    {
        /// <inheritdoc />
        public RepositoryChangeEventArgs(OperationType operationType, T item,bool fromSync=false)
        {
            OperationType = operationType;
            Item = item;
            FromSync = fromSync;
        }

        /// <summary>
        /// Was the event triggered by the <see cref="IRepositorySyncService"/>.
        /// If the value is true then the change was caused by another client.
        /// </summary>
        public bool FromSync { get; set; }

        /// <summary>
        /// The type of the operation that occurred on the <see cref="Item"/>
        /// </summary>
        public OperationType OperationType { get; set; }

        /// <summary>
        /// The items that was added, edited or deleted.
        /// </summary>
        public T Item { get; set; }
    }
}
