// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using Coddee.Data;
using Coddee.Data.Rest;
using HR.Data.Repositories;

namespace HR.Data.REST.Repositories
{
    [Repository(typeof(IStateRepository))]
    public class StateRepository : ReadOnlyRESTRepositoryBase<Models.State, int>, IStateRepository
    {
        public override string ControllerName => "State";
    }
}