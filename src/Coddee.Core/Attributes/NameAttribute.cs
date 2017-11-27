using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coddee.Attributes
{
    [AttributeUsage(AttributeTargets.All, AllowMultiple = true, Inherited = true)]
    public class NameAttribute : Attribute
    {
        public string Value { get; }

        public NameAttribute(string value)
        {
            Value = value;
        }
    }
}
