using System;
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
        public DynamicApiException(int code, string message,DynamicApiRequest request) : base(code, message)
        {
            Request = request;
        }

        /// <inheritdoc />
        public DynamicApiException(int code, string message, Exception inner, DynamicApiRequest request) : base(code, message, inner)
        {
            Request = request;
        }

        /// <summary>
        /// The request that caused the exception.
        /// </summary>
        public DynamicApiRequest Request { get; set; }
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