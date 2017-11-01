using System.Threading.Tasks;
using Coddee.Data;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;

namespace Coddee.SignalR
{
    [HubName("repositorySyncHub")]
    public class RepositorySyncHub : Hub
    {
        public virtual Task SyncItem(string identifire, RepositorySyncEventArgs args)
        {
            return Clients.Others.SyncReceived(identifire, args);
        }
    }
}
