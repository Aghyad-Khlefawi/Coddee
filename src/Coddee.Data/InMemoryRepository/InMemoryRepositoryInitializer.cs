// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;

namespace Coddee.Data
{
    public class InMemoryRepositoryInitializer : IRepositoryInitializer
    {
        private readonly IObjectMapper _mapper;
        private readonly RepositoryConfigurations _config;

        public InMemoryRepositoryInitializer(IObjectMapper mapper, RepositoryConfigurations config)
        {
            _mapper = mapper;
            _config = config;
        }


        public int RepositoryType { get; } = (int) RepositoryTypes.InMemory;

        public void InitializeRepository(IRepositoryManager repositoryManager, IRepository repository, Type implementedInterface)
        {
            repository.Initialize(repositoryManager,_mapper,implementedInterface,_config);
        }
    }
}
