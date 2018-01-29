// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;

namespace Coddee.Attributes
{
    [AttributeUsage(AttributeTargets.All, Inherited = true, AllowMultiple = true)]
    public sealed class AuthorizeAttribute : Attribute
    {

        public AuthorizeAttribute()
        {
            
        }
        public AuthorizeAttribute(string claim)
        {
            Claim = claim;
        }

        public string Claim { get; }

    }
}
