using System;
using Timer = System.Timers.Timer;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Coddee.Network
{
    public class NetworkServicesLocator
    {
        private static UdpClient _server;
        private static CancellationTokenSource _token;
        private static bool _isBroadcasting;
        public async void BroadcastServices(params NetworkService[] services)
        {
            if (_isBroadcasting)
                StopBroadcastServer();
            _token = new CancellationTokenSource();
            _isBroadcasting = true;
            await Task.Factory.StartNew(() =>
            {
                _server = new UdpClient(12100);
                while (_isBroadcasting)
                {
                    try
                    {
                        var clientEP = new IPEndPoint(IPAddress.Any, 0);
                        _server.Receive(ref clientEP);
                        var data = JsonConvert.SerializeObject(services);
                        var buffer = Encoding.UTF8.GetBytes(data);
                        _server.Send(buffer, buffer.Length, clientEP);
                    }
                    catch
                    {
                    }
                }
            }, _token.Token, TaskCreationOptions.LongRunning, TaskScheduler.Default);
        }
        public static void StopBroadcastServer()
        {
            _isBroadcasting = false;
            _server.Close();
            _token.Cancel();
        }


        public event EventHandler<IEnumerable<NetworkService>> OnServicesDescovered;
        public event EventHandler<IEnumerable<NetworkService>> OnServiceDescoveryCompleted;

        private UdpClient _client;
        private DateTime _startDate;
        private bool _completed;

        public async void StartServicesDiscoveryAsync(int timeoutSeconds)
        {
            var result = new List<NetworkService>();
            var timer = new Timer(timeoutSeconds * 100);
            _completed = false;
            timer.Elapsed += delegate
            {
                if (!_completed)
                {
                    _completed = true;
                    OnServiceDescoveryCompleted?.Invoke(this, result);
                }
                timer.Stop();
            };
            timer.Start();
            _startDate = DateTime.Now;
            _client = new UdpClient();
            await _client.SendAsync(new byte[] { }, 0, new IPEndPoint(IPAddress.Broadcast, 12100));
            while (DateTime.Now - _startDate < TimeSpan.FromSeconds(timeoutSeconds))
            {
                var res = await _client.ReceiveAsync();
                var resStr = Encoding.UTF8.GetString(res.Buffer);
                result.AddRange(JsonConvert.DeserializeObject<NetworkService[]>(resStr));
                OnServicesDescovered?.Invoke(this, result);
            }
            _completed = true;
            OnServiceDescoveryCompleted?.Invoke(this, result);
        }
    }
}