// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using Coddee.AspTest.Controllers;
using Coddee.Data;
using Coddee.Loggers;
using HR.Data.Repositories;

namespace HR.Web.Controllers
{
    public class CompanyController:CRUDApiControllerBase<ICompanyRepository,Data.Models.Company,Guid>
    {
        public CompanyController(IRepositoryManager repoManager, ILogger logger) : base(repoManager, logger)
        {
        }
    }
}
