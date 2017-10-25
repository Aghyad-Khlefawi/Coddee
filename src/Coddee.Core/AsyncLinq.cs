
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
        public static AsyncObservableCollection<T> ToAsyncObservableCollection<T>(this IEnumerable<T> collection)
        {
            return AsyncObservableCollection<T>.Create(collection);
        }

        public static async Task<AsyncObservableCollection<T>> ToAsyncObservableCollection<T>(this Task<IEnumerable<T>> collection)
        {
            return AsyncObservableCollection<T>.Create(await collection);
        }

    }
}
