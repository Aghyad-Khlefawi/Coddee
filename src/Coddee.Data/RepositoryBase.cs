// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;

namespace Coddee.Data
{
    /// <summary>
    /// Base implementation for a data repository
    /// </summary>
    public class RepositoryBase:IRepository
    {
        protected IObjectMapper _mapper;
        protected RepositoryConfigurations _config;
        protected IRepositoryManager _repositoryManager;

        public bool Initialized { get; protected set; }
        public Type ImplementedInterface { get; protected set; }

        /// <summary>
        /// Do any required initialization
        /// </summary>
        public void Initialize(IRepositoryManager repositoryManager, IObjectMapper mapper,Type implementedInterface,
        RepositoryConfigurations config = null)
        {
            _config = config;
            _repositoryManager = repositoryManager;
            ImplementedInterface = implementedInterface;
            _mapper = mapper;
            RegisterMappings(_mapper);
            Initialized = true;
        }

        /// <summary>
        /// Register any required additional mapping information
        /// </summary>
        /// <param name="mapper"></param>
        public virtual void RegisterMappings(IObjectMapper mapper)
        {

        }
    }
}