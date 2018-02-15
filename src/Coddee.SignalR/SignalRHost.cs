// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using Microsoft.AspNet.SignalR;
using Microsoft.Owin.Cors;
using Microsoft.Owin.Hosting;
using Owin;

namespace Coddee.SignalR
{
    /// <summary>
    /// A self hosting SignalR provider.
    /// </summary>
    /// <typeparam name="THub"></typeparam>
    public class SignalRHost<THub>
    {
        private IDisposable _server;

        /// <summary>
        /// Start the SignalR hub hosting.
        /// </summary>
        public void Start(string servicePort, THub hub)
        {
            var url = $"http://+:{servicePort}/";
            _server = WebApp.Start(url,
                                   app =>
                                   {
                                       GlobalHost.DependencyResolver.Register(typeof(THub), () => hub);
                                       app.UseCors(CorsOptions.AllowAll);
                                       app.MapSignalR(new HubConfiguration
                                       {
                                           EnableDetailedErrors = true
                                       });
                                   });
        }

        /// <summary>
        /// Stop the host.
        /// </summary>
        public void Stop()
        {
            _server.Dispose();
        }
    }
}
