// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System.Windows;
using System.Windows.Controls;

namespace Coddee.Notification
{
    public class NotificationTemplateSelector : DataTemplateSelector
    {

        public DataTemplate InformationTemplate { get; set; }
        public DataTemplate WarningTemplate { get; set; }
        public DataTemplate ErrorTemplate { get; set; }


        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is INotification notification)
            {
                switch (notification.Type)
                {
                    case (int)NotificationTypes.Information:
                        return InformationTemplate;
                    case (int)NotificationTypes.Warning:
                        return WarningTemplate;
                    case (int)NotificationTypes.Error:
                        return ErrorTemplate;
                }
            }

            return base.SelectTemplate(item, container);
        }
    }
}
