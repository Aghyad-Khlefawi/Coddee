// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;

namespace Coddee.Data.REST
{
    /// <summary>
    /// Base implementation for a REST repository functionality
    /// </summary>
    public abstract class RESTRepositoryBase : RepositoryBase, IRESTRepository
    {
        /// <summary>
        /// The default format of DateTime.
        /// </summary>
        public static string DefaultDateTimeFormat = "dd/MM/yyyy HH:mm:ss";

        /// <summary>
        /// The default Json DateTime converter.
        /// </summary>
        public static IsoDateTimeConverter DefaultDateTimeConverter = new IsoDateTimeConverter
        {
            DateTimeFormat = DefaultDateTimeFormat
        };
        /// <summary>
        /// The default Json DateTime converter.
        /// </summary>
        public static JsonSerializerSettings DefaultJsonSerializerSettings = new JsonSerializerSettings
        {
            DateFormatString = DefaultDateTimeFormat
        };

        /// <inheritdoc />
        public override int RepositoryType { get; } = (int)RepositoryTypes.REST;

        /// <summary>
        /// The http client that will make the requests to the server.
        /// </summary>
        protected HttpClient _httpClient;

        /// <summary>
        /// An <see cref="Action"/> that will be executed in case an Unauthorized response was received.
        /// </summary>
        protected Action _unauthorizedRequestHandler;

        /// <inheritdoc />
        public void Initialize(HttpClient httpClient,
                               Action unauthorizedRequestHandler,
                               IRepositoryManager repositoryManager,
                               IObjectMapper mapper,
                               Type implementedInterface,
                               RepositoryConfigurations config = null)
        {
            SetHttpClient(httpClient);
            _unauthorizedRequestHandler = unauthorizedRequestHandler;
            Initialize(repositoryManager, mapper, implementedInterface, config);
        }

        /// <inheritdoc />
        public void SetHttpClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
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
        /// <param name="url">The request URL</param>
        /// <param name="param">optional parameter will be sent as a JSON object</param>
        /// <returns></returns>
        protected async Task Post(string url,
                                  object param = null)
        {
            var res = await SendPostRequest(url, param);
            var resString = await res.Content.ReadAsStringAsync();
            if (!res.IsSuccessStatusCode)
            {
                if (res.StatusCode == HttpStatusCode.Unauthorized || res.StatusCode == HttpStatusCode.Forbidden)
                    _unauthorizedRequestHandler?.Invoke();
                throw HandleBadRequest(resString);
            }
        }

        private async Task<HttpResponseMessage> SendPostRequest(string url, object param)
        {
            var content = param != null
                              ? new StringContent(SerializeObject(param), Encoding.UTF8, "application/json")
                              : null;
            return await _httpClient.PostAsync(url, content);
        }

        /// <summary>
        /// Handle responses with BadRequest response code.
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        protected virtual Exception HandleBadRequest(string ex)
        {
            var exception = JsonConvert.DeserializeObject<APIException>(ex, DefaultJsonSerializerSettings);
            if (exception.InnerExceptionType == typeof(DBException))
            {
                var jo = JObject.Parse(exception.InnerExceptionSeriailized);
                var code = exception.Code;
                var message = jo[nameof(DBException.Message)].Value<string>();
                return new DBException(code, message);
            }
            return exception;
        }

        /// <summary>
        /// Send a post request that returns a result
        /// </summary>
        /// <typeparam name="T">The result type</typeparam>
        /// <param name="url">The request URL</param>
        /// <param name="param">optional parameter will be sent as a JSON object</param>
        /// <returns></returns>
        protected async Task<T> Post<T>(string url,
                                        object param = null)
        {
            var res = await SendPostRequest(url, param);
            var resString = await res.Content.ReadAsStringAsync();

            if (res.IsSuccessStatusCode)
            {

                return JsonConvert.DeserializeObject<T>(resString, DefaultJsonSerializerSettings);
            }

            if (res.StatusCode == HttpStatusCode.Unauthorized || res.StatusCode == HttpStatusCode.Forbidden)
                _unauthorizedRequestHandler?.Invoke();

            throw HandleBadRequest(resString);
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
        /// <param name="url">The request URL</param>
        /// <param name="param">optional parameter will be sent as a JSON object</param>
        /// <returns></returns>
        protected async Task Put(string url,
                                 object param = null)
        {
            var content = param != null
                ? new StringContent(SerializeObject(param), Encoding.UTF8, "application/json")
                : null;
            var res = await _httpClient.PutAsync(url, content);
            var resString = await res.Content.ReadAsStringAsync();
            if (!res.IsSuccessStatusCode)
            {
                if (res.StatusCode == HttpStatusCode.Unauthorized || res.StatusCode == HttpStatusCode.Forbidden)
                    _unauthorizedRequestHandler?.Invoke();
                throw HandleBadRequest(resString);
            }
        }

