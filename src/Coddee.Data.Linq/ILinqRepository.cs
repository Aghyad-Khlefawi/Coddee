// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;

namespace Coddee.Data.LinqToSQL
{
    /// <summary>
    /// Defines the requirements for a LinqToSQL repository
    /// </summary>
    public interface ILinqRepository : IRepository 
    {
        /// <summary>
        /// Do any required initialization
        /// </summary>
        void Initialize(
            ILinqDBManager dbManager,
            IRepositoryManager repositoryManager,
            IObjectMapper mapper,
            Type implementedInterface,
            RepositoryConfigurations config = null);
    }
}