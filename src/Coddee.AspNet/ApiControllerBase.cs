// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using System.Threading.Tasks;
using Coddee.Data;
using Coddee.Loggers;
using Microsoft.AspNetCore.Mvc;

namespace Coddee.AspTest.Controllers
{
    public class ApiControllerBase : Controller
    {
        protected string EventsSource = "ApiControllerBase";

        public ApiControllerBase(IRepositoryManager repoManager, ILogger logger)
        {
            _repositoryManager = repoManager;
            _logger = logger;
        }

        protected readonly ILogger _logger;
        protected readonly IRepositoryManager _repositoryManager;
    }

    public class ApiControllerBase<TRepository> : ApiControllerBase where TRepository : IRepository
    {
        protected TRepository _repository;

        public ApiControllerBase(IRepositoryManager repoManager, ILogger logger) : base(repoManager, logger)
        {
            _repository = _repositoryManager.GetRepository<TRepository>();
        }

        public IActionResult Error(Exception ex)
        {
            _logger?.Log(EventsSource, ex);
            return BadRequest("An error occurred.");
        }
    }

    public class ReadOnlyApiControllerBase<TRepository, TModel, TKey> : ApiControllerBase<TRepository>
        where TRepository : IReadOnlyRepository<TModel, TKey>
    {
        public ReadOnlyApiControllerBase(IRepositoryManager repoManager, ILogger logger) : base(repoManager, logger)
        {
        }
        [HttpGet]
        public virtual async Task<IActionResult> GetItem(TKey id)
        {
            try
            {
                return Json(await _repository[id]);
            }
            catch (Exception ex)
            {
                return Error(ex);
            }
        }
        [HttpGet]
        public virtual async Task<IActionResult> GetItems()
        {
            try
            {
                return Json(await _repository.GetItems());
            }
            catch (Exception ex)
            {
                return Error(ex);
            }
        }
    }
    public class CRUDApiControllerBase<TRepository, TModel, TKey> : ReadOnlyApiControllerBase<TRepository, TModel, TKey>
        where TRepository : ICRUDRepository<TModel, TKey>
    {


        public CRUDApiControllerBase(IRepositoryManager repoManager, ILogger logger) : base(repoManager, logger)
        {

        }


        [HttpPost]
        public virtual async Task<IActionResult> InsertItem([FromBody] TModel item)
        {
            try
            {
                return Json(await _repository.InsertItem(item));
            }
            catch (Exception ex)
            {
                return Error(ex);
            }
        }

        [HttpPut]
        public virtual async Task<IActionResult> UpdateItem([FromBody] TModel item)
        {
            try
            {
                return Json(await _repository.UpdateItem(item));
            }
            catch (Exception ex)
            {
                return Error(ex);
            }
        }


        [HttpDelete]
        public virtual async Task<IActionResult> DeleteItemByID([FromQuery] TKey ID)
        {
            try
            {
                await _repository.DeleteItem(ID);
                return Ok();
            }
            catch (Exception ex)
            {
                return Error(ex);
            }
        }
    }
}