// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System.Threading.Tasks;

namespace Coddee.Services.ApplicationSearch
{
    /// <summary>
    /// A module that provide the <see cref="IApplicationSearchService"/> services.
    /// </summary>
    [Module(nameof(ApplicationSearchModule))]
    public class ApplicationSearchModule : IModule
    {
        /// <inheritdoc/>
        public Task Initialize(IContainer container)
        {
            container.RegisterInstance<IApplicationSearchService, ApplicationSearchService>();
            container.RegisterType<IApplicationQuickSearch, SearchViewModel>();
            return Task.FromResult(true);
        }
    }
}
