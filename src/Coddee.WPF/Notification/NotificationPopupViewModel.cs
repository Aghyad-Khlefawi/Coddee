// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using System.Windows.Input;
using Coddee.WPF;

namespace Coddee.Notification
{
    public class NotificationPopupViewModel : ViewModelBase, IInteractiveNotification
    {
        public NotificationPopupViewModel(INotification notification)
        {
            Title = notification.Title;
            Description = notification.Description;
            Type = notification.Type;
            Date = notification.Date;
            Category = notification.Category;
            Parameter = notification.Parameter;

            OpenCommand = CreateReactiveCommand(Open);
            CloseCommand = CreateReactiveCommand(Close);

            if (notification is IInteractiveNotification interactiveNotification)
                Opened += e => interactiveNotification.Open();
        }

        public string Title { get; set; }
        public string Description { get; set; }
        public int Type { get; }
        public DateTime Date { get; set; }
        public string Category { get; set; }
        public string Parameter { get; set; }

        public event Action<INotification> Opened;
        public event Action<NotificationPopupViewModel> Closed;

        public ICommand OpenCommand { get; }
        public ICommand CloseCommand { get; }

        public bool IsClosed { get; set; }

        public void Close()
        {
            if (!IsClosed)
            {
                IsClosed = true;
                Closed?.Invoke(this);
            }
        }

        public void Open()
        {
            Opened?.Invoke(this);
            Close();
        }
    }
}
