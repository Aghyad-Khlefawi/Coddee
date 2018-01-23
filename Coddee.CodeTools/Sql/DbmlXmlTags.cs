using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coddee.CodeTools.Sql
{
    public class DbmlXmlTags
    {
        public const string Database = nameof(Database);
        public const string Column = nameof(Column);
        public const string Type = nameof(Type);
        public const string Table = nameof(Table);
        public const string Association = nameof(Association);
        public const string Function = nameof(Function);
        public const string Parameter = nameof(Parameter);
        public const string ElementType = nameof(ElementType);
    }

    public class DbmlXmlAttributes
    {
        public const string DbType = nameof(DbType);
        public const string Name = nameof(Name);
        public const string Type = nameof(Type);
        public const string IsPrimaryKey = nameof(IsPrimaryKey);
        public const string IsDbGenerated = nameof(IsDbGenerated);
        public const string CanBeNull = nameof(CanBeNull);
    }
}
