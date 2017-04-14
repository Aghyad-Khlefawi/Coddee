// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Coddee.Data
{
    /// <summary>
    /// Base implementation for a repository manager
    /// </summary>
    public abstract class RepositoryManagerBase : IRepositoryManager
    {
        public virtual void Initialize(IObjectMapper mapper)
        {
            _repositories = new Dictionary<Type, IRepository>();
            _mapper = mapper;
        }

        protected IObjectMapper _mapper;
        protected Dictionary<Type, IRepository> _repositories;

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
            return (TInterface) _repositories[typeof(TInterface)];
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
                        .Where(e => e.GetCustomAttribute<RepositoryAttribute>() != null)
                        .Select(e => new KeyValuePair<Type, Type>(
                            e.GetCustomAttribute<RepositoryAttribute>().ImplementedRepository, e.AsType()))
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
                    var repo = (IRepository) Activator.CreateInstance(repository.Value);
                    InitializeRepository(repo,repository.Key);
                    _repositories.Add(repository.Key, repo);
                }
                else
                    throw new ArgumentException(
                        $"The type {repository.Value.FullName} doesn't implements 'ILinqRepository<TDataContext>' interface");
            }
        }

        public virtual void InitializeRepository(IRepository repo,Type implementedInterface)
        {
            repo.Initialize(this, _mapper, implementedInterface);
        }
    }
}