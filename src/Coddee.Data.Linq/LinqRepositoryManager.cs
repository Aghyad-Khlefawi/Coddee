// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using System.Data.Linq;

namespace Coddee.Data.LinqToSQL
{
    public interface ILinqRepositoryManager: IRepositoryManager
    {
        void Initialize(ILinqDBManager dbManager,IObjectMapper mapper);
    }

    /// <summary>
    /// Base implementation for a LiqnToSQL repository manager
    /// </summary>
    /// <typeparam name="TDataContext"></typeparam>
    public class LinqRepositoryManager<TDataContext> : RepositoryManagerBase, ILinqRepositoryManager
        where TDataContext : DataContext
    {
        protected  LinqDBManager<TDataContext> _dbManager;
        
        public override void InitializeRepository(IRepository repo, Type implementedInterface)
        {
            ((ILinqRepository<TDataContext>) repo).Initialize(_dbManager, this, _mapper, implementedInterface);
        }

        public void Initialize(ILinqDBManager dbManager, IObjectMapper mapper)
        {
            _dbManager = (LinqDBManager<TDataContext>) dbManager;
            Initialize(mapper);
        }
    }
}