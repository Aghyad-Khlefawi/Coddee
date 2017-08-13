// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;

namespace Coddee.Services
{
    public class ModuleException : Exception
    {
        public ModuleException()
        {
        }

        public ModuleException(string message) : base(message)
        {
        }

        public ModuleException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}