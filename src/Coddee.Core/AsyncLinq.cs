// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Coddee.Collections;

namespace Coddee
{
    public static class AsyncLinq
    {
        public static async Task<List<T>> ToList<T>(this Task<IEnumerable<T>> collection)
        {
            return (await collection).ToList();
        }
        public static async Task<IEnumerable<T>> Where<T>(this Task<IEnumerable<T>> collection, Func<T, bool> predicate)
        {
            return (await collection).Where(predicate);
        }
        public static async Task<IEnumerable<TResult>> Select<T,TResult>(this IEnumerable<T> collection, Func<T, Task<TResult>> select)
        {
            var list = new List<TResult>();
            foreach (var item in collection)
            {
                list.Add(await select(item));
            }
            return list;
        }
        public static Task Fill<T>(this Task<IEnumerable<T>> collection, AsyncObservableCollection<T> asyncCollection)
        {
            return asyncCollection.FillAsync(collection);
        }

        public static Task ClearAndFill<T>(this Task<IEnumerable<T>> collection, AsyncObservableCollection<T> asyncCollection)
        {
            return asyncCollection.ClearAndFillAsync(collection);
        }

        public static AsyncObservableCollection<T> ToAsyncObservableCollection<T>(this IEnumerable<T> collection)
        {
            return AsyncObservableCollection<T>.Create(collection);
        }

        public static async Task<AsyncObservableCollection<T>> ToAsyncObservableCollection<T>(this Task<IEnumerable<T>> collection)
        {
            return AsyncObservableCollection<T>.Create(await collection);
        }


        public static async Task<IEnumerable<T>> ForEach<T>(this Task<IEnumerable<T>> collection,Action<T> action)
        {
            return (await collection).ForEach(action);
        }
    }
}
