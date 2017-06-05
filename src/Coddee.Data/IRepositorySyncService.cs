using System;

namespace Coddee.Data
{
    public class RepositorySyncEventArgs
    {
        public object Item { get; set; }
        public OperationType OperationType { get; set; }
    }

    public interface IRepositorySyncService
    {
        event Action<string, RepositorySyncEventArgs> SyncReceived;
        void SyncItem(string identifire, RepositorySyncEventArgs args);
    }

}
