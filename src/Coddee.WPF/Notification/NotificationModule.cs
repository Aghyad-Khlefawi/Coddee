// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System.Threading.Tasks;

namespace Coddee.Notification
{
    /// <summary>
    /// Registers the <see cref="INotificationService"/>.
    /// </summary>
    [Module(nameof(NotificationModule))]
    public class NotificationModule : IModule
    {
        /// <inheritdoc />
        public Task Initialize(IContainer container)
        {
            container.RegisterInstance<INotificationService, NotificationService>();
            return Task.FromResult(true);
        }
    }
}
