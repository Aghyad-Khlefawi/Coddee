// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Coddee.Network
{
    /// <summary>
    /// Locates <see cref="NetworkService"/> available on the local network.
    /// </summary>
    public class NetworkServicesLocator
    {
        private static UdpClient _server;
        private static CancellationTokenSource _token;
        private static bool _isBroadcasting;

        /// <summary>
        /// Start broadcasting the access information for <see cref="NetworkService"/>.
        /// </summary>
        public async void BroadcastServices(params NetworkService[] services)
        {
            if (_isBroadcasting)
                StopBroadcastServer();
            _token = new CancellationTokenSource();
            _isBroadcasting = true;

            async Task BroadcastServicesInternal()
            {
                _server = new UdpClient(12100);
                while (_isBroadcasting)
                {
                    try
                    {
                        var clientEP = new IPEndPoint(IPAddress.Any, 0);
                        await _server.ReceiveAsync();
                        var data = JsonConvert.SerializeObject(services);
                        var buffer = Encoding.UTF8.GetBytes(data);
                        await _server.SendAsync(buffer, buffer.Length, clientEP);
                    }
                    catch
                    {
                    }
                }
            }

            await Task.Factory.StartNew(BroadcastServicesInternal, _token.Token, TaskCreationOptions.LongRunning, TaskScheduler.Default);
        }

        /// <summary>
        /// Stop broadcasting the access information for <see cref="NetworkService"/>.
        /// </summary>
        public static void StopBroadcastServer()
        {
            _isBroadcasting = false;
#if NET461

            _server.Close();
#else
            _server.Dispose();
#endif
            _token.Cancel();
        }

        /// <summary>
        /// Triggered when a <see cref="NetworkService"/> is discovered
        /// </summary>
        public event EventHandler<IEnumerable<NetworkService>> OnServicesDiscovered;

        /// <summary>
        /// Triggered when the search for <see cref="NetworkService"/> is completed.
        /// </summary>
        public event EventHandler<IEnumerable<NetworkService>> OnServiceDiscoveryCompleted;

        private UdpClient _client;
        private DateTime _startDate;
        private bool _completed;

        /// <summary>
        /// Start looking for <see cref="NetworkService"/> on the local network
        /// </summary>
        /// <param name="timeoutSeconds"></param>
        public async void StartServicesDiscoveryAsync(int timeoutSeconds)
        {
            var result = new List<NetworkService>();



            Timer timer = null;
            timer = new Timer(TimerElapsed, null, 0, timeoutSeconds * 100);

            _completed = false;

            void TimerElapsed(object state)
            {
                if (!_completed)
                {
                    _completed = true;
                    OnServiceDiscoveryCompleted?.Invoke(this, result);
                }
                timer.Dispose();
            }

            _startDate = DateTime.Now;
            _client = new UdpClient();
            await _client.SendAsync(new byte[] { }, 0, new IPEndPoint(IPAddress.Broadcast, 12100));
            while (DateTime.Now - _startDate < TimeSpan.FromSeconds(timeoutSeconds))
            {
                var res = await _client.ReceiveAsync();
                var resStr = Encoding.UTF8.GetString(res.Buffer);

                result.AddRange(JsonConvert.DeserializeObject<NetworkService[]>(resStr));
                foreach (var item in result)
                {
                    if (item.AccessInfo == null)
                        item.AccessInfo = new NetowkrServiceAccessInfo();

                    item.AccessInfo.IP = res.RemoteEndPoint.Address.ToString();
                }
                OnServicesDiscovered?.Invoke(this, result);
            }
            _completed = true;
            OnServiceDiscoveryCompleted?.Invoke(this, result);
        }
    }
}