// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;

namespace Coddee.Data
{
    /// <summary>
    /// Contains the information of a <seealso cref="IRepositorySyncService"/> sync call.
    /// </summary>
    public class RepositorySyncEventArgs
    {
        public RepositorySyncEventArgs()
        {
            
        }

        public RepositorySyncEventArgs(OperationType operationType, object item)
        {
            Item = item;
            OperationType = operationType;
        }

        /// <summary>
        /// The items that was added, edited or deleted.
        /// </summary>
        public object Item { get; set; }

        /// <summary>
        /// The operation that occurred on the <see cref="Item"/>.
        /// </summary>
        public OperationType OperationType { get; set; }
    }

    /// <summary>
    /// A sync service contract that should provide the ability to sync 
    /// the repository objects between multiple clients in real-time.
    /// </summary>
    public interface IRepositorySyncService
    {

        /// <summary>
        /// Triggered whenever this client receives a sync call. 
        /// </summary>
        event Action<string, RepositorySyncEventArgs> SyncReceived;

        /// <summary>
        /// Send a sync call to the other clients.
        /// </summary>
        /// <param name="identifire">An object identifier to be able to determine
        /// which repository should handle the sync call.</param>
        /// <param name="args">The sync object information.</param>
        void SyncItem(string identifire, RepositorySyncEventArgs args);
    }

}
