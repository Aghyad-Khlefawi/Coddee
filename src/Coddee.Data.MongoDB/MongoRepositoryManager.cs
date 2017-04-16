// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;

namespace Coddee.Data.MongoDB
{

    public interface IMongoRepositoryManager : IRepositoryManager
    {
        void Initialize(IMongoDBManager dbManager, IObjectMapper mapper);
    }

    public class MongoRepositoryManager : RepositoryManagerBase, IMongoRepositoryManager
    {
        private IMongoDBManager _dbManager;

        public override void InitializeRepository(IRepository repo, Type implementedInterface)
        {
            ((IMongoRepository) repo).Initialize(_dbManager,
                                                 this,
                                                 _mapper,
                                                 implementedInterface);
        }

        public void Initialize(IMongoDBManager dbManager, IObjectMapper mapper)
        {
            _dbManager = dbManager;
            Initialize(mapper);
        }
    }
}