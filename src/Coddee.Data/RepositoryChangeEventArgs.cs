using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coddee.Data
{
   public class RepositoryChangeEventArgs<T>:EventArgs
    {
        public RepositoryChangeEventArgs(OperationType operationType, T item)
        {
            OperationType = operationType;
            Item = item;
        }

        public OperationType OperationType { get; set; }
        public T Item { get; set; }
    }
}
