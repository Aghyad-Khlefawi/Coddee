// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;

namespace Coddee.Notification
{
    /// <summary>
    /// An <see cref="INotification"/> that represent an <see cref="NotificationTypes.Information"/> notification
    /// </summary>
    public class InformationNotification : NotificationBase
    {
        /// <inheritdoc />
        public InformationNotification(string title, string description) :
            this(title, description, DateTime.Now)
        {

        }

        /// <inheritdoc />
        public InformationNotification(string title, string description, DateTime date) :
            base(title, description, date, (int)NotificationTypes.Information)
        {

        }
    }

    /// <summary>
    /// An <see cref="INotification"/> that represent a <see cref="NotificationTypes.Warning"/> notification
    /// </summary>
    public class WarningNotification : NotificationBase
    {
        /// <inheritdoc />
        public WarningNotification(string title, string description) :
            this(title, description, DateTime.Now)
        {

        }

        /// <inheritdoc />
        public WarningNotification(string title, string description, DateTime date) :
            base(title, description, date, (int)NotificationTypes.Warning)
        {

        }
    }

    /// <summary>
    /// An <see cref="INotification"/> that represent an <see cref="NotificationTypes.Error"/> notification
    /// </summary>
    public class ErrorNotification : NotificationBase
    {
        /// <inheritdoc />
        public ErrorNotification(string title, string description) :
            this(title, description, DateTime.Now)
        {

        }

        /// <inheritdoc />
        public ErrorNotification(string title, string description, DateTime date) :
            base(title, description, date, (int)NotificationTypes.Error)
        {

        }
    }
}