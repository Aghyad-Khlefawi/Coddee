// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

namespace Coddee
{
    /// <summary>
    /// An object that has an identifier that differentiate it from other object.
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    public interface IUniqueObject<TKey>
    {
        /// <summary>
        /// Get the object identifier.
        /// </summary>
        TKey GetKey { get; }
    }
}
