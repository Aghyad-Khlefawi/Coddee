// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Coddee
{
    /// <summary>
    /// The result of Diff operation
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TKey"></typeparam>
    public class Diff<T, TKey> where T : IUniqueObject<TKey>
    {
        /// <summary>
        /// The items added to the collection
        /// </summary>
        public IEnumerable<T> Added { get; set; }


        /// <summary>
        /// The items deleted from the collection
        /// </summary>
        public IEnumerable<T> Deleted { get; set; }

        /// <summary>
        /// The items edited in the collection
        /// </summary>
        public IEnumerable<T> Edited { get; set; }
    }

    /// <summary>
    /// A static class that provides the functionality of comparing objects changes.
    /// </summary>
    public static class Diff
    {
        /// <summary>
        /// Compare to collections changes.
        /// </summary>
        public static Diff<T, TKey> DiffCollections<T, TKey>(IEnumerable<T> original, IEnumerable<T> edited) where T : IUniqueObject<TKey>
        {
            var editedItems = new List<T>();
            var common = edited.Where(e => original.Any(o => o.GetKey.Equals(e.GetKey)));
            foreach (var editedItem in common)
            {
                var originalItem = original.First(e => e.GetKey.Equals(editedItem.GetKey));
                if (IsItemEdited<T, TKey>(originalItem, editedItem))
                {
                    editedItems.Add(editedItem);
                }
            }
            return new Diff<T, TKey>
            {
                Deleted = original.Where(e => edited.All(o => !o.GetKey.Equals(e.GetKey))).ToList(),
                Added = edited.Where(e => original.All(o => !o.GetKey.Equals(e.GetKey))).ToList(),
                Edited = editedItems
            };
        }

        private static bool IsItemEdited<T, TKey>(T original, T editedItem) where T : IUniqueObject<TKey>
        {
            if (original == null && editedItem == null)
                return false;
            var props = original.GetType().GetTypeInfo().GetProperties();
            foreach (var propertyInfo in props)
            {
                var originalVal = propertyInfo.GetValue(original);
                var editVal = propertyInfo.GetValue(editedItem);
                if (originalVal == null && editVal == null)
                    continue;
                if (originalVal == null)
                    return true;
                if (!originalVal.Equals(editVal))
                    return true;
            }
            return false;
        }
    }
}
