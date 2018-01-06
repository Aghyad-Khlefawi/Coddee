using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coddee.CodeTools.Sql
{
    [AttributeUsage(AttributeTargets.All, Inherited = true, AllowMultiple = false)]
    public sealed class SqlMapIgnoreAttribute : Attribute
    {
        public SqlMapIgnoreAttribute()
        {
            
        }
    }
}
