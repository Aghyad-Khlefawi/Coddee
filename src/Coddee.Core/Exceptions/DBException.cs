// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;

namespace Coddee.Data
{
    /// <summary>
    /// An exception that occurs while dealing with the database.
    /// </summary>
    public class DBException : CoddeeException
    {
        /// <inheritdoc />
        public DBException()
        {
            
        }
        /// <inheritdoc />
        public DBException(int code)
            :base(code)
        {
        }

        /// <inheritdoc />
        public DBException(int code, string message) : base(code,message)
        {
        }

        /// <inheritdoc />
        public DBException(int code, string message, Exception inner) : base(code, message, inner)
        {
        }
    }
}
