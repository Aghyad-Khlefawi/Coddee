// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using System.Threading.Tasks;
using Coddee.AspNet.Controllers;
using Coddee.Data;
using Coddee.Loggers;
using HR.Data.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace HR.Web.Controllers
{
    public class CompanyController : CRUDApiControllerBase<ICompanyRepository, Data.Models.Company, Guid>
    {
        public CompanyController(IRepositoryManager repoManager, ILogger logger) : base(repoManager, logger)
        { 
        }

        [HttpGet]
        public async Task<IActionResult> GetDetailedItems()
        {
            try
            {
                return Json(await _repository.GetDetailedItems());
            }
            catch (Exception ex)
            {
                return Error(ex);
            }
        }
    }
}