        /// <summary>
        /// Send a Put request that returns a result
        /// </summary>
        /// <typeparam name="T">The result type</typeparam>
        /// <param name="url">The request URL</param>
        /// <param name="param">optional parameter will be sent as a JSON object</param>
        /// <returns></returns>
        protected async Task<T> Put<T>(string url,
                                       object param = null)
        {
            var content = param != null
                ? new StringContent(JsonConvert.SerializeObject(param, DefaultJsonSerializerSettings), Encoding.UTF8, "application/json")
                : null;
            var res = await _httpClient.PutAsync(url, content);
            return await HandleResquestResponse<T>(res);
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
        /// <param name="url">The request URL</param>
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

            var requestUri = urlBuilder.ToString(0,
                                                 param != null
                                                     ? urlBuilder.Length - 1
                                                     : urlBuilder.Length);
            var res =
                await _httpClient.GetAsync(requestUri);
            return await HandleResquestResponse<T>(res);
        }

        /// <summary>
        /// Handle the <see cref="HttpResponseMessage"/> and return the result of the response.
        /// </summary>
        protected virtual async Task<T> HandleResquestResponse<T>(HttpResponseMessage res)
        {
            var resString = await res.Content.ReadAsStringAsync();
            if (res.StatusCode == HttpStatusCode.Unauthorized || res.StatusCode == HttpStatusCode.Forbidden)
                _unauthorizedRequestHandler?.Invoke();

            if (!res.IsSuccessStatusCode)
                throw HandleBadRequest(resString);

            if (string.IsNullOrEmpty(resString))
                throw new APIException(APIExceptionCodes.EmptyResponse, "Server returned an empty response");

            try
            {
                return JsonConvert.DeserializeObject<T>(resString, DefaultJsonSerializerSettings);
            }
            catch (Exception e)
            {
                throw new RestClientException(0, "Failed to deserialize the response", e, resString);
            }
        }

        /// <summary>
        /// Sends a get request
        /// </summary>
        /// <typeparam name="T">The result type</typeparam>
        /// <param name="url">The request URL</param>
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
        /// <param name="url">The request URL</param>
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
                throw HandleBadRequest(resString);
            if (res.StatusCode == HttpStatusCode.Unauthorized || res.StatusCode == HttpStatusCode.Forbidden)
                _unauthorizedRequestHandler?.Invoke();
        }

        /// <summary>
        /// Sends a Delete request
        /// </summary>
        /// <param name="controller">The requested controller</param>
        /// <param name="action">The requested action</param>
        /// <param name="id">The resource id</param>
        protected Task Delete(string controller,
                              string action,
                              string id)
        {
            return Delete($"{controller}/{action}", id);
        }

        /// <summary>
        /// Helper function to convert an object to string
        /// </summary>
        /// <param name="name">The parameter name</param>
        /// <param name="value">The parameter value</param>
        /// <returns></returns>
        protected virtual KeyValuePair<string, string> KeyValue(string name, object value)
        {
            return new KeyValuePair<string, string>(name, value.ToString());
        }

        /// <summary>
        /// Serialize an object to JSON.
        /// </summary>
        protected virtual string SerializeObject<T>(T obj)
        {
            return JsonConvert.SerializeObject(obj, DefaultJsonSerializerSettings);
        }
    }

    /// <summary>
    /// Base implementation for a REST repository that targets a specific Model type.
    /// </summary>
    /// <typeparam name="TModel"></typeparam>
    /// <typeparam name="TKey"></typeparam>
    public abstract class RESTRepositoryBase<TModel, TKey> : RESTRepositoryBase, IRepository<TModel, TKey>
        where TModel : IUniqueObject<TKey>
    {
        /// <summary>
        /// The sync service identifier for the model type.
        /// </summary>
        protected readonly string _identifier;


        /// <inheritdoc />
        protected RESTRepositoryBase()
        {
            _identifier = typeof(TModel).Name;
        }

        /// <summary>
        /// Called when the <see cref="IRepositorySyncService"/> receives a sync request
        /// </summary>
        public override void SyncServiceSyncReceived(string identifier, RepositorySyncEventArgs args)
        {
            base.SyncServiceSyncReceived(identifier, args);
            if (identifier == _identifier)
            {
                RaiseItemsChanged(this,
                                  new RepositoryChangeEventArgs<TModel>(args.OperationType,
                                                                        ((JObject)args.Item).ToObject<TModel>(), true));
            }
        }

        /// <summary>
        /// Called when the repository content is changed
        /// </summary>
        protected virtual void RaiseItemsChanged(object sender, RepositoryChangeEventArgs<TModel> args)
        {
            ItemsChanged?.Invoke(this, args);
        }

        /// <summary>
        /// Calls the <see cref="IRepository.SetSyncService"/> on the registered repositories
        /// </summary>
        /// <param name="syncService">The sync service to use</param>
        /// <param name="sendSyncRequests">if set to true the repository will send sync requests when insert, edit and delete</param>
        public override void SetSyncService(IRepositorySyncService syncService, bool sendSyncRequests = true)
        {
            base.SetSyncService(syncService, sendSyncRequests);
            ItemsChanged += OnItemsChanged;
        }

        /// <summary>
        /// Default handler for <see cref="ItemsChanged"/> event
        /// </summary>
        protected virtual void OnItemsChanged(object sender, RepositoryChangeEventArgs<TModel> e)
        {
            if (!e.FromSync && _sendSyncRequests)
                _syncService?.SyncItem(_identifier, new RepositorySyncEventArgs { Item = e.Item, OperationType = e.OperationType });
        }

        /// <inheritdoc />
        public event EventHandler<RepositoryChangeEventArgs<TModel>> ItemsChanged;
    }

