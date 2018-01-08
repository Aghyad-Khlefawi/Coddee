// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System.Collections.Generic;
using Coddee.CodeTools.Components;

namespace Coddee.CodeTools.Sql
{
    public class SqlDatabase
    {
        public string DatabaseName { get; set; }
        public string ConnectionString { get; set; }
        public List<SqlDbTable> Tables { get; set; }
    }
}
