﻿// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;

namespace Coddee.Data.MongoDB
{
    /// <summary>
    /// A repository initializer that should provide the MongoDB repositories with their dependencies to operate.
    /// </summary>
    public class MongoRepositoryInitializer:IRepositoryInitializer
    {
        private readonly IMongoDBManager _dbManager;
        private readonly IObjectMapper _mapper;
        private readonly RepositoryConfigurations _config;

        /// <inheritdoc />
        public int RepositoryType { get; } = (int)RepositoryTypes.Mongo;

        /// <inheritdoc />
        public MongoRepositoryInitializer(IMongoDBManager dbManager, IObjectMapper mapper,
                                          RepositoryConfigurations config = null)
        {
            _dbManager = dbManager;
            _mapper = mapper;
            _config = config;
        }

        /// <inheritdoc />
        public void InitializeRepository(IRepositoryManager repositoryManager, IRepository repository, Type implementedInterface)
        {
            ((IMongoRepository)repository).Initialize(_dbManager,repositoryManager,_mapper,implementedInterface, _config);
        }
    }
}
