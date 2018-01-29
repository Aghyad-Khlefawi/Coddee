// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;

namespace Coddee.Network
{
    public class NetowkrServiceAccessInfo
    {
        public string IP { get; set; }
        public string Port { get; set; }
        public string Protocol { get; set; }
    }

    public class NetworkService
    {
        public Guid ID { get; set; }
        public string Name { get; set; }
        public NetowkrServiceAccessInfo AccessInfo { get; set; }
    }
}