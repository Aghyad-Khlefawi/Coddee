// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

namespace Coddee
{
    /// <summary>
    /// Event propagation behavior 
    /// </summary>
    public enum EventRoutingStrategy
    {
        /// <summary>
        /// The event will be sent to all the sub-objects.
        /// </summary>
        Tunnel = 0,

        /// <summary>
        /// The event will be sent to all the parent objects.
        /// </summary>
        Bubble = 1,

        /// <summary>
        /// The event will be sent to it's subscribers only.
        /// </summary>
        Direct = 2
    }

}
