// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Coddee.CodeTools.Components;

namespace Coddee.CodeTools.Sql.Queries
{
    public class GetTableColumnsQuery : SqlQuery<SqlColumn>
    {
        public Task<IEnumerable<SqlColumn>> Execute(SqlConnection connection, string tableName)
        {
            return Execute(connection, new SqlParameter("tableName", tableName));
        }

        protected override string GetQueryString()
        {
            return SqlQueryStrings.GetTableColumns;
        }
    }
}
