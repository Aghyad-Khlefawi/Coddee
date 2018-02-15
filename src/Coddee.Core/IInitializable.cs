// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System.Threading.Tasks;

namespace Coddee
{
    /// <summary>
    /// An object that needs initialization before being used.
    /// </summary>
    public interface IInitializable
    {
        /// <summary>
        /// Indicates that the object is initialized and ready.
        /// </summary>
        bool IsInitialized { get; }

        /// <summary>
        /// Initialized the object.
        /// </summary>
        /// <param name="forceInitialize">if set to true the object will re-initialize if it's already initialized.</param>
        Task Initialize(bool forceInitialize = false);
    }
}
