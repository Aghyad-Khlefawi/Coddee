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
        bool IsInitialized { get; }
        Task Initialize(bool forceInitialize = false);
    }
}
