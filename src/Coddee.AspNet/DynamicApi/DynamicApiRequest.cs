using System;
using Microsoft.AspNetCore.Http;

namespace Coddee.AspNet
{
    /// <summary>
    /// Contains the information of a dynamic API request
    /// </summary>
    public class DynamicApiRequest
    {
        /// <summary>
        /// The time the request was received.
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// The <see cref="HttpContext"/> object.
        /// </summary>
        public HttpContext HttpContext { get; set; }

        /// <summary>
        /// Contains the requested controller and action information.
        /// </summary>
        public DynamicApiActionPath RequestedActionPath { get; set; }

        /// <summary>
        /// An id to Identify the request.
        /// </summary>
        public long Id { get; set; }
    }
}