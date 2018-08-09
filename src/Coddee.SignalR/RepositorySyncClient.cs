// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using System.Threading.Tasks;
using Coddee.Core;
using Coddee.Data;
using Microsoft.AspNetCore.SignalR.Client;

namespace Coddee.SignalR
{
   public class RepositorySyncClient:IRepositorySyncService
   {
       private HubConnection _connection;

       public async Task Connect(string url)
       {
           _connection = new HubConnectionBuilder()
                         .WithUrl(url)
                         .Build();

           _connection.On<string, RepositorySyncEventArgs>(SyncActions.SyncReceived, OnSyncReceived);

           await _connection.StartAsync();
       }

       private void OnSyncReceived(string arg1, RepositorySyncEventArgs arg2)
       {
           SyncReceived?.Invoke(arg1,arg2);
       }

       public event Action<string, RepositorySyncEventArgs> SyncReceived;

       public void SyncItem(string identifire, RepositorySyncEventArgs args)
       {
           _connection.InvokeAsync(SyncActions.SyncItem, identifire, args);
       }
   }
}
