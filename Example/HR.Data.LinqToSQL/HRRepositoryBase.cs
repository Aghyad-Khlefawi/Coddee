// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using Coddee;
using Coddee.Data.LinqToSQL;
using HR.Data.Linq.DB;

namespace HR.Data.Linq.Repositories
{
    public class HRRepositoryBase:LinqRepositoryBase<HRDataClassesDataContext>
    {
    }

    public class HRRepositoryBase<TTable> : LinqRepositoryBase<HRDataClassesDataContext, TTable>
        where TTable:class,new()
    {
    }

    public class ReadOnlyHRRepositoryBase<TTable, TModel, TKey> : ReadOnlyLinqRepositoryBase<HRDataClassesDataContext, TTable,TModel,TKey>
        where TTable : class, new()
        where TModel : IUniqueObject<TKey>, new()
    {

    }

    public class CRUDHRRepositoryBase<TTable, TModel, TKey> : CRUDLinqRepositoryBase<HRDataClassesDataContext, TTable, TModel, TKey>
        where TTable : class, new()
        where TModel : IUniqueObject<TKey>, new()
    {

    }
}