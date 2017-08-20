// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

namespace Coddee.Services
{
    /// <summary>
    /// A service responsible for managing global events that can be subscribed to or invoked from across the application.
    /// </summary>
    public interface IGlobalEventsService
    {
        /// <summary>
        /// Gets the event that needs to be inoked or subscribed to.
        /// </summary>
        /// <typeparam name="TResult">The event type.</typeparam>
        /// <returns>The global event object.</returns>
        TResult GetEvent<TResult>() where TResult : class, IGlobalEvent, new();
    }
}