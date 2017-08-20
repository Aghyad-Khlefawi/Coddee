// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Coddee.Services.ApplicationSearch
{
    public class ApplicationSearchService:IApplicationSearchService
    {
        public ApplicationSearchService()
        {
            _searchCategories = new ConcurrentBag<CategorySearchItems>();
            _indexedItems = new List<SearchItem>();
        }

        private readonly List<SearchItem> _indexedItems;
        private readonly ConcurrentBag<CategorySearchItems> _searchCategories;

        public event EventHandler<IEnumerable<SearchItem>> ItemsIndexed;

        public IEnumerable<SearchItem> GetIndexedItems()
        {
            return _indexedItems.AsReadOnly();
        }

        public IEnumerable<SearchItem> IndexItems(string categoryTitle, IEnumerable<SearchItem> items)
        {
            var category = _searchCategories.FirstOrDefault(e => e.Category == categoryTitle);
            if (category == null)
            {
                category = new CategorySearchItems { Category = categoryTitle };
                _searchCategories.Add(category);
            }

            foreach (var item in items)
            {
                category.Items.TryAdd(item.SearchField.ToLower().Replace(" ", ""), item);
                _indexedItems.Add(item);
            }
            ItemsIndexed?.Invoke(this, items);
            return items;
        }


        public SearchOperation Search(string term, Action<SearchItem> resultFoundCallback, Action<IList<SearchItem>> SearchCompleted)
        {
            var token = new CancellationTokenSource();
            var operation = new SearchOperation(token);
            var result = new List<SearchItem>();
            operation.Result = result;

            term = term.ToLower().Replace(" ", "");

            Task.WhenAll(_searchCategories.Select(category =>
                                                      Task.Factory.StartNew(() => Search(category), token.Token)))
                .ContinueWith(t =>
                {
                    operation.IsCompleted = true;
                    SearchCompleted?.Invoke(result);
                }, token.Token);

            void Search(CategorySearchItems category)
            {
                SearchItem foundItem = null;
                if (category.Items.ContainsKey(term))
                {
                    foundItem = category.Items[term];
                    ResultFound(category.Items[term]);
                }
                foreach (var categoryItem in category.Items)
                {
                    if (categoryItem.Key.Contains(term))
                    {
                        if (foundItem != categoryItem.Value)
                            ResultFound(categoryItem.Value);
                    }
                }
            }

            void ResultFound(SearchItem resultItem)
            {
                resultFoundCallback(resultItem);
                result.Add(resultItem);
            }
            return operation;
        }
    }
}
