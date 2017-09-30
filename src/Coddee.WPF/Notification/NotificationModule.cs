// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System.Threading.Tasks;

namespace Coddee.Notification
{
    [Module(nameof(NotificationModule))]
    public class NotificationModule : IModule
    {
        public Task Initialize(IContainer container)
        {
            container.RegisterInstance<INotificationService, NotificationService>();
            return Task.FromResult(true);
        }
    }
}
