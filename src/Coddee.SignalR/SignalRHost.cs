using System;
using Microsoft.AspNet.SignalR;
using Microsoft.Owin.Cors;
using Microsoft.Owin.Hosting;
using Owin;

namespace Coddee.SignalR
{
    public class SignalRHost<THub>
    {
        private IDisposable _server;
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

        public void Stop()
        {
            _server.Dispose();
        }
    }
}
