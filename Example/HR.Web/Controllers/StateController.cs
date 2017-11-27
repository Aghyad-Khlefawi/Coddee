// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using Coddee.AspNet.Controllers;
using Coddee.Data;
using Coddee.Loggers;
using HR.Data.Repositories;

namespace HR.Web.Controllers
{
    public class StateController:ReadOnlyApiControllerBase<IStateRepository, Data.Models.State,int>
    {
        public StateController(IRepositoryManager repoManager, ILogger logger) : base(repoManager, logger)
        {

        }
    }
}
