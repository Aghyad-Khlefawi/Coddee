// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using Newtonsoft.Json;

namespace Coddee.Data
{
    [JsonObject]
    public class APIException : CoddeeException
    {
        public APIException()
        {
            
        }

        public APIException(Exception inner)
            : base(0, inner.Message, inner)
        {
            if (inner is DBException dbException)
                Code = dbException.Code;

            InnerExceptionSeriailized = JsonConvert.SerializeObject(inner);
            InnerExceptionType = inner.GetType();
        }

        public APIException(int code)
            :base(code)
        {
        }

        public APIException(int code, string message) : base(code,message)
        {
        }

        public APIException(int code, string message, Exception inner) : base(code, message, inner)
        {
            InnerExceptionSeriailized = JsonConvert.SerializeObject(inner);
            InnerExceptionType = inner.GetType();
        }

        public string InnerExceptionSeriailized { get; set; }
        public Type InnerExceptionType { get; set; }
    }
}
