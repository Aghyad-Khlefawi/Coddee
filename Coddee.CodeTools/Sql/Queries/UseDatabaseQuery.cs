using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Coddee.CodeTools.Components;

namespace Coddee.CodeTools.Sql
{
    public class UseDatabaseQuery : SqlQuery
    {
        public Task ExecuteNonQuery(SqlConnection connection, string databaseName)
        {
            return ExecuteNonQuery(connection, GetQueryString().Replace("@Database", databaseName), null);
        }

        protected override string GetQueryString()
        {
            return SqlQueryStrings.UseDatabase;
        }
    }
}
