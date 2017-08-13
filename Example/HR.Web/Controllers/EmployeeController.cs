// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using System.Threading.Tasks;
using Coddee.AspTest.Controllers;
using Coddee.Data;
using Coddee.Loggers;
using HR.Data.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace HR.Web.Controllers
{
    public class EmployeeController : CRUDApiControllerBase<IEmployeeRepository, Data.Models.Employee, Guid>
    {
        public EmployeeController(IRepositoryManager repoManager, ILogger logger) : base(repoManager, logger)
        {

        }

        [HttpGet]
        public async Task<IActionResult> GetEmployeesByCompany([FromQuery]Guid companyID)
        {
            try
            {
                return Json(await _repository.GetEmployeesByCompany(companyID));
            }
            catch (Exception ex)
            {
                return Error(ex);
            }
        }
    }
}
