// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;

namespace Coddee.AspNet
{
    [AttributeUsage(AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public sealed class ApiActionAttribute : Attribute
    {
        public ApiActionAttribute(string path)
            : this(path, AspNet.HttpMethod.Get)
        {

        }

        public ApiActionAttribute(string path, string httpMethod)
        {
            Path = path;
            HttpMethod = httpMethod;
        }

        public string Path { get; }
        public string HttpMethod { get; set; }
    }
}