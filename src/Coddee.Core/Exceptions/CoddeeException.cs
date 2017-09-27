// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;

namespace Coddee
{
   public class CoddeeException:Exception
    {
        public CoddeeException()
        {
            
        }
        public CoddeeException(Exception ex)
            : base(ex.Message, ex)
        {
        }

        public CoddeeException(int code)
        {
            Code = code;
        }

        public CoddeeException(int code, string message) : base(message)
        {
            Code = code;
        }

        public CoddeeException(int code, string message, Exception inner) : base(message, inner)
        {
            Code = code;
        }

        public int Code { get; set; }
    }
}
