using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Coddee.Data;
using Microsoft.AspNet.SignalR;

namespace Coddee.SignalR
{
    public class RepositorySyncHub : Hub
    {
        public Task SyncItem(string identifire, RepositorySyncEventArgs args)
        {
            return Clients.Others.SyncReceived(identifire, args);
        }
    }
}