    /// <summary>
    /// Base implementation for a REST repository that provides ReadOnly functionality
    /// </summary>
    /// <typeparam name="TModel">The model type</typeparam>
    /// <typeparam name="TKey">The table key(ID) type</typeparam>
    public abstract class ReadOnlyRESTRepositoryBase<TModel, TKey> : RESTRepositoryBase<TModel, TKey>,
        IReadOnlyRepository<TModel, TKey> where TModel : IUniqueObject<TKey>
    {
        /// <inheritdoc />
        protected ReadOnlyRESTRepositoryBase(string controllerName)
        {
            ControllerName = controllerName;
        }

        /// <summary>
        /// The name of the AspNet controller.
        /// </summary>
        public string ControllerName { get; }

        /// <summary>
        /// Sends a get request to the targeted controller
        /// </summary>
        /// <returns></returns>
        protected Task<T> GetFromController<T>([CallerMemberName]string action = "",
                                               params KeyValuePair<string, string>[] param)
        {
            return Get<T>(ControllerName, action, param);
        }

        /// <summary>
        /// Sends a get request to the targeted controller
        /// </summary>
        /// <returns></returns>
        protected Task<T> GetFromController<T>(KeyValuePair<string, string> param, [CallerMemberName]string action = "")
        {
            return Get<T>(ControllerName, action, param);
        }

        /// <summary>
        /// Sends a get request to the targeted controller
        /// </summary>
        /// <returns></returns>
        protected Task<T> GetFromController<T>(IDictionary<string, string> param, [CallerMemberName]string action = "")
        {
            return Get<T>(ControllerName, action, param);
        }

        /// <inheritdoc />
        public Task<TModel> this[TKey index] =>
            GetFromController<TModel>(ApiCommonActions.GetItem,
                                      new KeyValuePair<string, string>("index", index.ToString()));

        /// <inheritdoc />
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
        /// <inheritdoc />
        protected CRUDRESTRepositoryBase(string controllerName) : base(controllerName)
        {
        }

        /// <summary>
        /// Sends a POST request to the targeted controller
        /// </summary>
        protected virtual Task PostToController(string action,
                                        object param = null)
        {
            return Post(ControllerName, action, param);
        }

        /// <summary>
        /// Sends a POST request to the targeted controller
        /// </summary>
        protected virtual Task<T> PostToController<T>(string action,
                                              object param = null)
        {
            return Post<T>(ControllerName, action, param);
        }

        /// <summary>
        /// Sends a POST request to the targeted controller
        /// </summary>
        protected virtual Task PutToController(string action,
                                       object param = null)
        {
            return Put(ControllerName, action, param);
        }

        /// <summary>
        /// Sends a POST request to the targeted controller
        /// </summary>
        protected virtual Task<T> PutToController<T>(string action,
                                             object param = null)
        {
            return Put<T>(ControllerName, action, param);
        }

        /// <summary>
        /// Sends a DELETE request to the targeted controller
        /// </summary>
        protected virtual Task DeleteFromController(string action, TKey id)
        {
            return Delete(ControllerName, action, id.ToString());
        }


        /// <inheritdoc />
        public virtual async Task<TModel> UpdateItem(TModel item)
        {
            var res = await PutToController<TModel>(ApiCommonActions.UpdateItem, item);
            RaiseItemsChanged(this, new RepositoryChangeEventArgs<TModel>(OperationType.Edit, res, false));
            return res;
        }

        /// <inheritdoc />
        public virtual async Task<TModel> InsertItem(TModel item)
        {
            var res = await PostToController<TModel>(ApiCommonActions.InsertItem, item);
            RaiseItemsChanged(this, new RepositoryChangeEventArgs<TModel>(OperationType.Add, res, false));
            return res;
        }

        /// <inheritdoc />
        public virtual async Task DeleteItemByKey(TKey ID)
        {
            var res = await this[ID];
            await DeleteFromController(ApiCommonActions.DeleteItemByKey, ID);
            RaiseItemsChanged(this, new RepositoryChangeEventArgs<TModel>(OperationType.Delete, res, false));
        }

        /// <inheritdoc />
        public virtual async Task DeleteItem(TModel item)
        {
            await DeleteItemByKey(item.GetKey);
        }
    }
}