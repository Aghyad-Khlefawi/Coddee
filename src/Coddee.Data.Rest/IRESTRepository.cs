// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using System.Net.Http;

namespace Coddee.Data.Rest
{
    /// <summary>
    /// Defines the requirements for a repository that uses a REST api
    /// </summary>
    public interface IRESTRepository
    {
        void Initialize(HttpClient httpClient,
                        Action unauthorizedRequestHandler,
                        IRepositoryManager repositoryManager,
                        IObjectMapper mapper,
                        Type implementedInterface);
    }
}