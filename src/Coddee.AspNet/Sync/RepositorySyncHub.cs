// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Coddee.Core;
using Coddee.Data;
using Microsoft.AspNetCore.SignalR;

namespace Coddee.AspNetCore.Sync
{
    public class HubUser
    {
        public string ConnectedId { get; set; }
        public string Token { get; set; }
    }

    public class HubAuthorizationProvider
    {
        public HubAuthorizationProvider()
        {
            _authorizedUsers = new ConcurrentDictionary<string, HubUser>();
            _tokens = new ConcurrentDictionary<string, HubUser>();
        }

        private readonly ConcurrentDictionary<string, HubUser> _tokens;
        private readonly ConcurrentDictionary<string, HubUser> _authorizedUsers;

        public string CreateToken()
        {
            var token = Guid.NewGuid().ToString();
            _tokens.TryAdd(token, null);
            return token;
        }
        public HubUser AuthorizeUser(string token, HubCallerContext userContext)
        {
            if (_tokens.ContainsKey(token))
            {
                var hubUser = new HubUser
                {
                    ConnectedId = userContext.ConnectionId,
                    Token = token
                };
                _tokens[userContext.ConnectionId] = hubUser;
                _authorizedUsers.TryAdd(userContext.ConnectionId, hubUser);
                return hubUser;
            }
            throw new ArgumentException("The provided token is invalid");
        }

        public void RemoveUser(HubCallerContext userContext)
        {
            _tokens.TryRemove(userContext.ConnectionId, out HubUser token);
            _authorizedUsers.TryRemove(userContext.ConnectionId, out HubUser user);
        }

        public IReadOnlyList<string> GetAuthorizedUsers()
        {
            return _authorizedUsers.Keys.ToList();
        }
    }

    public class RepositorySyncHub : Hub
    {
        private readonly HubAuthorizationProvider _authorizationProvider;

        public RepositorySyncHub(HubAuthorizationProvider authorizationProvider)
        {
            _authorizationProvider = authorizationProvider;
        }

        /// <summary>
        /// Send a sync request to the hub clients.
        /// </summary>
        public virtual async Task SyncItem(string identifire, RepositorySyncEventArgs args)
        {
            await Clients.Users(_authorizationProvider.GetAuthorizedUsers()).SendCoreAsync(SyncActions.SyncReceived, new object[] { identifire, args });
        }

        public virtual Task Identify(string token)
        {
            _authorizationProvider.AuthorizeUser(token, Context);
            return Task.CompletedTask;
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            _authorizationProvider.RemoveUser(Context);
            return base.OnDisconnectedAsync(exception);
        }
    }
}
