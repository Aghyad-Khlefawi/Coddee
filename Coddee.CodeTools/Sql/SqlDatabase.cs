using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Coddee.CodeTools.Components;

namespace Coddee.CodeTools.Sql
{
    public class SqlDatabase
    {
        public string DatabaseName { get; set; }
        public string ConnectionString { get; set; }
        public List<SqlDbTable> Tables { get; set; }
    }
}
