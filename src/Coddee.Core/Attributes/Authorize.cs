using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coddee.Attributes
{
    [AttributeUsage(AttributeTargets.All, Inherited = true, AllowMultiple = true)]
    public sealed class AuthorizeAttribute : Attribute
    {

        public AuthorizeAttribute()
        {
            
        }
        public AuthorizeAttribute(string claim)
        {
            Claim = claim;
        }

        public string Claim { get; }

    }
}
