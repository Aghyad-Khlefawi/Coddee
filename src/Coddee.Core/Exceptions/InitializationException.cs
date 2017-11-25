// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;

namespace Coddee.Exceptions
{
    public class InitializationException : Exception
    {
        public InitializationException()
        {
        }

        public InitializationException(string componentName) : base($"{componentName} Failed to initialize.")
        {
        }

        public InitializationException(string componentName, Exception inner) : base(componentName, inner)
        {
        }
    }
}
