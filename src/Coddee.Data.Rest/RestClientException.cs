using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Coddee.Data.REST
{
    /// <summary>
    /// Exception that occurs in the REST client
    /// </summary>
    public class RestClientException:CoddeeException
    {
        /// <inheritdoc />
        public RestClientException()
        {

        }

        /// <inheritdoc />
        public RestClientException(Exception inner)
            : base(inner)
        {
        }

        /// <inheritdoc />
        public RestClientException(int code)
            : base(code)
        {
        }

        /// <inheritdoc />
        public RestClientException(int code, string message) : base(code, message)
        {
        }

        /// <inheritdoc />
        public RestClientException(int code, string message, Exception inner,string responseContent) : base(code, message, inner)
        {
            ResponseContent = responseContent;
        }

        /// <summary>
        /// The content of the response that caused the exception.
        /// </summary>
        public string ResponseContent { get; set; }
    }
}
