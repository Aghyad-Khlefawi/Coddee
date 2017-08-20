// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;

namespace Coddee.Data.MongoDB
{
   public class MongoRepositoryInitializer:IRepositoryInitializer
    {
        private readonly IMongoDBManager _dbManager;
        private readonly IObjectMapper _mapper;
        public int RepositoryType { get; } = (int)RepositoryTypes.Mongo;

        public MongoRepositoryInitializer(IMongoDBManager dbManager, IObjectMapper mapper)
        {
            _dbManager = dbManager;
            _mapper = mapper;
        }

        public void InitializeRepository(IRepositoryManager repositoryManager, IRepository repository, Type implementedInterface)
        {
            ((IMongoRepository)repository).Initialize(_dbManager,repositoryManager,_mapper,implementedInterface);
        }
    }
}
