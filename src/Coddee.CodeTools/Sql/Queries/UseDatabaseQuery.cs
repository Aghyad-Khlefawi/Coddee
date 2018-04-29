// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System.Data.SqlClient;
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
