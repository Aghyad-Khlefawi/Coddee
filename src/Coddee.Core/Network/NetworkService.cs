// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;

namespace Coddee.Network
{
    /// <summary>
    /// The required information to access a NetworkService
    /// </summary>
    public class NetowkrServiceAccessInfo
    {
        /// <summary>
        /// The service IP address as a string e.g. "192.168.1.30"
        /// </summary>
        public string IP { get; set; }
        
        /// <summary>
        /// The service TCP port e.g. "8080"
        /// </summary>
        public string Port { get; set; }

        /// <summary>
        /// The protocol used by the service e.g. "http" 
        /// </summary>
        public string Protocol { get; set; }
    }

    /// <summary>
    /// Identify a Network service
    /// </summary>
    public class NetworkService
    {

        /// <summary>
        /// A GUID to identify the service
        /// </summary>
        public Guid ID { get; set; }

        /// <summary>
        /// A friendly name for the service
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The service access information.
        /// </summary>
        public NetowkrServiceAccessInfo AccessInfo { get; set; }
    }
}