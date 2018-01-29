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
        string Title { get; set; }
        string Description { get; set; }
        int Type { get; }
        DateTime Date { get; set; }
        string Category { get; set; }
        string Parameter { get; set; }
    }

    /// <summary>
    /// An application notification that can be interacted with
    /// </summary>
    public interface IInteractiveNotification : INotification
    {
        event Action<INotification> Opened;
        void Open();
    }
}
