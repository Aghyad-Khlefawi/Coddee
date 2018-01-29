// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;

namespace Coddee.Data.MongoDB
{
    /// <summary>
    /// Defines the requirements for a MongoDB repository
    /// </summary>
    public interface IMongoRepository:IRepository
    {
        /// <summary>
        /// Do any required initialization
        /// </summary>
        void Initialize(IMongoDBManager dbManager,
                        IRepositoryManager repositoryManager,
                        IObjectMapper mapper,
                        Type implementedInterface,
                        RepositoryConfigurations config = null);
    }
}
