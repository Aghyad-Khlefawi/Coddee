// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using System.Windows.Input;
using Coddee.WPF;

namespace Coddee.Notification
{
    /// <summary>
    /// A ViewModel for notification popup.
    /// </summary>
    public class NotificationPopupViewModel : ViewModelBase, IInteractiveNotification
    {
        /// <inheritdoc />
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

        /// <inheritdoc />
        public event Action<INotification> Opened;

        /// <summary>
        /// Triggered when the popup is closed.
        /// </summary>
        public event Action<NotificationPopupViewModel> Closed;

        /// <inheritdoc cref="Open"/>
        public ICommand OpenCommand { get; }
        
        /// <inheritdoc cref="Close"/>
        public ICommand CloseCommand { get; }

        /// <summary>
        /// Indicates whether the popup is closed
        /// </summary>
        public bool IsClosed { get; set; }

        /// <summary>
        /// Close the popup.
        /// </summary>
        public void Close()
        {
            if (!IsClosed)
            {
                IsClosed = true;
                Closed?.Invoke(this);
            }
        }

        /// <inheritdoc />
        public void Open()
        {
            Opened?.Invoke(this);
            Close();
        }
    }
}
