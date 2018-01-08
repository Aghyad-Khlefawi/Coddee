// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Coddee.CodeTools.Components;

namespace Coddee.CodeTools.Sql
{
    public class GetDbTablesQuery:SqlQuery<SqlDbTable>
    {
        public Task<IEnumerable<SqlDbTable>> Execute (SqlConnection connection)
        {
            return Execute(connection, null);
        }

        protected override string GetQueryString()
        {
            return SqlQueryStrings.GetDatabaseTables;
        }
    }
}
