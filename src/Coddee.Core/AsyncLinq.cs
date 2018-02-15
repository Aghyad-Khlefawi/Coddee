// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Coddee.Collections;

namespace Coddee
{
    /// <summary>
    /// Extensions for working with collection asynchronously
    /// </summary>
    public static class AsyncLinq
    {
        /// <summary>
        /// Returns the result of a task as a List.
        /// </summary>
        public static async Task<List<T>> ToList<T>(this Task<IEnumerable<T>> collection)
        {
            return (await collection).ToList();
        }

        /// <summary>
        /// Filters a task result.
        /// </summary>
        public static async Task<IEnumerable<T>> Where<T>(this Task<IEnumerable<T>> collection, Func<T, bool> predicate)
        {
            return (await collection).Where(predicate);
        }

        /// <summary>
        /// Projects each element of task result into a new form
        /// </summary>
        public static async Task<IEnumerable<TResult>> Select<T,TResult>(this IEnumerable<T> collection, Func<T, Task<TResult>> select)
        {
            var list = new List<TResult>();
            foreach (var item in collection)
            {
                list.Add(await select(item));
            }
            return list;
        }

        /// <summary>
        /// Adds the result of a task to an <see cref="AsyncObservableCollection{T}"/>
        ///  </summary>
        public static Task Fill<T>(this Task<IEnumerable<T>> collection, AsyncObservableCollection<T> asyncCollection)
        {
            return asyncCollection.FillAsync(collection);
        }

        /// <summary>
        /// clears then adds the result of a task to an <see cref="AsyncObservableCollection{T}"/>
        ///  </summary>
        public static Task ClearAndFill<T>(this Task<IEnumerable<T>> collection, AsyncObservableCollection<T> asyncCollection)
        {
            return asyncCollection.ClearAndFillAsync(collection);
        }

        /// <summary>
        /// Converts  a collection to an <see cref="AsyncObservableCollection{T}"/>
        /// </summary>
        public static AsyncObservableCollection<T> ToAsyncObservableCollection<T>(this IEnumerable<T> collection)
        {
            return AsyncObservableCollection<T>.Create(collection);
        }

        /// <summary>
        /// Converts the result of a task to an <see cref="AsyncObservableCollection{T}"/>
        /// </summary>
        public static async Task<AsyncObservableCollection<T>> ToAsyncObservableCollection<T>(this Task<IEnumerable<T>> collection)
        {
            return AsyncObservableCollection<T>.Create(await collection);
        }

        /// <summary>
        /// Perform an action for each element in a task result.
        /// </summary>
        public static async Task<IEnumerable<T>> ForEach<T>(this Task<IEnumerable<T>> collection,Action<T> action)
        {
            return (await collection).ForEach(action);
        }
    }
}
