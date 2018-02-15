// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System.Threading.Tasks;
using Coddee.Data;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;

namespace Coddee.SignalR
{
    /// <summary>
    /// A simple SignalR hub that provides synchronization between the repositories.
    /// </summary>
    [HubName("repositorySyncHub")]
    public class RepositorySyncHub : Hub
    {
        /// <summary>
        /// Send a sync request to the hub clients.
        /// </summary>
        public virtual Task SyncItem(string identifire, RepositorySyncEventArgs args)
        {
            return Clients.Others.SyncReceived(identifire, args);
        }
    }
}
