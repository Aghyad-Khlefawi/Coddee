// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Coddee.Data.Rest
{
    /// <summary>
    /// Base implementation for a REST repository functionality
    /// </summary>
    public abstract class RESTRepositoryBase : RepositoryBase, IRESTRepository
    {
        private HttpClient _httpClient;
        private Action _unauthorizedRequestHandler;

        public void Initialize(HttpClient httpClient,
                               Action unauthorizedRequestHandler,
                               IRepositoryManager repositoryManager,
                               IObjectMapper mapper,
                               Type implementedInterface)
        {
            _httpClient = httpClient;
            _unauthorizedRequestHandler = unauthorizedRequestHandler;
            Initialize(repositoryManager, mapper, implementedInterface);
        }


        /// <summary>
        /// Send a post request that doesn't return a result
        /// </summary>
        /// <param name="controller">The controller name</param>
        /// <param name="action">The action</param>
        /// <param name="param">optional parameter will be sent as a JSON object</param>
        /// <returns></returns>
        protected Task Post(string controller,
                            string action,
                            object param = null)
        {
            return Post($"{controller}/{action}", param);
        }


        /// <summary>
        /// Send a post request that doesn't return a result
        /// </summary>
        /// <param name="url">The request url</param>
        /// <param name="param">optional parameter will be sent as a JSON object</param>
        /// <returns></returns>
        protected async Task Post(string url,
                                  object param = null)
        {
            var content = param != null
                ? new StringContent(JsonConvert.SerializeObject(param), Encoding.UTF8, "application/json")
                : null;
            var res = await _httpClient.PostAsync(url, content);
            var resString = await res.Content.ReadAsStringAsync();
            if (!res.IsSuccessStatusCode)
            {
                if (res.StatusCode == HttpStatusCode.Unauthorized || res.StatusCode == HttpStatusCode.Forbidden)
                    _unauthorizedRequestHandler?.Invoke();
                throw new InvalidOperationException(resString);
            }
        }

        /// <summary>
        /// Send a post request that returns a result
        /// </summary>
        /// <typeparam name="T">The result type</typeparam>
        /// <param name="url">The request url</param>
        /// <param name="param">optional parameter will be sent as a JSON object</param>
        /// <returns></returns>
        protected async Task<T> Post<T>(string url,
                                        object param = null)
        {
            var content = param != null
                ? new StringContent(JsonConvert.SerializeObject(param), Encoding.UTF8, "application/json")
                : null;
            var res = await _httpClient.PostAsync(url, content);
            var resString = await res.Content.ReadAsStringAsync();
            if (res.IsSuccessStatusCode)
                return JsonConvert.DeserializeObject<T>(resString);
            if (res.StatusCode == HttpStatusCode.Unauthorized || res.StatusCode == HttpStatusCode.Forbidden)
                _unauthorizedRequestHandler?.Invoke();

            throw new InvalidOperationException(resString);
        }

        /// <summary>
        /// Send a post request that returns a result
        /// </summary>
        /// <typeparam name="T">The result type</typeparam>
        /// <param name="controller">The requested controller</param>
        /// <param name="action">The requested action</param>
        /// <param name="param">optional parameter will be sent as a JSON object</param>
        /// <returns></returns>
        protected Task<T> Post<T>(string controller,
                                  string action,
                                  object param = null)
        {
            return Post<T>($"{controller}/{action}", param);
        }


        /// <summary>
        /// Send a Put request that doesn't return a result
        /// </summary>
        /// <param name="controller">The controller name</param>
        /// <param name="action">The action</param>
        /// <param name="param">optional parameter will be sent as a JSON object</param>
        /// <returns></returns>
        protected Task Put(string controller,
                           string action,
                           object param = null)
        {
            return Put($"{controller}/{action}", param);
        }

        /// <summary>
        /// Send a Put request that doesn't return a result
        /// </summary>
        /// <param name="url">The request url</param>
        /// <param name="param">optional parameter will be sent as a JSON object</param>
        /// <returns></returns>
        protected async Task Put(string url,
                                 object param = null)
        {
            var content = param != null
                ? new StringContent(JsonConvert.SerializeObject(param), Encoding.UTF8, "application/json")
                : null;
            var res = await _httpClient.PutAsync(url, content);
            var resString = await res.Content.ReadAsStringAsync();
            if (!res.IsSuccessStatusCode)
            {
                if (res.StatusCode == HttpStatusCode.Unauthorized || res.StatusCode == HttpStatusCode.Forbidden)
                    _unauthorizedRequestHandler?.Invoke();
                throw new InvalidOperationException(resString);
            }
        }

        /// <summary>
        /// Send a Put request that returns a result
        /// </summary>
        /// <typeparam name="T">The result type</typeparam>
        /// <param name="url">The request url</param>
        /// <param name="param">optional parameter will be sent as a JSON object</param>
        /// <returns></returns>
        protected async Task<T> Put<T>(string url,
                                       object param = null)
        {
            var content = param != null
                ? new StringContent(JsonConvert.SerializeObject(param), Encoding.UTF8, "application/json")
                : null;
            var res = await _httpClient.PutAsync(url, content);
            var resString = await res.Content.ReadAsStringAsync();
            if (res.IsSuccessStatusCode)
                return JsonConvert.DeserializeObject<T>(resString);
            if (res.StatusCode == HttpStatusCode.Unauthorized || res.StatusCode == HttpStatusCode.Forbidden)
                _unauthorizedRequestHandler?.Invoke();

            throw new InvalidOperationException(resString);
        }

        /// <summary>
        /// Send a Put request that returns a result
        /// </summary>
        /// <typeparam name="T">The result type</typeparam>
        /// <param name="controller">The requested controller</param>
        /// <param name="action">The requested action</param>
        /// <param name="param">optional parameter will be sent as a JSON object</param>
        /// <returns></returns>
        protected Task<T> Put<T>(string controller,
                                 string action,
                                 object param = null)
        {
            return Put<T>($"{controller}/{action}", param);
        }

        /// <summary>
        /// Sends a get request
        /// </summary>
        /// <typeparam name="T">The result type</typeparam>
        /// <param name="url">The request url</param>
        /// <param name="param">optional parameter will be sent as a query string</param>
        /// <returns></returns>
        protected async Task<T> Get<T>(string url,
                                       params KeyValuePair<string, string>[] param)
        {
            var urlBuilder = new StringBuilder(url);
            if (param != null)
            {
                urlBuilder.Append("?");
                foreach (var item in param)
                {
                    urlBuilder.Append(item.Key);
                    urlBuilder.Append("=");
                    urlBuilder.Append(item.Value);
                    urlBuilder.Append("&");
                }
            }
            var res =
                await _httpClient.GetAsync(urlBuilder.ToString(0,
                                                               param != null
                                                                   ? urlBuilder.Length - 1
                                                                   : urlBuilder.Length));
            var resString = await res.Content.ReadAsStringAsync();
            if (res.IsSuccessStatusCode)
                return JsonConvert.DeserializeObject<T>(resString);
            if (res.StatusCode == HttpStatusCode.Unauthorized || res.StatusCode == HttpStatusCode.Forbidden)
                _unauthorizedRequestHandler?.Invoke();
            throw new InvalidOperationException(resString);
        }

        /// <summary>
        /// Sends a get request
        /// </summary>
        /// <typeparam name="T">The result type</typeparam>
        /// <param name="url">The request url</param>
        /// <param name="param">optional parameter will be sent as a query string</param>
        /// <returns></returns>
        protected Task<T> Get<T>(string url,
                                 IDictionary<string, string> param = null)
        {
            return Get<T>(url, param?.ToArray());
        }

        /// <summary>
        /// Sends a get request
        /// </summary>
        /// <typeparam name="T">The result type</typeparam>
        /// <param name="controller">The requested controller</param>
        /// <param name="action">The requested action</param>
        /// <param name="param">optional parameter will be sent as a query string</param>
        protected Task<T> Get<T>(string controller,
                                 string action,
                                 params KeyValuePair<string, string>[] param)
        {
            return Get<T>($"{controller}/{action}", param);
        }

        /// <summary>
        /// Sends a get request
        /// </summary>
        /// <typeparam name="T">The result type</typeparam>
        /// <param name="controller">The requested controller</param>
        /// <param name="action">The requested action</param>
        /// <param name="param">optional parameter will be sent as a query string</param>
        protected Task<T> Get<T>(string controller,
                                 string action,
                                 IDictionary<string, string> param = null)
        {
            return Get<T>(controller, action, param?.ToArray());
        }

        /// <summary>
        /// Sends a Delete request
        /// </summary>
        /// <typeparam name="T">The result type</typeparam>
        /// <param name="url">The request url</param>
        /// <param name="id">The resource id</param>
        /// <returns></returns>
        protected async Task Delete(string url,
                                    string id)
        {
            var urlBuilder = new StringBuilder(url);
            if (id != null)
            {
                urlBuilder.Append("?");
                urlBuilder.Append("id");
                urlBuilder.Append("=");
                urlBuilder.Append(id);
            }
            var res =
                await _httpClient.DeleteAsync(urlBuilder.ToString());
            var resString = await res.Content.ReadAsStringAsync();
            if (!res.IsSuccessStatusCode)
                throw new InvalidOperationException(resString);
            if (res.StatusCode == HttpStatusCode.Unauthorized || res.StatusCode == HttpStatusCode.Forbidden)
                _unauthorizedRequestHandler?.Invoke();
        }

        /// <summary>
        /// Sends a Delete request
        /// </summary>
        /// <typeparam name="T">The result type</typeparam>
        /// <param name="controller">The requested controller</param>
        /// <param name="action">The requested action</param>
        /// <param name="id">The resource id</param>
        protected Task Delete(string controller,
                              string action,
                              string id)
        {
            return Delete($"{controller}/{action}", id);
        }
    }

    /// <summary>
    /// Base implementation for a REST repository that provides ReadOnly functionality
    /// </summary>
    /// <typeparam name="TModel">The model type</typeparam>
    /// <typeparam name="TKey">The table key(ID) type</typeparam>
    public abstract class ReadOnlyRESTRepositoryBase<TModel, TKey> : RESTRepositoryBase, IReadOnlyRepository<TModel, TKey>
    {
        /// <summary>
        /// Return the name of the targeted controller
        /// </summary>
        /// <returns></returns>
        public abstract string ControllerName { get; }

        protected Task<T> GetFromController<T>(string action,
                                               params KeyValuePair<string, string>[] param)
        {
            return Get<T>(ControllerName, action, param);
        }

        protected Task<T> GetFromController<T>(string action,
                                               IDictionary<string, string> param = null)
        {
            return Get<T>(ControllerName, action, param);
        }

        public Task<TModel> this[TKey index] =>
            GetFromController<TModel>(ApiCommonActions.GetItems,
                                      new KeyValuePair<string, string>("ID", index.ToString()));

        public Task<IEnumerable<TModel>> GetItems()
        {
            return GetFromController<IEnumerable<TModel>>(ApiCommonActions.GetItems);
        }
    }

    /// <summary>
    /// Base implementation for a REST repository that provides the CRUD(Create,Read,Update,Delete) functionality
    /// </summary>
    /// <typeparam name="TModel">The model type</typeparam>
    /// <typeparam name="TKey">The table key(ID) type</typeparam>
    public abstract class CRUDRESTRepositoryBase<TModel, TKey> : ReadOnlyRESTRepositoryBase<TModel, TKey>,
        ICRUDRepository<TModel, TKey>
        where TModel : IUniqueObject<TKey>
    {
        protected Task PostToController(string action,
                                        object param = null)
        {
            return Post(ControllerName, action, param);
        }

        protected Task<T> PostToController<T>(string action,
                                              object param = null)
        {
            return Post<T>(ControllerName, action, param);
        }

        protected Task PutToController(string action,
                                       object param = null)
        {
            return Put(ControllerName, action, param);
        }

        protected Task<T> PutToController<T>(string action,
                                             object param = null)
        {
            return Put<T>(ControllerName, action, param);
        }

        protected Task DeleteFromController(string action, TKey id)
        {
            return Delete(ControllerName, action, id.ToString());
        }





        public Task<TModel> UpdateItem(TModel item)
        {
            return PutToController<TModel>(ApiCommonActions.UpdateItem, item);
        }

        public Task<TModel> InsertItem(TModel item)
        {
            return PostToController<TModel>(ApiCommonActions.InsertItem, item);
        }

        public Task DeleteItem(TKey ID)
        {
            return DeleteFromController(ApiCommonActions.DeleteItemByID, ID);
        }

        public Task DeleteItem(TModel item)
        {
            return DeleteItem(item.GetKey);
        }
    }
}