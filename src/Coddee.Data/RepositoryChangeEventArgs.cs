using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coddee.Data
{
   public class RepositoryChangeEventArgs<T>:EventArgs
    {
        public RepositoryChangeEventArgs(OperationType operationType, T item,bool fromSync)
        {
            OperationType = operationType;
            Item = item;
            FromSync = fromSync;
        }

        public bool FromSync { get; set; }
        public OperationType OperationType { get; set; }
        public T Item { get; set; }
    }
}
