// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;

namespace Coddee
{
    /// <summary>
    /// Identifies a function that is used in an API controller class.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public sealed class ApiActionAttribute : Attribute
    {
        /// <inheritdoc />
        public ApiActionAttribute()
        {

        }

        /// <inheritdoc />
        public ApiActionAttribute(string path)
            : this(path, Coddee.HttpMethod.Get)
        {
        }

        /// <inheritdoc />
        public ApiActionAttribute(string path, string httpMethod)
        {
            Path = path;
            HttpMethod = httpMethod;
        }

        /// <summary>
        /// The route to the action.
        /// </summary>
        public string Path { get; }

        /// <summary>
        /// The required <see cref="Coddee.HttpMethod"/>.
        /// </summary>
        public string HttpMethod { get; set; }
    }
}