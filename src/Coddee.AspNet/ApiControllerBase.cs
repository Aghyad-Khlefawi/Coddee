// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using System.Threading.Tasks;
using Coddee.Data;
using Coddee.Loggers;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Coddee.AspNet.Controllers
{
    /// <summary>
    /// Base class for AspNetCore controllers.
    /// </summary>
    public class ApiControllerBase : Controller
    {

        /// <summary>
        /// The event source used for the logging.
        /// </summary>
        protected string EventsSource = "ApiControllerBase";

        /// <inheritdoc />
        public ApiControllerBase(IRepositoryManager repoManager, ILogger logger)
        {
            _repositoryManager = repoManager;
            _logger = logger;
        }

        /// <summary>
        /// The logger service
        /// </summary>
        protected readonly ILogger _logger;


        /// <summary>
        /// The repository manager used by the application.
        /// </summary>
        protected readonly IRepositoryManager _repositoryManager;

        /// <summary>
        /// Handle an exception during a request.
        /// </summary>
        public virtual IActionResult Error(Exception ex)
        {
            _logger?.Log(EventsSource, ex);
            return BadRequest(JsonConvert.SerializeObject(new APIException(ex)));
        }
    }

    /// <inheritdoc />
    public class ApiControllerBase<TRepository> : ApiControllerBase where TRepository : IRepository
    {

        /// <summary>
        /// The repository this controller is using.
        /// </summary>
        protected TRepository _repository;

        /// <inheritdoc />
        public ApiControllerBase(IRepositoryManager repoManager, ILogger logger) : base(repoManager, logger)
        {
            _repository = _repositoryManager.GetRepository<TRepository>();
        }
    }

    /// <inheritdoc />
    public class ReadOnlyApiControllerBase<TRepository, TModel, TKey> : ApiControllerBase<TRepository>
        where TRepository : IReadOnlyRepository<TModel, TKey> where TModel : IUniqueObject<TKey>
    {
        /// <inheritdoc />
        public ReadOnlyApiControllerBase(IRepositoryManager repoManager, ILogger logger) : base(repoManager, logger)
        {
        }

        /// <summary>
        /// Return an item from the repository by its primary key.
        /// </summary>
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

        /// <summary>
        /// Returns all items from the repository.
        /// </summary>
        /// <returns></returns>
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
    /// <inheritdoc />
    public class CRUDApiControllerBase<TRepository, TModel, TKey> : ReadOnlyApiControllerBase<TRepository, TModel, TKey>
        where TRepository : ICRUDRepository<TModel, TKey> where TModel : IUniqueObject<TKey>
    {


        /// <inheritdoc />
        public CRUDApiControllerBase(IRepositoryManager repoManager, ILogger logger) : base(repoManager, logger)
        {

        }

        /// <summary>
        /// Inserts an item to the repository.
        /// </summary>
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

        /// <summary>
        /// Updates an item in the repository.
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// Delete an item from the repository.
        /// </summary>
        /// <returns></returns>
        [HttpDelete]
        public virtual async Task<IActionResult> DeleteItemByID([FromQuery] TKey ID)
        {
            try
            {
                await _repository.DeleteItemByKey(ID);
                return Ok();
            }
            catch (Exception ex)
            {
                return Error(ex);
            }
        }
    }
}