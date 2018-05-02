// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using Coddee.Data.LinqToSQL;
using HR.Data.Linq.DB;

namespace HR.Data.LinqToSQL
{
    public class HRDBManager:LinqDBManager<HRDataClassesDataContext>
    {
        public override HRDataClassesDataContext CreateContext()
        {
            return new HRDataClassesDataContext(Connection);
        }
    }
}