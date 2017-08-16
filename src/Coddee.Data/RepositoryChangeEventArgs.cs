// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;

namespace Coddee.Data
{
   public class RepositoryChangeEventArgs<T>:EventArgs
    {
        public RepositoryChangeEventArgs(OperationType operationType, T item,bool fromSync=false)
        {
            OperationType = operationType;
            Item = item;
            FromSync = fromSync;
        }

        public bool FromSync { get; set; }
        public OperationType OperationType { get; set; }
        public T Item { get; set; }
    }
}
