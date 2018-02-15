// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

namespace Coddee.Services
{
    /// <summary>
    /// A service that manages the <see cref="IEvent"/> objects.
    /// </summary>
    public interface IEventDispatcher
    {
        /// <summary>
        /// Gets the event that needs to be invoked or subscribed to.
        /// </summary>
        /// <typeparam name="TResult">The event type.</typeparam>
        /// <returns>The global event object.</returns>
        TResult GetEvent<TResult>() where TResult : class, IEvent, new();
    }
}
