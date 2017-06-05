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