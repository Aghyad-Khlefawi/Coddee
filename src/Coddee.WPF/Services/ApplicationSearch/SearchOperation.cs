// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System.Collections.Generic;
using System.Threading;

namespace Coddee.Services.ApplicationSearch
{
    /// <summary>
    /// Search operation object.
    /// Contains the operation result and gives the ability to abort the operation.
    /// </summary>
    public class SearchOperation
    {

        /// <summary>
        /// The CancellationToken of the search tasks
        /// </summary>
        private readonly CancellationTokenSource _token;

        public IList<SearchItem> Result { get; set; }
        public bool IsCompleted { get; set; }

        public SearchOperation(CancellationTokenSource token)
        {
            _token = token;
        }

        /// <summary>
        /// Abort the search operation.
        /// </summary>
        public void Abort()
        {
            _token.Cancel();
        }
    }
}
