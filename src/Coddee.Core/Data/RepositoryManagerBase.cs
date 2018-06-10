// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Coddee.Attributes;

namespace Coddee.Data
{
    /// <summary>
    /// Base class for RepositoryManagers
    /// </summary>
    public abstract class RepositoryManagerBase : IRepositoryManager
    {
        /// <inheritdoc />
        protected RepositoryManagerBase()
        {
            _repositoryInitializers = new Dictionary<int, IRepositoryInitializer>();

        }

        /// <summary>
        /// Repositories Sync service
        /// </summary>
        protected IRepositorySyncService _syncService;

        /// <summary>
        /// When set to true the repositories will send sync requests to the sync service.
        /// </summary>
        protected bool _sendSyncRequests;

        /// <summary>
        /// Checks if the repository type has the name.
        /// </summary>
        /// <returns></returns>
        protected bool CheckRepositoryName(Type repositoryType, string name)
        {
            var reportAttr = repositoryType.GetTypeInfo().GetCustomAttribute<RepositoryAttribute>();
            var interType = reportAttr.ImplementedRepository;
            var nameAttrs = interType.GetTypeInfo().GetCustomAttributes<NameAttribute>();
            foreach (var nameAttr in nameAttrs)
            {
                if (nameAttr != null && nameAttr.Value.Equals(name, StringComparison.OrdinalIgnoreCase))
                    return true;
            }

            var repoName = interType.Name;
            repoName = repoName.Remove(0, 1);
            if (repoName.EndsWith("Repository"))
                repoName = repoName.Substring(0, repoName.Length - 10);

            if (repoName.Equals(name, StringComparison.OrdinalIgnoreCase))
                return true;
            return false;
        }

        /// <summary>
        /// The repository initializers supported by this repository manager
        /// </summary>
        protected Dictionary<int, IRepositoryInitializer> _repositoryInitializers;

        /// <inheritdoc />
        public abstract IRepository GetRepository(string name);

        /// <inheritdoc />
        public abstract TInterface GetRepository<TInterface>() where TInterface : IRepository;
        /// <inheritdoc />
        public abstract IEnumerable<IRepository> GetRepositories();

        /// <inheritdoc />
        public virtual void RegisterRepositories(params string[] assemblies)
        {
            foreach (var assembly in assemblies)
            {
                var repositories =
                    Assembly.Load(new AssemblyName(assembly))
                            .DefinedTypes
                            .Where(e => CustomAttributeExtensions.GetCustomAttribute<RepositoryAttribute>((MemberInfo) e) != null && CustomAttributeExtensions.GetCustomAttribute<RepositoryAttribute>((MemberInfo) e).Discoverable)
                            .Select(e => new KeyValuePair<Type, Type>(e.GetCustomAttribute<RepositoryAttribute>().ImplementedRepository, e.AsType()))
                            .ToArray();
                RegisterRepositories(repositories);
            }
        }

        /// <inheritdoc />
        public abstract void RegisterRepositories(params KeyValuePair<Type, Type>[] repositories);
        /// <inheritdoc />
        public abstract void AddRepository(IRepository repository, Type implementedRepository);

        /// <inheritdoc />
        public virtual void SetSyncService(IRepositorySyncService syncService, bool sendSyncRequests = true)
        {
            _syncService = syncService;
        }


        /// <inheritdoc />
        public IEnumerable<IRepositoryInitializer> GetRepositoryInitializers()
        {
            return _repositoryInitializers.Values.ToList();
        }

        /// <inheritdoc />
        public string GetRepositoryName(IRepository repository)
        {
            var type = repository.ImplementedInterface.GetTypeInfo();
            var nameAttrs = type.GetCustomAttribute<NameAttribute>();
            if (nameAttrs != null)
                return nameAttrs.Value;

            var repoName = type.Name;
            repoName = repoName.Remove(0, 1);
            if (repoName.EndsWith("Repository"))
                repoName = repoName.Substring(0, repoName.Length - 10);

            return repoName;
        }

        /// <inheritdoc />
        public void AddRepositoryInitializer(IRepositoryInitializer initializer)
        {
            _repositoryInitializers[initializer.RepositoryType] = initializer;
        }


        /// <summary>
        /// Calls the <see cref="IRepository.Initialize"/> method for a repository
        /// </summary>
        public void InitializeRepository(IRepository repo, Type implementedInterface)
        {
            if (!_repositoryInitializers.ContainsKey(repo.RepositoryType))
                throw new InvalidOperationException($"No initializer found for repository '{repo.GetType().FullName}'");

            _repositoryInitializers[repo.RepositoryType].InitializeRepository(this, repo, implementedInterface);
        }
    }
}