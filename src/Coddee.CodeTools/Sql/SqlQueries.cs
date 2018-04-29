// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using Coddee.CodeTools.Sql;
using Coddee.CodeTools.Sql.Queries;

namespace Coddee.CodeTools.Components
{
    public class SqlQueryStrings
    {
        /// <summary>
        /// Sets the database you want to execute the queries on
        /// </summary>
        public static string UseDatabase = "Use [@Database]";

        /// <summary>
        /// Return the tables in the current db
        /// Result columns: SchemaName,TableName
        /// </summary>
        public static string GetDatabaseTables = "SELECT   schemas.name AS SchemaName ,tables.name AS TableName FROM sys.tables INNER JOIN sys.schemas ON schemas.schema_id = TABLES.schema_id WHERE tables.name <> 'sysdiagrams' ORDER BY schemas.name,TABLES.name ;";

        /// <summary>
        /// Returns the table columns
        /// Result columns: SchemaName,TableName,ColumnName,ColumnType,IsNullbale,HasDefaultValue,IsComputed,IsPrimaryKey
        /// </summary>
        public static string GetTableColumns = "SELECT   schemas.name AS SchemaName , tables.name AS TableName, columns.name AS ColumnName, types.name AS ColumnType, columns.is_nullable AS IsNullable, CAST(CASE WHEN columns.is_identity = 1 OR columns.default_object_id <> 0 OR columns.is_computed = 1 THEN 1 ELSE 0 END AS BIT) AS HasDefaultValue,     columns.is_computed AS IsComputed, CAST(CASE WHEN EXISTS(SELECT indexes.object_id     FROM   sys.indexes     INNER JOIN sys.index_columns ON index_columns.index_id = indexes.index_id     AND index_columns.object_id = indexes.object_id     WHERE  indexes.is_primary_key = 1 AND indexes.object_id = tables.object_id     AND index_columns.column_id = columns.column_id) THEN 1 ELSE 0 END AS BIT) AS IsPrimaryKey FROM     sys.columns INNER JOIN sys.tables ON TABLES.object_id = COLUMNS.object_id     INNER JOIN sys.schemas ON schemas.schema_id = TABLES.schema_id     INNER JOIN sys.types ON types.user_type_id = COLUMNS.user_type_id     WHERE    tables.name <> 'sysdiagrams'   AND tables.name = @tableName     ORDER BY schemas.name, TABLES.name, COLUMNS.column_id;";

    }

    public class SqlQueries
    {
        public static UseDatabaseQuery UseDatabase = new UseDatabaseQuery();
        public static GetDbTablesQuery GetDatabaseTables = new GetDbTablesQuery();
        public static GetTableColumnsQuery GetTableColumns = new GetTableColumnsQuery();
    }
}
