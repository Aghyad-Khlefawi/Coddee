// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using System.Data.Linq;

namespace Coddee.Data.LinqToSQL
{
    public class LinqRepositoryInitializer : IRepositoryInitializer 
    {
        private readonly ILinqDBManager _dbManager;
        private readonly IObjectMapper _mapper;
        private readonly RepositoryConfigurations _config;

        public LinqRepositoryInitializer(ILinqDBManager dbManager,
                                         IObjectMapper mapper,
                                         RepositoryConfigurations config = null)
        {
            _dbManager = dbManager;
            _mapper = mapper;
            _config = config;
        }

        public void InitializeRepository(IRepositoryManager repositoryManager, IRepository repository, Type implementedInterface)
        {
            ((ILinqRepository)repository).Initialize(_dbManager, repositoryManager, _mapper, implementedInterface, _config);
        }
    }
}
