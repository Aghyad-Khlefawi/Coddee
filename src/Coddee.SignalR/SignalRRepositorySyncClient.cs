// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using Coddee.Data;
using Microsoft.AspNet.SignalR.Client;

namespace Coddee.SignalR
{
    /// <summary>
    /// Sync service basic client implementation.
    /// </summary>
    public class SignalRRepositorySyncClient : SignalRClient, IRepositorySyncService
    {
        /// <summary>
        /// This event is called when ever a sync request is received.
        /// </summary>
        public event Action<string, RepositorySyncEventArgs> SyncReceived = delegate { };

        /// <inheritdoc />
        protected override void SubscribeToEvents()
        {
            base.SubscribeToEvents();
            _proxy.On(nameof(IRepositorySyncService.SyncReceived),
                      (string id, RepositorySyncEventArgs arg) => { SyncReceived(id, arg); });
        }


        /// <inheritdoc />
        public void SyncItem(string identifire, RepositorySyncEventArgs args)
        {
            if (_connection.State == ConnectionState.Connected)
                _proxy.Invoke(nameof(IRepositorySyncService.SyncItem), identifire, args);
        }
    }
}
