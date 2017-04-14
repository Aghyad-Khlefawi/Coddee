// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coddee.AppBuilder
{
    public class ApplicationBuildException : Exception
    {
        public ApplicationBuildException()
        {
        }

        public ApplicationBuildException(string message) : base(message)
        {
        }

        public ApplicationBuildException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
