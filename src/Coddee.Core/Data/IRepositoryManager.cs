﻿// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using System.Collections.Generic;
using Coddee.Attributes;

namespace Coddee.Data
{
    /// <summary>
    /// A container for the repositories, responsible for discovering, initializing and retrieving 
    /// the repositories
    /// </summary>
    public interface IRepositoryManager
    {

        /// <summary>
        /// Gets a repository by its class name of <see cref="NameAttribute"/>
        /// </summary>
        /// <returns></returns>
        IRepository GetRepository(string name);

        /// <summary>
        /// Gets a repository by its interface
        /// </summary>
        /// <typeparam name="TInterface"></typeparam>
        /// <returns></returns>
        TInterface GetRepository<TInterface>() where TInterface:IRepository;

        /// <summary>
        /// Returns all the registered repositories
        /// </summary>
        /// <returns></returns>
        IEnumerable<IRepository> GetRepositories();

        /// <summary>
        /// Discover and register the repository from assemblies
        /// </summary>
        /// <param name="assemblies">The assemblies to search for the repositories</param>
        /// <returns></returns>
        void RegisterRepositories(params string[] assemblies);
        
        

        /// <summary>
        /// Register the provided repositories
        /// </summary>
        /// <param name="repositories">The repositories to register</param>
        /// <returns></returns>
        void RegisterRepositories(params KeyValuePair<Type, Type>[] repositories);
        

        /// <summary>
        /// Add an already initialized repository
        /// </summary>
        void AddRepository(IRepository repository,Type implementedRepository);

        /// <summary>
        /// Set the sync service to be passed to all repositories.
        /// </summary>
        void SetSyncService(IRepositorySyncService syncService, bool sendSyncRequests = true);

        /// <summary>
        /// Adds a repository initializer to the repository manager.
        /// </summary>
        void AddRepositoryInitializer(IRepositoryInitializer initializer);

        /// <summary>
        /// Adds a repository initializer to the repository manager.
        /// </summary>
        IEnumerable<IRepositoryInitializer> GetRepositoryInitializers();

        /// <summary>
        /// Returns the name that identifies the repository.
        /// </summary>
        /// <param name="repository"></param>
        /// <returns></returns>
        string GetRepositoryName(IRepository repository);
    }


}
