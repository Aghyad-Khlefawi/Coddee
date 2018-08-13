// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using Coddee.AppBuilder;
using Coddee.Core;
using Coddee.Data;

namespace Coddee.SignalR
{
    public class RepositorySyncClientConfig
    {
        public RepositorySyncClientConfig(string hubUrl)
        {
            HubUrl = hubUrl;
        }

        public string HubUrl { get; set; }
    }
    public static class Extensions
    {
        public static TBuilder UseRepositorySyncClient<TBuilder>(this TBuilder builder, ConfigFunc<RepositorySyncClientConfig> config)
        where TBuilder : IApplicationBuilder
        {
            builder.BuildActionsCoordinator.AddAction(DefaultBuildActions.RepositorySyncClient(container =>
            {
                var configRes = config(container);
                var sync = container.Resolve<RepositorySyncClient>();
                sync.Configure(configRes);
                sync.Connect();
                container.RegisterInstance<IRepositorySyncService>(sync);
                if (container.IsRegistered<IRepositoryManager>())
                {
                    var repoManager = container.Resolve<IRepositoryManager>();
                    repoManager.SetSyncService(sync);
                }
            }));
            return builder;
        }
    }
}
