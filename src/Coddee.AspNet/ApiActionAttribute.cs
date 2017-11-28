// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using System.Net.Http;

namespace Coddee.AspNet
{
    [AttributeUsage(AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
    public sealed class ApiActionAttribute : Attribute
    {
        public ApiActionAttribute(string path)
            : this(path, HttpMethod.Get)
        {

        }

        public ApiActionAttribute(string path, HttpMethod httpMethod)
        {
            Path = path;
            HttpMethod = httpMethod;
        }

        public string Path { get; }
        public HttpMethod HttpMethod { get; set; }


    }
}