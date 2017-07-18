using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coddee.Data
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
    public sealed class RepositoryAttribute : Attribute
    {
        public RepositoryAttribute(Type implementedRepository, bool discoverable = true)
        {
            ImplementedRepository = implementedRepository;
            Discoverable = discoverable;
        }

        public bool Discoverable { get; set; }
        public Type ImplementedRepository { get; set; }
    }
}