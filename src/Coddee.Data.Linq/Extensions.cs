// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;

namespace Coddee.Data
{
    public static class Extensions
    {
        public static void DeleteAllOnSubmit<TTable>(this Table<TTable> table,Func<TTable, bool> filter) where TTable : class
        {
            table.DeleteAllOnSubmit(table.Where(filter));
        }
        public static void DeleteAllAndSubmit<TTable>(this Table<TTable> table, Func<TTable, bool> filter) where TTable : class
        {
            table.DeleteAllOnSubmit(filter);
            table.Context.SubmitChanges();
        }
        public static void DeleteAllAndSubmit<TTable>(this Table<TTable> table,IEnumerable<TTable> items) where TTable : class
        {
            table.DeleteAllOnSubmit(items);
            table.Context.SubmitChanges();
        }
        public static void DeleteOnSubmit<TTable>(this Table<TTable> table, Func<TTable, bool> filter) where TTable : class
        {
            table.DeleteOnSubmit(table.First<TTable>(filter));
        }
        public static void DeleteAndSubmit<TTable>(this Table<TTable> table, Func<TTable, bool> filter) where TTable : class
        {
            table.DeleteOnSubmit(filter);
            table.Context.SubmitChanges();
        }
        public static void InsertAndSubmit<TTable>(this Table<TTable> table, TTable item) where TTable : class
        {
            table.InsertOnSubmit(item);
            table.Context.SubmitChanges();
        }
        public static void InsertAllAndSubmit<TTable>(this Table<TTable> table, IEnumerable<TTable> items) where TTable : class
        {
            table.InsertAllOnSubmit(items);
            table.Context.SubmitChanges();
        }
    }
}
