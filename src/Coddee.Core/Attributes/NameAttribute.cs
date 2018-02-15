// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;

namespace Coddee.Attributes
{
    /// <summary>
    /// Provides an alternative or simple name for a member
    /// </summary>
    [AttributeUsage(AttributeTargets.All, AllowMultiple = true, Inherited = true)]
    public class NameAttribute : Attribute
    {
        /// <summary>
        /// The name
        /// </summary>
        public string Value { get; }

        /// <inheritdoc />
        public NameAttribute(string value)
        {
            Value = value;
        }
    }
}
