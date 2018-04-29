// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Coddee.CodeTools.Components;
using Coddee.CodeTools.Sql;
using Coddee.CodeTools.Sql.Queries;

namespace Coddee.CodeTools
{
    public enum CompareItemStatus
    {
        Matched,
        Different,
        Missing
    }

    public class TableCompareResult
    {
        public SqlTable Table { get; set; }
        public List<SqlColumn> MissingColumns { get; set; }
        public CompareItemStatus ItemStatus { get; set; }
    }
    public class CompareResult
    {
        public object Item { get; set; }
        public CompareItemStatus ItemStatus { get; set; }
    }

    public class DbmlCompareResult
    {
        public List<TableCompareResult> Tables { get; set; }
        public List<CompareResult> Views { get; set; }
        public List<CompareResult> Functions { get; set; }
        public List<CompareResult> Procedures { get; set; }
    }

    public class DbmlContent
    {
        public List<SqlTable> Tables { get; set; }
        public List<SqlTable> Views { get; set; }
        public List<SqlFunction> Functions { get; set; }
        public List<SqlFunction> Procedures { get; set; }
    }
    public class DbmlCompare
    {

        public DbmlCompareResult Compare(string filePath, IEnumerable<SqlTable> sqlTables)
        {
            DbmlCompareResult res = new DbmlCompareResult
            {
                Functions = new List<CompareResult>(),
                Procedures = new List<CompareResult>(),
                Tables = new List<TableCompareResult>(),
                Views = new List<CompareResult>()
            };

            var dbml = GetContent(filePath);

            foreach (var table in sqlTables)
            {
                var dbmlTable = dbml.Tables.FirstOrDefault(e => e.Equals(table));
                var tableResult = new TableCompareResult
                {
                    Table = table,
                    ItemStatus = CompareItemStatus.Matched,
                    MissingColumns = new List<SqlColumn>()
                };
                if (dbmlTable == null)
                {
                    //Table missing from dbml
                    tableResult.ItemStatus = CompareItemStatus.Missing;
                }
                else
                {
                    foreach (var column in dbmlTable.Columns)
                    {
                        if (table.Columns.All(e => !e.Equals(column)))
                        {
                            //Table column missing from dbml,
                            tableResult.MissingColumns.Add(column);
                        }
                    }
                    if (tableResult.MissingColumns.Any())
                        tableResult.ItemStatus = CompareItemStatus.Different;
                }
                res.Tables.Add(tableResult);
            }

            return res;
        }

        private DbmlContent GetContent(string dbmlPath)
        {
            var xml = new XmlDocument();
            xml.Load(dbmlPath);
            List<SqlTable> tables = new List<SqlTable>();
            List<SqlFunction> functions = new List<SqlFunction>();
            XmlNode database = xml.GetElementsByTagName(DbmlXmlTags.Database).Item(0);
            foreach (XmlNode node in database.ChildNodes)
            {
                if (node.Name == DbmlXmlTags.Table)
                {
                    var name = node.Attributes[DbmlXmlAttributes.Name].Value;
                    var fullName = name.Split('.');
                    var table = new SqlTable
                    {
                        TableName = fullName[1],
                        SchemaName = fullName[0],
                        Columns = new List<SqlColumn>()
                    };

                    var type = node.FirstChild.Attributes[DbmlXmlAttributes.Name].Value;
                    table.ClassName = type;
                    foreach (XmlNode tableNode in node.FirstChild.ChildNodes)
                    {
                        if (tableNode.Name == DbmlXmlTags.Column)
                        {
                            SqlColumn column = GetColumnFromNode(tableNode);
                            column.Table = table;
                            table.Columns.Add(column);
                        }
                    }
                    tables.Add(table);
                }
                else if (node.Name == DbmlXmlTags.Function)
                {
                    var name = node.Attributes[DbmlXmlAttributes.Name].Value;
                    var fullName = name.Split('.');
                    var function = new SqlFunction
                    {
                        TableName = fullName[1],
                        SchemaName = fullName[0],
                        Parameters = new List<SqlParameter>()
                    };
                    foreach (XmlNode functionNode in node.ChildNodes)
                    {
                        if (functionNode.Name == DbmlXmlTags.Parameter)
                        {
                            var column = new SqlParameter
                            {
                                Name = functionNode.Attributes[DbmlXmlAttributes.Name].Value,
                                DbType = FixColumnType(functionNode.Attributes[DbmlXmlAttributes.DbType].Value),
                            };
                            function.Parameters.Add(column);
                        }

                        if (functionNode.Name == DbmlXmlTags.ElementType)
                        {
                            var attr = functionNode.Attributes[DbmlXmlAttributes.Name];
                            if (attr != null)
                            {
                                var result = new SqlResultType
                                {
                                    Name = attr.Value,
                                    Columns = new List<SqlColumn>()
                                };

                                foreach (XmlNode columnNode in functionNode.ChildNodes)
                                {
                                    var column = GetColumnFromNode(columnNode);
                                    result.Columns.Add(column);
                                }

                                function.ResultType = result;
                            }
                        }
                    }
                    functions.Add(function);
                }

            }
            return new DbmlContent
            {
                Tables = tables,
                Functions = functions
            };
            
        }

        private SqlColumn GetColumnFromNode(XmlNode tableNode)
        {
            return new SqlColumn
            {
                ColumnName = tableNode.Attributes[DbmlXmlAttributes.Name].Value,
                IsPrimaryKey = bool.Parse(tableNode.Attributes[DbmlXmlAttributes.IsPrimaryKey]?.Value ?? bool.FalseString),
                ColumnType = FixColumnType(tableNode.Attributes[DbmlXmlAttributes.DbType].Value),
                HasDefaultValue = bool.Parse(tableNode.Attributes[DbmlXmlAttributes.IsPrimaryKey]?.Value ?? bool.FalseString),
                IsNullable = bool.Parse(tableNode.Attributes[DbmlXmlAttributes.CanBeNull]?.Value ?? bool.FalseString),
            };
        }

        private string FixColumnType(string columnType)
        {
            var name = columnType.ToLower();
            var dbTypeName = name.Split(' ')[0];
            return !dbTypeName.Contains('(') ? dbTypeName : dbTypeName.Substring(0, dbTypeName.IndexOf('('));
        }
    }
}
