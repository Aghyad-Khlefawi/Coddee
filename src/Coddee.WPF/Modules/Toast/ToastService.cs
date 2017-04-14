// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using Coddee.Collections;
using Microsoft.Practices.Unity;

namespace Coddee.WPF.Modules.Toast
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
                        Message = "Message1",
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
        private double _duration;

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
        public void Initialize(Region toastRegion, double duration)
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
            var toast = _container.Resolve<Toast>();
            ToastList.Add(toast);
            toast.Initialize(message,
                             type,
                             _duration,
                             e =>
                             {
                                 ToastList.Remove(e);
                             });
        }
    }
}