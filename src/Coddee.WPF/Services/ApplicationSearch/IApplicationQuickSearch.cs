// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using Coddee.Mvvm;

namespace Coddee.Services.ApplicationSearch
{
    /// <summary>
    /// Provides a visual element for the <see cref="IApplicationSearchService"/>
    /// </summary>
    public interface IApplicationQuickSearch : IPresentableViewModel
    {
        /// <summary>
        /// Sets the focus on the element.
        /// </summary>
        void Focus();
    }
}
