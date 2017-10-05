// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using System.Timers;
using Coddee.Collections;
using Coddee.WPF;

namespace Coddee.Notification
{
    public class NotificationService : ViewModelBase<NotificationServiceView>, INotificationService
    {
        private double _notificationDuration;

        private AsyncObservableCollection<INotification> _notifications;
        public AsyncObservableCollection<INotification> Notifications
        {
            get { return _notifications; }
            set { SetProperty(ref this._notifications, value); }
        }

        protected override void OnDesignMode()
        {
            base.OnDesignMode();
            Notifications = new AsyncObservableCollection<INotification>
            {
                new InformationNotification("Info","Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum."),
                new WarningNotification("Warning","Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Tortor consequat id porta nibh venenatis cras sed felis. Ut consequat semper viverra nam libero justo laoreet. Quam quisque id diam vel quam elementum pulvinar. Neque gravida in fermentum et sollicitudin ac. "),
                new ErrorNotification("Error","Id volutpat lacus laoreet non curabitur. Adipiscing elit duis tristique sollicitudin nibh sit amet. Turpis in eu mi bibendum neque "),
            };
        }

        public event EventHandler<INotification> NotificationReceived;

        public void Notify(INotification notification)
        {
            NotificationReceived?.Invoke(this, notification);
            var popup = new NotificationPopupViewModel(notification);
            popup.Closed += e => Notifications.Remove(e);
            var timer = new Timer(_notificationDuration);
            timer.Elapsed += delegate
            {
                ExecuteOnUIContext(() =>
                {
                    Notifications.Remove(popup);
                    timer.Stop();
                    timer.Dispose();
                });
            };
            timer.Start();
            GC.KeepAlive(timer);
            Notifications.Insert(0, popup);
        }


        public void Inititlize(Region notificationsRegion, double notificationDuration = 5000)
        {
            notificationsRegion.View(this);
            _notificationDuration = notificationDuration;
            Notifications = AsyncObservableCollection<INotification>.Create();
        }
    }
}
