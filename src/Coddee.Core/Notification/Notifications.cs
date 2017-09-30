// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;

namespace Coddee.Notification
{
    public class InformationNotification : NotificationBase
    {
        public InformationNotification(string title, string description) :
            this(title, description, DateTime.Now)
        {

        }

        public InformationNotification(string title, string description, DateTime date) :
            base(title, description, date, (int)NotificationTypes.Information)
        {

        }
    }

    public class WarningNotification : NotificationBase
    {
        public WarningNotification(string title, string description) :
            this(title, description, DateTime.Now)
        {

        }

        public WarningNotification(string title, string description, DateTime date) :
            base(title, description, date, (int)NotificationTypes.Warning)
        {

        }
    }

    public class ErrorNotification : NotificationBase
    {
        public ErrorNotification(string title, string description) :
            this(title, description, DateTime.Now)
        {

        }

        public ErrorNotification(string title, string description, DateTime date) :
            base(title, description, date, (int)NotificationTypes.Error)
        {

        }
    }
}