using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Coddee.CodeTools.Components;

namespace Coddee.CodeTools.Sql.Queries
{
    public class GetTableColumnsQuery : SqlQuery<SqlTableColumn>
    {
        public Task<IEnumerable<SqlTableColumn>> Execute(SqlConnection connection, string tableName)
        {
            return Execute(connection, new SqlParameter("tableName", tableName));
        }

        protected override string GetQueryString()
        {
            return SqlQueryStrings.GetTableColumns;
        }
    }
}
