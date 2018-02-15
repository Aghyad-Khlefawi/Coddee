// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;

namespace Coddee.Data
{
    /// <summary>
    /// Base implementation for a data repository
    /// </summary>
    public abstract class RepositoryBase:IRepository
    {

        /// <summary>
        /// The context object that provides more information to the repository about the action that is being executed.
        /// </summary>
        protected object _context;

        /// <summary>
        /// Property mapper 
        /// </summary>
        protected IObjectMapper _mapper;

        /// <summary>
        /// The repository configuration object
        /// <remarks>This field will can be null</remarks>
        /// </summary>
        protected RepositoryConfigurations _config;

        /// <summary>
        /// The repository manager that has created this repository
        /// </summary>
        protected IRepositoryManager _repositoryManager;


        /// <summary>
        /// Synchronization service.
        /// </summary>
        protected IRepositorySyncService _syncService;

        /// <summary>
        /// If this field is true then the repository should send sync request when the data is changed.
        /// </summary>
        protected bool _sendSyncRequests;

        /// <inheritdoc />
        public abstract int RepositoryType { get; }

        /// <inheritdoc />
        public bool Initialized { get; protected set; }

        /// <inheritdoc />
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

        /// <inheritdoc />
        public virtual void SetSyncService(IRepositorySyncService syncService,bool sendSyncRequests = true)
        {
            _syncService = syncService;
            _sendSyncRequests = sendSyncRequests;
            syncService.SyncReceived += SyncServiceSyncReceived;
        }

        /// <inheritdoc />
        public void SetContext(object context)
        {
            _context = context;
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

    /// <inheritdoc />
    public abstract class RepositoryBase<TModel> : RepositoryBase
    { 

    }
}