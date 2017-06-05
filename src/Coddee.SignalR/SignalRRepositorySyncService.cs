using System;
using System.Threading.Tasks;
using Coddee.Loggers;
using Coddee.Network;
using Microsoft.Practices.Unity;

namespace Coddee.SignalR
{
    public class SignalRRepositorySyncService : NetworkService
    {
        private const string _eventsSource = "SyncService";

        private readonly IUnityContainer _container;

        public static Guid ServiceID = Guid.Parse("8d035347-6a5c-4644-8b82-faa5e7b0519d");

        public SignalRRepositorySyncService(IUnityContainer container)
        {
            _container = container;
            ID = ServiceID;
            Name = nameof(SignalRRepositorySyncService);

            if (_container != null && _container.IsRegistered<ILogger>())
                _logger = _container.Resolve<ILogger>();
        }

        private readonly ILogger _logger;

        private SignalRHost<RepositorySyncHub> _host;
        public SignalRRepositorySyncClient Proxy { get; private set; }

        public async Task StartService()
        {
            AccessInfo = new NetowkrServiceAccessInfo
            {
                IP = "localhost",
                Port = "1666",
                Protocol = "http"
            };

            _host = new SignalRHost<RepositorySyncHub>();
            _host.Start(AccessInfo.Port, new RepositorySyncHub());
            _logger?.Log(_eventsSource, "SyncService host started.");
            await ConnectClient();
        }

        public async Task ConnectClient()
        {
            Proxy = new SignalRRepositorySyncClient();
            await Proxy.Connect(AccessInfo.IP, AccessInfo.Port);
            _logger?.Log(_eventsSource, "SyncService client connected.");
        }
    }
}
