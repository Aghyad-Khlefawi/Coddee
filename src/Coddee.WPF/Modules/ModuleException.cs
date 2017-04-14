// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using System.Runtime.Serialization;

namespace Coddee.WPF.Modules
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

        protected ModuleException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}