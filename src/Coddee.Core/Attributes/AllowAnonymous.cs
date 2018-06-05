using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coddee.Attributes
{
    /// <summary>
    /// Indicate that an API action requires no permissions.
    /// </summary>
    [AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = false)]
    public sealed class AllowAnonymousAttribute : Attribute
    {
        /// <inheritdoc />
        public AllowAnonymousAttribute()
        {
            
        }
    }
}
