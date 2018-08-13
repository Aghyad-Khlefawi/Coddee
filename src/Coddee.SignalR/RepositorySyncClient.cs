// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using System.Threading.Tasks;
using Coddee.Core;
using Coddee.Data;
using Coddee.Loggers;
using Microsoft.AspNetCore.SignalR.Client;

namespace Coddee.SignalR
{
    public class RepositorySyncClient : IRepositorySyncService
    {
        private const string _eventsSource = "RepositorySyncClient";

        private readonly ILogger _logger;
        private RepositorySyncClientConfig _configs;
        private HubConnection _connection;

        public RepositorySyncClient(ILogger logger)
        {
            _logger = logger;
        }

        public async Task Connect()
        {
            try
            {
                if (_configs == null)
                    throw new ConfigurationException("The sync services is not configured, call The configure method before connecting.");

                _logger.Log(_eventsSource, "Connecting to sync hub...");
                _connection = new HubConnectionBuilder()
                              .WithUrl(_configs.HubUrl)
                              .Build();

                _connection.On<string, RepositorySyncEventArgs>(SyncActions.SyncReceived, OnSyncReceived);

                await _connection.StartAsync();
                _logger.Log(_eventsSource, "Connected to sync hub successfully");

            }
            catch (Exception e)
            {
                _logger.Log(_eventsSource, e);
            }
        }

        private void OnSyncReceived(string arg1, RepositorySyncEventArgs arg2)
        {
            SyncReceived?.Invoke(arg1, arg2);
        }

        public event Action<string, RepositorySyncEventArgs> SyncReceived;

        public void SyncItem(string identifire, RepositorySyncEventArgs args)
        {
            _connection.InvokeAsync(SyncActions.SyncItem, identifire, args);
        }

        public void Configure(RepositorySyncClientConfig config)
        {
            _configs = config;
        }
    }
}
