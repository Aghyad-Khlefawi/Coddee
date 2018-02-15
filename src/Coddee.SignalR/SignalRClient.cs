// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System.Threading.Tasks;
using Microsoft.AspNet.SignalR.Client;

namespace Coddee.SignalR
{
    /// <summary>
    /// A basic implementation of a SignalR hub client.
    /// </summary>
    public class SignalRClient
    {
        /// <summary>
        /// The SignalR proxy object.
        /// </summary>
        protected IHubProxy _proxy;

        /// <summary>
        /// The SignalR connection object.
        /// </summary>
        protected HubConnection _connection;


        /// <summary>
        /// The client ID given by the SignalR hub.
        /// </summary>
        public string ClientID { get; protected set; }


        /// <summary>
        /// Connect to the SignalR hub.
        /// </summary>
        public virtual async Task Connect(string serverIp, string servicePort, string hubname = "repositorySyncHub")
        {
            _connection = new HubConnection($"http://{serverIp}:{servicePort}/");
            _proxy = _connection.CreateHubProxy(hubname);

            SubscribeToEvents();

            await _connection.Start();
        }

        /// <summary>
        /// The is method should be overridden to subscribe to the hub events.
        /// </summary>
        protected virtual void SubscribeToEvents()
        {

        }
    }
}
