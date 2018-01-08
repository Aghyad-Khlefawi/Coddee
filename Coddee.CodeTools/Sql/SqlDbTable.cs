// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using Coddee.CodeTools.Sql;
using Coddee.CodeTools.Sql.Queries;
using System.Collections.Generic;

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
