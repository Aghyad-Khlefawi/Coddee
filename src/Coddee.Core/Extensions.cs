// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coddee
{
    /// <summary>
    /// Static extension methods class.
    /// </summary>
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
                if (ch <= 0x7f)
                    retval[ix] = (byte)ch;
                else
                    retval[ix] = (byte)'?';
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
            var item = collection.FirstOrDefault<T>(pred);
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
        /// Removes item from collection and insert the new item
        /// The old item is removed based on IEquality interface
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        /// <param name="item"></param>
        public static void UpdateFirst<T>(this IList<T> collection, T item)
        {
            collection.UpdateFirst(item, e => e.Equals(item));
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
        /// Updates the collection base on the operation type 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public static void Update<T>(this IList<T> collection, OperationType operationType, T item, Func<T, bool> predicate)
        {
            if (operationType == OperationType.Add)
                collection.Add(item);
            else if (operationType == OperationType.Edit)
                collection.Update(item, predicate);
            else if (operationType == OperationType.Delete)
                collection.Remove(predicate);
        }

        /// <summary>
        /// Updates the collection base on the operation type 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public static void UpdateFirst<T>(this IList<T> collection, OperationType operationType, T item)
        {
            if (operationType == OperationType.Add)
                collection.Insert(0, item);
            else if (operationType == OperationType.Edit)
                collection.UpdateFirst(item);
            else if (operationType == OperationType.Delete)
                collection.Remove(item);
        }

        /// <summary>
        /// Updates the collection base on the operation type 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public static void UpdateFirst<T>(this IList<T> collection, OperationType operationType, T item, Func<T, bool> predicate)
        {
            if (operationType == OperationType.Add)
                collection.Insert(0, item);
            else if (operationType == OperationType.Edit)
                collection.UpdateFirst(item, predicate);
            else if (operationType == OperationType.Delete)
                collection.Remove(predicate);
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
            var oldItem = collection.FirstOrDefault<T>(predicate);
            if (oldItem != null)
            {
                var oldIndex = collection.IndexOf(oldItem);
                collection.Remove(oldItem);
                collection.Insert(oldIndex, item);
            }
        }

        /// <summary>
        /// Removes item from collection and insert the new item
        /// The old item is removed based on the predicate
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        /// <param name="item"></param>
        /// <param name="predicate"></param>
        public static void UpdateFirst<T>(this IList<T> collection, T item, Func<T, bool> predicate)
        {
            var oldItem = collection.FirstOrDefault<T>(predicate);
            if (oldItem != null)
            {
                collection.Remove(oldItem);
                collection.Insert(0, item);
            }
        }

        /// <summary>
        /// Returns the first object that matches the key base on the <see cref="IUniqueObject{TKey}"/> interface.
        /// </summary>
        public static T First<T, TKey>(this IEnumerable<T> collection, TKey key) where T : IUniqueObject<TKey>
        {
            return collection.First(e => e.GetKey.Equals(key));
        }

        /// <summary>
        /// Casts the elements of a collection to another type.
        /// </summary>
        public static IEnumerable<TResult> CastAs<TResult>(this IEnumerable collection)
        {
            foreach (var item in collection)
            {
                yield return (TResult)item;
            }
        }

        /// <summary>
        /// Casts the elements of a collection to another type.
        /// </summary>
        public static async Task<IEnumerable<TResult>> CastAs<TSource, TResult>(this Task<IEnumerable<TSource>> collection)
        {
            return (await collection).CastAs<TResult>();
        }

        /// <summary>
        /// Returns the first object that matches the key base on the <see cref="IUniqueObject{TKey}"/> interface.
        /// </summary>
        public static T FirstOrDefault<T, TKey>(this IEnumerable<T> collection, TKey key) where T : IUniqueObject<TKey>
        {
            return collection.FirstOrDefault(e => e.GetKey.Equals(key));
        }

        /// <summary>
        /// Removes the first item of the collection that matches the predicate.
        /// </summary>
        public static void Remove<T>(this IList<T> collection, Func<T, bool> predicate)
        {
            collection.Remove(collection.First<T>(predicate));
        }

        /// <summary>
        /// Removes the first item of the collection that matches the predicate.
        /// </summary>
        public static void RemoveIfExists<T>(this IList<T> collection, Func<T, bool> predicate)
        {
            var item = collection.FirstOrDefault<T>(predicate);
            if (item != null)
                collection.Remove(item);
        }

        /// <summary>
        /// Removes the first item of the collection that matches the argument.
        /// </summary>
        public static void RemoveIfExists<T>(this IList<T> collection, T item)
        {
            if (collection.Contains(item))
                collection.Remove(item);
        }

        /// <summary>
        /// Waits for a collection of tasks to complete.
        /// </summary>
        public static Task WhenAll(this IEnumerable<Task> tasks)
        {
            return Task.WhenAll(tasks);
        }

        /// <summary>
        /// Creates a continuation that executes asynchronously when the task completes.
        /// </summary>
        public static Task ContinueWithResult<T>(this Task<T> task, Action<T> action)
        {
            return task.ContinueWith(t => action(t.Result));
        }

        /// <summary>
        /// Creates a continuation that executes asynchronously when the task completes.
        /// </summary>
        public static Task ContinueWithResultAs<T, TResult>(this Task<IEnumerable<T>> task, Action<IEnumerable<TResult>> action)
        {
            return task.ContinueWith(t => action(t.Result.CastAs<TResult>()));
        }

        /// <summary>
        /// Combines a collection of strings to one string.
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="seperator"></param>
        /// <returns></returns>
        public static string Combine(this IEnumerable<string> collection, string seperator)
        {
            if (!collection.Any())
                return null;

            var sb = new StringBuilder();
            foreach (var item in collection)
            {
                sb.Append($"{item}{seperator}");
            }
            return sb.ToString(0, sb.Length - seperator.Length);
        }

        /// <summary>
        /// Build a string that represent the exception object
        /// </summary>
        /// <param name="exception">The exception object</param>
        /// <param name="level">The exception depth for spacing inner exception</param>
        /// <param name="debuginfo">Show debug information (Source and stack trace)</param>
        /// <returns></returns>
        public static string BuildExceptionString(this Exception exception, int level = 0, bool debuginfo = false)
        {
            var execptionInfoBuilder = new StringBuilder();
            var append = "";
            if (level != 0)
                for (int i = 0; i < level + 1; i++)
                {
                    append += "\t";
                }
            execptionInfoBuilder.Append($"\n{append}\tException Type : {exception.GetType().Name}");
            execptionInfoBuilder.Append($"\n{append}\tDetails: {exception.Message}");
            if (debuginfo)
            {
                execptionInfoBuilder.Append($"\n{append}\tSource: {exception.Source}");
                execptionInfoBuilder.Append($"\n{append}\tTrace: {exception.StackTrace}");
            }
            execptionInfoBuilder.Append("\n");
            if (exception.InnerException != null)
                execptionInfoBuilder.Append(exception.InnerException.BuildExceptionString(level + 1, debuginfo));
            return execptionInfoBuilder.ToString();
        }
    }
}