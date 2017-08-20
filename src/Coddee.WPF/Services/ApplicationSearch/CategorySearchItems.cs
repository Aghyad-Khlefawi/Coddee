// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System.Collections.Concurrent;

namespace Coddee.Services.ApplicationSearch
{
    /// <summary>
    /// A category of searchable objects;
    /// </summary>
    class CategorySearchItems
    {
        public CategorySearchItems()
        {
            Items = new ConcurrentDictionary<string, SearchItem>();
        }

        public string Category { get; set; }
        public ConcurrentDictionary<string, SearchItem> Items { get; set; }
    }

}
