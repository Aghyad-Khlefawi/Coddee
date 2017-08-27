// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Coddee.Data
{
    /// <summary>
    /// Base implementation for a data repository
    /// </summary>
    public abstract class RepositoryBase:IRepository
    {
        protected IObjectMapper _mapper;
        protected RepositoryConfigurations _config;
        protected IRepositoryManager _repositoryManager;
        protected IRepositorySyncService _syncService;

        public abstract int RepositoryType { get; }
        public bool Initialized { get; protected set; }
        public Type ImplementedInterface { get; protected set; }

        /// <summary>
        /// Do any required initialization
        /// </summary>
        public virtual void Initialize(IRepositoryManager repositoryManager, IObjectMapper mapper,Type implementedInterface,
        RepositoryConfigurations config = null)
        {
            _config = config;
            _repositoryManager = repositoryManager;
            ImplementedInterface = implementedInterface;
            _mapper = mapper;
            RegisterMappings(_mapper);
            Initialized = true;
        }

        public virtual void SetSyncService(IRepositorySyncService syncService)
        {
            _syncService = syncService;
            syncService.SyncReceived += SyncServiceSyncReceived;
        }
        
        /// <summary>
        /// Called whenever a sync call is received from the <see cref="IRepositorySyncService"/> 
        /// </summary>
        public virtual void SyncServiceSyncReceived(string identifier, RepositorySyncEventArgs args)
        {

        }

        /// <summary>
        /// Register any required additional mapping information
        /// </summary>
        /// <param name="mapper"></param>
        public virtual void RegisterMappings(IObjectMapper mapper)
        {

        }

       
    }

    public abstract class RepositoryBase<TModel> : RepositoryBase
    {
        public virtual Condition<TModel, T> Condition<T>(Expression<Func<TModel, T>> property, T value)
        {
            return new Condition<TModel, T>(property, value);
        }
    }
}