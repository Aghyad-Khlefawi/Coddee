// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;

namespace Coddee.Notification
{
    /// <summary>
    /// An application notification
    /// </summary>
    public interface INotification
    {
        /// <summary>
        /// The title of the notification.
        /// </summary>
        string Title { get; set; }

        /// <summary>
        /// The context of the notification
        /// </summary>
        string Description { get; set; }

        /// <summary>
        /// The notification type <see cref="NotificationTypes"/>
        /// </summary>
        int Type { get; }

        /// <summary>
        /// The date the notification was originated at.
        /// </summary>
        DateTime Date { get; set; }

        /// <summary>
        /// The category of the notification.
        /// </summary>
        string Category { get; set; }

        /// <summary>
        /// A parameter to help interacting with the notification.
        /// </summary>
        string Parameter { get; set; }
    }

    /// <summary>
    /// An application notification that can be interacted with
    /// </summary>
    public interface IInteractiveNotification : INotification
    {
        /// <summary>
        /// Triggered when the notification is interacted with.
        /// </summary>
        event Action<INotification> Opened;


        /// <summary>
        /// Called when the notification is interacted with.
        /// </summary>
        void Open();
    }
}
