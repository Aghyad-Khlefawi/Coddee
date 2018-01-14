// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using Coddee.CodeTools.Sql;
using Coddee.CodeTools.Sql.Queries;
using System.Collections.Generic;

namespace Coddee.CodeTools.Components
{
    public class SqlTable:IEquatable<SqlTable>
    {
        public string TableName { get; set; }
        public string SchemaName { get; set; }

        [SqlMapIgnore]
        public string ClassName { get; set; }
        [SqlMapIgnore]
        public List<SqlTableColumn> Columns { get; set; }

        public bool Equals(SqlTable other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return string.Equals(TableName, other.TableName) && string.Equals(SchemaName, other.SchemaName);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((SqlTable) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((TableName != null ? TableName.GetHashCode() : 0) * 397) ^ (SchemaName != null ? SchemaName.GetHashCode() : 0);
            }
        }
    }
}
