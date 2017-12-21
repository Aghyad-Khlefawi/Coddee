// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Coddee
{
    public class Diff<T, TKey> where T : IUniqueObject<TKey>
    {
        public IEnumerable<T> Added { get; set; }
        public IEnumerable<T> Deleted { get; set; }
        public IEnumerable<T> Edited { get; set; }
    }

    public class Diff
    {
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
