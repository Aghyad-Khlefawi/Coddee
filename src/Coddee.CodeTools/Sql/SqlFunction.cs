using System.Collections.Generic;
using Coddee.CodeTools.Sql.Queries;

namespace Coddee.CodeTools.Sql
{
    public class SqlParameter
    {
        public SqlParameter()
        {
            
        }
        public SqlParameter(string name, object value)
        {
            Name = name;
            Value = value;
        }

        public string Name { get; set; }
        public object Value { get; set; }
        public string Type { get; set; }
        public string DbType { get; set; }
    }
    
    public class SqlResultType
    {
        public string Name { get; set; }
        public List<SqlColumn> Columns { get; set; }
    }

    public class SqlFunction
    {
        public string TableName { get; set; }
        public string SchemaName { get; set; }
        public SqlResultType ResultType { get; set; }
        public List<SqlParameter> Parameters { get; set; }
    }
}
