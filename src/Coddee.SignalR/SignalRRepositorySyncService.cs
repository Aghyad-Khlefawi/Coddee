// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using System.Threading.Tasks;
using Coddee.Loggers;
using Coddee.Network;


namespace Coddee.SignalR
{
    /// <summary>
    /// A <see cref="NetworkService"/> that provides synchronization between repositories on different clients.
    /// </summary>
    public class SignalRRepositorySyncService : NetworkService
    {
        private const string _eventsSource = "SyncService";

        /// <summary>
        /// A GUID that identifies the service
        /// </summary>
        public static Guid ServiceID = Guid.Parse("8d035347-6a5c-4644-8b82-faa5e7b0519d");

        /// <inheritdoc />
        public SignalRRepositorySyncService(IContainer container)
        {
            ID = ServiceID;
            Name = nameof(SignalRRepositorySyncService);

            if (container != null && container.IsRegistered<ILogger>())
                _logger = container.Resolve<ILogger>();
        }

        private readonly ILogger _logger;

        private SignalRHost<RepositorySyncHub> _host;

        /// <summary>
        /// A proxy object to communicate with the service.
        /// </summary>
        public SignalRRepositorySyncClient Proxy { get; private set; }

        /// <summary>
        /// Start the service host.
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// Connect to the service.
        /// </summary>
        public async Task ConnectClient()
        {
            Proxy = new SignalRRepositorySyncClient();
            await Proxy.Connect(AccessInfo.IP, AccessInfo.Port);
            _logger?.Log(_eventsSource, "SyncService client connected.");
        }
    }
}
