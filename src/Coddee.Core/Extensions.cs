// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Coddee.Collections;


namespace Coddee
{
    public static class Extensions
    {
        /// <summary>
        /// Converts a string to an ASCII string represented in a byte array
        /// </summary>
        /// <param name="s">The string to convert</param>
        /// <returns>ASCII byte array</returns>
        public static byte[] ConvertToASCII(this string s)
        {
            byte[] retval = new byte[s.Length];
            for (int ix = 0; ix < s.Length; ++ix)
            {
                char ch = s[ix];
                if (ch <= 0x7f) retval[ix] = (byte)ch;
                else retval[ix] = (byte)'?';
            }
            return retval;
        }

        /// <summary>
        /// Converts a bytes array to Base64String format
        /// </summary>
        /// <param name="bytes">The bytes array</param>
        /// <returns>a Base64String</returns>
        public static string ConvertToString64(this byte[] bytes)
        {
            return Convert.ToBase64String(bytes);
        }

        /// <summary>
        /// Converts a normal string to Base64String format using UTF8 encoding
        /// </summary>
        /// <param name="text">The string to convert</param>
        /// <returns>a Base64String</returns>
        public static string ConvertToString64(this string text)
        {
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(text));
        }


        /// <summary>
        /// Executes an action on a collection of items
        /// </summary>
        public static IEnumerable<T> ForEach<T>(this IEnumerable<T> list, Action<T> action)
        {
            var items = list.ToList();
            for (int i = 0; i <= items.Count - 1; i++)
            {
                action(items[i]);
            }
            return items;
        }

        /// <summary>
        /// Removes item from collection
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public static void Remove<T>(this ICollection<T> collection, Func<T, bool> pred)
        {
            var item = collection.FirstOrDefault(pred);
            if (item != null)
                collection.Remove(item);
        }

        /// <summary>
        /// Removes item from collection and insert the new item
        /// The old item is removed based on IEquality interface
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        /// <param name="item"></param>
        public static void Update<T>(this IList<T> collection, T item)
        {
            collection.Update(item, e => e.Equals(item));
        }

        /// <summary>
        /// Updates the collection base on the operation type 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public static void Update<T>(this IList<T> collection, OperationType operationType, T item)
        {
            if (operationType == OperationType.Add)
                collection.Add(item);
            else if (operationType == OperationType.Edit)
                collection.Update(item);
            else if (operationType == OperationType.Delete)
                collection.Remove(item);
        }


        /// <summary>
        /// Removes item from collection and insert the new item
        /// The old item is removed based on the predicate
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        /// <param name="item"></param>
        /// <param name="predicate"></param>
        public static void Update<T>(this IList<T> collection, T item, Func<T, bool> predicate)
        {
            var oldItem = collection.FirstOrDefault(predicate);
            if (oldItem != null)
            {
                var oldIndex = collection.IndexOf(oldItem);
                collection.Remove(oldItem);
                collection.Insert(oldIndex, item);
            }
        }

        public static AsyncObservableCollection<T> ToAsyncObservableCollection<T>(this IEnumerable<T> collection)
        {
            return AsyncObservableCollection<T>.Create(collection); 
        }

        public static async Task<AsyncObservableCollection<T>> ToAsyncObservableCollection<T>(this Task<IEnumerable<T>> collection)
        {
            return AsyncObservableCollection<T>.Create(await collection);
        }

        public static Task WhenAll(this IEnumerable<Task> tasks)
        {
            return Task.WhenAll(tasks);
        }

        public static Task ContinueWithResult<T>(this Task<T> task,Action<T> action)
        {
            return task.ContinueWith(t => action(t.Result));
        }
    }
}