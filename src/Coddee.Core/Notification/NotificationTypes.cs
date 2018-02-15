// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

namespace Coddee.Notification
{
    /// <summary>
    /// Basic notification types.
    /// </summary>
    public enum NotificationTypes
    {
        /// <summary>
        /// Information notification that is not very important.
        /// </summary>
        Information,

        /// <summary>
        /// A warning notification for the operation that may lead to an error in the future.
        /// </summary>
        Warning,

        /// <summary>
        /// Error notification that indicates that some operation has failed.
        /// </summary>
        Error
    }
}
