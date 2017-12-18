// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using Coddee.WPF;

namespace Coddee.Services
{
    public enum ToastType
    {
        Information = 0,
        Success = 1,
        Warning = 2,
        Error = 3
    }
    public interface IToastService
    {
        /// <summary>
        /// Initilize the toast service
        /// </summary>
        /// <param name="toastRegion">The region that the toast service the be contained</param>
        /// <param name="duration">The default duration for the toasts</param>
        void Initialize(Region toastRegion, TimeSpan duration);

        /// <summary>
        /// Show a toast
        /// </summary>
        void ShowToast(string message, ToastType type);
        /// <summary>
        /// Show a toast
        /// </summary>
        void ShowToast(string message, ToastType type,double duration);
        void ShowToast(string message, ToastType type, TimeSpan duration);
    }
}
