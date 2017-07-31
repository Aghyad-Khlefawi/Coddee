using System;
using Coddee.Data;
using Microsoft.AspNet.SignalR.Client;

namespace Coddee.SignalR
{
    public class SignalRRepositorySyncClient : SignalRClient, IRepositorySyncService
    {
        public event Action<string, RepositorySyncEventArgs> SyncReceived = delegate { };

        protected override void SubscribeToEvents()
        {
            base.SubscribeToEvents();
            _proxy.On(nameof(IRepositorySyncService.SyncReceived),
                      (string id, RepositorySyncEventArgs arg) => { SyncReceived(id, arg); });
        }


        public void SyncItem(string identifire, RepositorySyncEventArgs args)
        {
            if (_connection.State == ConnectionState.Connected)
                _proxy.Invoke(nameof(IRepositorySyncService.SyncItem), identifire, args);
        }
    }
}
