using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;

namespace Coddee.AspNet
{
    /// <summary>
    /// Dynamic API Exception.
    /// </summary>
    [Serializable]
    public class DynamicApiException : CoddeeException
    {
        /// <inheritdoc />
        public DynamicApiException()
        {
        }

        /// <inheritdoc />
        public DynamicApiException(int code, string message) : base(code, message)
        {
        }

        /// <inheritdoc />
        public DynamicApiException(int code, string message, Exception inner) : base(code, message, inner)
        {
        }
        protected DynamicApiException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }

    [Serializable]
    public class DynamicApiAggregateException : DynamicApiException
    {
        /// <inheritdoc />
        public DynamicApiAggregateException()
        {
        }

        /// <inheritdoc />
        public DynamicApiAggregateException(int code, string message) : base(code, message)
        {
        }

        /// <inheritdoc />
        public DynamicApiAggregateException(int code, string message, AggregateException ex) : this(code, message,ex.InnerExceptions.ToList())
        {
        }

        public DynamicApiAggregateException(int code, string message, IList<Exception> inner) : base(code, message)
        {
            InnerExceptions = new ReadOnlyCollection<Exception>(inner);
            Messages = new ReadOnlyCollection<string>(inner.Select(e=>e.Message).ToList());
        }
        protected DynamicApiAggregateException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
        public ReadOnlyCollection<Exception> InnerExceptions { get; }

        public ReadOnlyCollection<string> Messages { get; }
    }

    /// <summary>
    /// Exception codes used in <see cref="DynamicApi"/>
    /// </summary>
    public static class DynamicApiExceptionCodes
    {

        /// <summary>
        /// Indicates that an unknown error occurred while processing the request.
        /// </summary>
        public const int UnknownError = 100;

        /// <summary>
        /// Indicates that the client is unauthorized to call a specific action.
        /// </summary>
        public const int Unauthorized = 101;

        /// <summary>
        /// Indicates that the client is calling an action that was not found.
        /// </summary>
        public const int ActionNotFound = 102;


        /// <summary>
        /// Indicates that the client is calling an action that was found but with incorrect parameters.
        /// </summary>
        public const int MissingParameter = 103;

    }
}