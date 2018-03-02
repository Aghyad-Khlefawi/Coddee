// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

namespace Coddee.Data.REST
{
    /// <summary>
    /// Common API actions used by the repositories
    /// </summary>
    public class ApiCommonActions
    {
#pragma warning disable 1591
        public const string InsertItem = nameof(InsertItem);
        public const string UpdateItem = nameof(UpdateItem);
        public const string GetItemsWithConditions = nameof(GetItemsWithConditions);
        public const string GetItems = nameof(GetItems);
        public const string GetItem = nameof(GetItem);
        public const string DeleteItem = nameof(DeleteItem);
        public const string DeleteItemByKey = nameof(DeleteItemByKey);
#pragma warning restore 1591
    }
}