// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using Coddee.Collections;
using Coddee.WPF;


namespace Coddee.Services.Toast
{
    public class ToastService : ViewModelBase<ToastServiceView>, IToastService
    {
        public ToastService()
        {
            if (IsDesignMode())
            {
                ToastList = AsyncObservableCollection<Toast>.Create(new []
                {
                    new Toast
                    {
                        Message = "Like harold ye sorrow in monastic come not pleasure her did way been condole come pollution him the true superstition",
                        Type = ToastType.Error
                    },
                    new Toast
                    {
                        Message = "Message2",
                        Type = ToastType.Information
                    },
                    new Toast
                    {
                        Message = "Message3",
                        Type = ToastType.Success
                    },
                    new Toast
                    {
                        Message = "Message4",
                        Type = ToastType.Warning
                    }
                });
            }
        }
        private Region _toastRegion;
        private TimeSpan _duration;

        private AsyncObservableCollection<Toast> _toastList;
        public AsyncObservableCollection<Toast> ToastList
        {
            get { return _toastList; }
            set { SetProperty(ref this._toastList, value); }
        }

        /// <summary>
        /// Initilize the toast service
        /// </summary>
        /// <param name="toastRegion">The region that the toast service the be contained</param>
        /// <param name="duration">The default duration for the toasts</param>
        public void Initialize(Region toastRegion, TimeSpan duration)
        {
            _toastRegion = toastRegion;
            _duration = duration;
            ToastList = AsyncObservableCollection<Toast>.Create();
            _toastRegion.View(this);
        }

        /// <summary>
        /// Show a toast
        /// </summary>
        public void ShowToast(string message, ToastType type)
        {
            ShowToast(message,type,_duration);
        }

        /// <summary>
        /// Show a toast
        /// </summary>
        public void ShowToast(string message, ToastType type,double duration)
        {
            ShowToast(message, type, TimeSpan.FromMilliseconds(duration));
        }
        
        /// <summary>
        /// Show a toast
        /// </summary>
        public void ShowToast(string message, ToastType type, TimeSpan duration)
        {
            var toast = _container.Resolve<Toast>();
            ToastList.Add(toast);
            toast.Initialize(message,
                             type,
                             duration,
                             e =>
                             {
                                 ToastList.Remove(e);
                             });
        }
    }
}