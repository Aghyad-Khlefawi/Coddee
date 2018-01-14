// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using Coddee.CodeTools.Components;
using Coddee.CodeTools.Sql.Queries;

namespace Coddee.CodeTools
{
    public class DbmlCompareResult
    {
        public List<SqlTable> MissingTables { get; set; }
        public List<SqlTableColumn> MissingColumns { get; set; }
    }

    public class DbmlCompare
    {

        public DbmlCompareResult Compare(string filePath, IEnumerable<SqlTable> sqlTables)
        {
            DbmlCompareResult res = new DbmlCompareResult
            {
                MissingColumns = new List<SqlTableColumn>(),
                MissingTables = new List<SqlTable>()
            };
            var xml = new XmlDocument();
            xml.Load(filePath);
            xml.DocumentElement.SetAttribute("xmlns", "http://schemas.microsoft.com/linqtosql/dbml/2007");
            List<SqlTable> tables = new List<SqlTable>();
            XmlNode database = xml.GetElementsByTagName("Database").Item(0);

  
            foreach (XmlNode node in database.ChildNodes)
            {
                if (node.Name == "Table")
                {
                    var name = node.Attributes["Name"].Value;
                    var fullName = name.Split('.');
                    var table = new SqlTable
                    {
                        TableName = fullName[1],
                        SchemaName = fullName[0],
                        Columns = new List<SqlTableColumn>()
                    };

                    var type = node.FirstChild.Attributes["Name"].Value;
                    table.ClassName = type;
                    foreach (XmlNode tableNode in node.FirstChild.ChildNodes)
                    {
                        if (tableNode.Name == "Column")
                        {
                            var column = new SqlTableColumn
                            {
                                ColumnName = tableNode.Attributes["Name"].Value,
                                IsPrimaryKey = bool.Parse(tableNode.Attributes["IsPrimaryKey"]?.Value ?? bool.FalseString),
                                ColumnType = tableNode.Attributes["DbType"].Value,
                                HasDefaultValue = bool.Parse(tableNode.Attributes["IsPrimaryKey"]?.Value ?? bool.FalseString),
                                IsNullable = bool.Parse(tableNode.Attributes["CanBeNull"]?.Value ?? bool.FalseString),
                                Table = table
                            };
                            FixColumnType(column);
                            table.Columns.Add(column);
                        }
                    }
                    tables.Add(table);
                }
            }

            foreach (var table in sqlTables)
            {
                var dbmlTable = tables.FirstOrDefault(e => e.Equals(table));
                if (dbmlTable == null)
                {
                    //Table missing from dbml
                    res.MissingTables.Add(table);
                }
                else
                {
                    foreach (var column in dbmlTable.Columns)
                    {
                        if (table.Columns.All(e => !e.Equals(column)))
                        {
                            //Table column missing from dbml,
                            res.MissingColumns.Add(column);
                        }
                    }
                }
            }

            return res;
        }

        private void FixColumnType(SqlTableColumn column)
        {
            var name = column.ColumnType.ToLower();
            var dbTypeName = name.Split(' ')[0];
            if (!dbTypeName.Contains('('))
            {
                column.ColumnType = dbTypeName;
            }
            else
            {
                column.ColumnType = dbTypeName.Substring(0, dbTypeName.IndexOf('('));
            }
        }
    }
}
