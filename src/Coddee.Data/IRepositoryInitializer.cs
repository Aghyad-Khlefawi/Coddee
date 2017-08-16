// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;

namespace Coddee.Data
{
    public interface IRepositoryInitializer
    {
        void InitializeRepository(IRepositoryManager repositoryManager, IRepository repository, Type implementedInterface);
    }
}
