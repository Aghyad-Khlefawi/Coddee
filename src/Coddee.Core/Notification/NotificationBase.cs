// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;

namespace Coddee.Notification
{
    /// <summary>
    /// A default implementation for <see cref="INotification"/>
    /// </summary>
    public abstract class NotificationBase:INotification
    {
        /// <inheritdoc />
        protected NotificationBase(string title, string description, DateTime date,int type)
        {
            Title = title;
            Description = description;
            Date = date;
            Type = type;
        }

        /// <inheritdoc />
        public string Title { get; set; }
        /// <inheritdoc />
        public string Description { get; set; }
        /// <inheritdoc />
        public int Type { get; }
        /// <inheritdoc />
        public DateTime Date { get; set; }
        /// <inheritdoc />
        public string Category { get; set; }
        /// <inheritdoc />
        public string Parameter { get; set; }

        /// <summary>
        /// Triggered when the user interacts with the notification.
        /// </summary>
        public event Action<INotification> Opened;

        /// <summary>
        /// Trigger <see cref="Opened"/> event
        /// </summary>
        public virtual void Open()
        {
            Opened?.Invoke(this);
        }
    }
}
