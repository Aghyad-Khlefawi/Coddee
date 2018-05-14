// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using Newtonsoft.Json;

namespace Coddee.Data
{
    /// <summary>
    /// An exception wrapper for exceptions occurring in a Web-API 
    /// </summary>
    [JsonObject]
    public class APIException : CoddeeException
    {
        /// <inheritdoc />
        public APIException()
        {

        }

        /// <inheritdoc />
        public APIException(Exception inner)
            : base(0, inner.Message, inner)
        {
            if (inner is DBException dbException)
                Code = dbException.Code;

            InnerExceptionSeriailized = JsonConvert.SerializeObject(inner);
            InnerExceptionType = inner.GetType();
        }

        /// <inheritdoc />
        public APIException(int code)
            : base(code)
        {
        }

        /// <inheritdoc />
        public APIException(int code, string message) : base(code, message)
        {
        }

        /// <inheritdoc />
        public APIException(int code, string message, Exception inner) : base(code, message, inner)
        {
            InnerExceptionSeriailized = JsonConvert.SerializeObject(inner);
            InnerExceptionType = inner.GetType();
        }

        /// <summary>
        /// The wrapped exception in a JSON format
        /// </summary>
        public string InnerExceptionSeriailized { get; set; }

        /// <summary>
        /// The type of wrapped exception
        /// </summary>
        public Type InnerExceptionType { get; set; }
    }

    /// <summary>
    /// Defines the commonly used exception codes
    /// </summary>
    public static class APIExceptionCodes
    {
        /// <summary>
        /// Indicates that the request is missing a parameter.
        /// </summary>
        public static int MissingParamters = 0;

        /// <summary>
        /// Indicates that the server returned an empty response.
        /// </summary>
        public static int EmptyResponse = 1;


    }
}
