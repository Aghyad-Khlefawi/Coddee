// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;

namespace Coddee.Data
{
    public class DBException : CoddeeException
    {
        public DBException()
        {
            
        }
        public DBException(int code)
            :base(code)
        {
        }

        public DBException(int code, string message) : base(code,message)
        {
        }

        public DBException(int code, string message, Exception inner) : base(code, message, inner)
        {
        }
    }
}
