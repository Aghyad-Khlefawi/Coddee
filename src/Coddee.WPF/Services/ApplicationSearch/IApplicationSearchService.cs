// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using System.Collections.Generic;

namespace Coddee.Services.ApplicationSearch
{
    public interface IApplicationSearchService
    {
        event EventHandler<IEnumerable<SearchItem>> ItemsIndexed;
        IEnumerable<SearchItem> GetIndexedItems();

        /// <summary>
        /// Add items that can be searched
        /// </summary>
        /// <param name="category">The category of the items.</param>
        /// <param name="items">The searchable items.</param>
        IEnumerable<SearchItem> IndexItems(string category, IEnumerable<SearchItem> items);

        /// <summary>
        /// Search the available items.
        /// </summary>
        /// <param name="term">Search term.</param>
        /// <param name="resultFoundCallback">This action will be called when an item is found.</param>
        /// <param name="SearchCompleted">This action will be called when the search operation is completed.</param>
        /// <returns>SearchOpeartion object.</returns>
        SearchOperation Search(string term, Action<SearchItem> resultFoundCallback, Action<IList<SearchItem>> SearchCompleted);
    }
}
