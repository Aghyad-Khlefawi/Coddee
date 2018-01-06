using Coddee.CodeTools.Sql;
using Coddee.CodeTools.Sql.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coddee.CodeTools.Components
{
    public class SqlDbTable
    {
        public string TableName { get; set; }
        public string SchemaName { get; set; }

        [SqlMapIgnore]
        public List<SqlTableColumn> Columns { get; set; }
    }
}
