// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using Coddee.CodeTools.Sql.Queries;
using Coddee.WPF;

namespace Coddee.CodeTools.Components.Data
{
    public class ColumnImportArgumentsViewModel : ViewModelBase
    {
        public ColumnImportArgumentsViewModel(SqlTableColumn column)
        {
            Name = column.ColumnName;
            SqlType = column.ColumnType;
            IsPrimaryKey = column.IsPrimaryKey;
            Type = GetCSharpType(column);
        }

        private Type GetCSharpType(SqlTableColumn column)
        {
            switch (column.ColumnType)
            {
                case "smallint":
                case "tinyint":
                case "int":
                    return typeof(Int32);
                case "nchar":
                case "varchar":
                case "nvarchar":
                    return typeof(string);
                case "bit":
                    return typeof(bool);
                case "datetime":
                case "date":
                case "datetime2":
                    return typeof(DateTime);
                case "char":
                    return typeof(char);
                case "float":
                    return typeof(float);
                case "decimal":
                    return typeof(decimal);
                case "uniqueidentifier":
                    return typeof(Guid);
                case "varbinary":
                case "image":
                    return typeof(byte[]);
            }
            return typeof(object);
        }

        private bool _isPrimaryKey;
        public bool IsPrimaryKey
        {
            get { return _isPrimaryKey; }
            set { SetProperty(ref _isPrimaryKey, value); }
        }
        private string _name;
        public string Name
        {
            get { return _name; }
            set { SetProperty(ref _name, value); }
        }
        private string _sqlType;
        public string SqlType
        {
            get { return _sqlType; }
            set { SetProperty(ref _sqlType, value); }
        }
        private Type _type;
        public Type Type
        {
            get { return _type; }
            set { SetProperty(ref _type, value); }
        }

        private bool _importColumn = true;
        public bool ImportColumn
        {
            get { return _importColumn; }
            set { SetProperty(ref _importColumn, value); }
        }
    }
}