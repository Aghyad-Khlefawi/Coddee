// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;

namespace Coddee.Attributes
{
    /// <summary>
    /// Indicates that a member requires a specific permission
    /// </summary>
    [AttributeUsage(AttributeTargets.All, Inherited = true, AllowMultiple = true)]
    public sealed class AuthorizeAttribute : Attribute
    {
        /// <inheritdoc />
        public AuthorizeAttribute()
        {
            
        }

        /// <inheritdoc />
        public AuthorizeAttribute(string claim)
        {
            Claim = claim;
        }

        /// <summary>
        /// The claim required to use the member.
        /// </summary>
        public string Claim { get; }

    }
}
