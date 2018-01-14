// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using Coddee.CodeTools.Components;

namespace Coddee.CodeTools.Sql.Queries
{
    public class SqlTableColumn:IEquatable<SqlTableColumn>
    {
        public string ColumnName { get; set; }
        public string ColumnType { get; set; }
        public bool IsNullable { get; set; }
        public bool HasDefaultValue { get; set; }
        public bool IsComputed { get; set; }
        public bool IsPrimaryKey { get; set; }

        [SqlMapIgnore]
        public SqlTable Table { get; set; }

        public bool Equals(SqlTableColumn other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return string.Equals(ColumnName, other.ColumnName) && string.Equals(ColumnType, other.ColumnType) && IsNullable == other.IsNullable && IsPrimaryKey == other.IsPrimaryKey;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((SqlTableColumn) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (ColumnName != null ? ColumnName.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (ColumnType != null ? ColumnType.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ IsNullable.GetHashCode();
                hashCode = (hashCode * 397) ^ HasDefaultValue.GetHashCode();
                hashCode = (hashCode * 397) ^ IsPrimaryKey.GetHashCode();
                return hashCode;
            }
        }
    }
}
