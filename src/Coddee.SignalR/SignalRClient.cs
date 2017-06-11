using System.Threading.Tasks;
using Microsoft.AspNet.SignalR.Client;

namespace Coddee.SignalR
{
    public class SignalRClient
    {
        protected IHubProxy _proxy;
        protected HubConnection _connection;

        public virtual async Task Connect(string serverIp, string servicePort)
        {
            _connection = new HubConnection($"http://{serverIp}:{servicePort}/");
            _proxy = _connection.CreateHubProxy("repositorySyncHub");

            SubscribeToEvents();

            await _connection.Start();
        }

        protected virtual void SubscribeToEvents()
        {

        }
    }
}
