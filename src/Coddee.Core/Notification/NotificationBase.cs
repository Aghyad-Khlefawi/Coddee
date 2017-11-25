// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;

namespace Coddee.Notification
{
    public abstract class NotificationBase:INotification
    {
        protected NotificationBase(string title, string description, DateTime date,int type)
        {
            Title = title;
            Description = description;
            Date = date;
            Type = type;
        }

        public string Title { get; set; }
        public string Description { get; set; }
        public int Type { get; }
        public DateTime Date { get; set; }
        public string Category { get; set; }
        public string Parameter { get; set; }

        public event Action<INotification> Opened;
        public virtual void Open()
        {
            Opened?.Invoke(this);
        }
    }
}
