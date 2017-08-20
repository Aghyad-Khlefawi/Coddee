// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Coddee.Data
{
    /// <summary>
    /// Base implementation for a repository manager
    /// </summary>
    public class RepositoryManager : IRepositoryManager
    {

        public RepositoryManager()
        {
            _repositories = new Dictionary<Type, IRepository>();
            _repositoryInitializers = new Dictionary<int, IRepositoryInitializer>();
        }
        
        private IRepositorySyncService _syncService;
        protected Dictionary<Type, IRepository> _repositories;
        protected Dictionary<int, IRepositoryInitializer> _repositoryInitializers;


        /// <summary>
        /// Gets a repository by its interface
        /// </summary>
        /// <typeparam name="TInterface"></typeparam>
        /// <returns></returns>
        public virtual TInterface GetRepository<TInterface>() where TInterface : IRepository
        {
            if (!_repositories.ContainsKey(typeof(TInterface)))
                throw new ArgumentException(
                    $"Repository of type {typeof(TInterface).Name} was not found, make sure that the repository is registered and marked with the RepositoryAttribute");
            return (TInterface)_repositories[typeof(TInterface)];
        }

        /// <summary>
        /// Returns all the registered repositories
        /// </summary>
        /// <returns></returns>
        public virtual IEnumerable<IRepository> GetRepositories()
        {
            return _repositories.Values;
        }

        /// <summary>
        /// Discover and register the repository from assemblies
        /// </summary>
        /// <param name="assemblies">The assemblies to search for the repositories</param>
        /// <returns></returns>
        public virtual void RegisterRepositories(params string[] assemblies)
        {
            foreach (var assembly in assemblies)
            {
                var repositories =
                    Assembly.Load(new AssemblyName(assembly))
                        .DefinedTypes
                        .Where(e => e.GetCustomAttribute<RepositoryAttribute>() != null && e.GetCustomAttribute<RepositoryAttribute>().Discoverable)
                        .Select(e => new KeyValuePair<Type, Type>(e.GetCustomAttribute<RepositoryAttribute>().ImplementedRepository, e.AsType()))
                        .ToArray();
                RegisterRepositories(repositories);
            }
        }

        /// <summary>
        /// Register the provided repositories
        /// </summary>
        /// <param name="repositories">The repositories to register</param>
        /// <returns></returns>
        public virtual void RegisterRepositories(params KeyValuePair<Type, Type>[] repositories)
        {
            foreach (var repository in repositories)
            {
                if (repository.Value.GetTypeInfo().ImplementedInterfaces.Any(e => e == typeof(IRepository)))
                {
                    if (repository.Value.GetTypeInfo().ImplementedInterfaces.All(e => e != repository.Key))
                        throw new ArgumentException(
                            $"The type {repository.Value.FullName} doesn't implements '{repository.Key.FullName}' interface");
                    var repo = (IRepository)Activator.CreateInstance(repository.Value);
                    InitializeRepository(repo, repository.Key);
                    AddRepository(repo, repository.Key);
                }
                else
                    throw new ArgumentException(
                        $"The type {repository.Value.FullName} doesn't implements 'ILinqRepository<TDataContext>' interface");
            }
        }

        public void AddRepository(IRepository repository, Type implementedRepository)
        {
            _repositories[implementedRepository] = repository;
            if (_syncService != null)
                repository.SetSyncService(_syncService);
        }

        public void SetSyncService(IRepositorySyncService syncService)
        {
            _syncService = syncService;
            foreach (var repo in _repositories.Values)
            {
                repo.SetSyncService(_syncService);
            }
        }

        public virtual void InitializeRepository(IRepository repo, Type implementedInterface)
        {
            if (!_repositoryInitializers.ContainsKey(repo.RepositoryType))
                throw new InvalidOperationException($"No initializer found for repository '{repo.GetType().FullName}'");

            _repositoryInitializers[repo.RepositoryType].InitializeRepository(this, repo, implementedInterface);
        }

        public void AddRepositoryInitializer(IRepositoryInitializer initializer)
        {
            _repositoryInitializers[initializer.RepositoryType] = initializer;
        }
    }
}