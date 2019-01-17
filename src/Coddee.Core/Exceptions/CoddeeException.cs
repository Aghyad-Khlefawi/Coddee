// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using System.Runtime.Serialization;

namespace Coddee
{
    /// <summary>
    /// A general purpose exception that provides a code to identify the error
    /// </summary>
    [Serializable]
    public class CoddeeException : Exception
    {
        /// <inheritdoc />
        public CoddeeException()
        {

        }

        /// <inheritdoc />
        public CoddeeException(Exception ex)
            : base(ex.Message, ex)
        {
        }

        /// <inheritdoc />
        public CoddeeException(int code)
        {
            Code = code;
        }
        /// <inheritdoc />
        public CoddeeException(string message) : base(message)
        {
        }

        /// <inheritdoc />
        public CoddeeException(int code, string message) : base(message)
        {
            Code = code;
        }

        /// <inheritdoc />
        public CoddeeException(int code, string message, Exception inner) : base(message, inner)
        {
            Code = code;
        }
        protected CoddeeException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
        /// <summary>
        /// An error code
        /// </summary>
        public int Code { get; set; }
    }
}
