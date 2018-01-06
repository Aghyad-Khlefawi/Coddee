namespace Coddee.CodeTools.Sql.Queries
{
    public class SqlTableColumn
    {
        public string ColumnName { get; set; }
        public string ColumnType { get; set; }
        public bool IsNullable { get; set; }
        public bool HasDefaultValue { get; set; }
        public bool IsComputed { get; set; }
        public bool IsPrimaryKey { get; set; }
    }
}
