// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using System.Windows.Threading;
using Coddee.WPF;

namespace Coddee.Services.Toast
{
    /// <summary>
    /// Toast object
    /// </summary>
    public class Toast : ViewModelBase
    {
        /// <summary>
        /// Initialize the item timer and information
        /// </summary>
        /// <param name="message">The toast content</param>
        /// <param name="type">The toast type</param>
        /// <param name="duration">How long will the toast stay on the screen in milliseconds</param>
        /// <param name="distroy">An action triggered after the duration is over</param>
        public void Initialize(string message, ToastType type, double duration, Action<Toast> distroy)
        {
            Message = message;
            Type = type;
            _timer = new DispatcherTimer(TimeSpan.FromMilliseconds(duration),
                                         DispatcherPriority.Background,
                                         TimerElapsed,
                                         GetDispatcher());
            GC.KeepAlive(_timer);
            _distroy += distroy;
        }

        private DispatcherTimer _timer;
        private event Action<Toast> _distroy;

        /// <summary>
        /// Called when the duration is over
        /// </summary>
        private void TimerElapsed(object sender, EventArgs e)
        {
            _timer.Stop();
            GC.SuppressFinalize(_timer);
            _distroy?.Invoke(this);
        }

        private string _message;
        public string Message
        {
            get { return _message; }
            set { SetProperty(ref this._message, value); }
        }

        private ToastType _type;
        public ToastType Type
        {
            get { return _type; }
            set { SetProperty(ref this._type, value); }
        }
    }
}