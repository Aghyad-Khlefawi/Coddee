// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using System.Net.Http;

namespace Coddee.Data.Rest
{
    public interface IRESTRepositoryManager : IRepositoryManager
    {
        void Initialize(string apiBaseURL, Action unauthorizedRequestHandler, IObjectMapper mapper);
    }
    /// <summary>
    /// Base implementation for a LiqnToSQL repository manager
    /// </summary>
    public class RESTRepositoryManager: RepositoryManagerBase, IRESTRepositoryManager
    {
        protected HttpClient _client;
        private Action _unauthorizedRequestHandler;
        public override void InitializeRepository(IRepository repo, Type implementedInterface)
        {
            ((IRESTRepository)repo).Initialize(_client, _unauthorizedRequestHandler, this, _mapper, implementedInterface);
        }

       
        public void Initialize(string apiBaseURL, Action unauthorizedRequestHandler, IObjectMapper mapper)
        {
            _client = new HttpClient {BaseAddress = new Uri(apiBaseURL, UriKind.Absolute)};
            _unauthorizedRequestHandler = unauthorizedRequestHandler;
            Initialize(mapper);
        }
    }
}