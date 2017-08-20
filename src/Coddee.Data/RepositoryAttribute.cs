// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;

namespace Coddee.Data
{
    /// <summary>
    /// Identifies a repository object.
    /// This attribute is used to discover repositories in an assembly.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
    public sealed class RepositoryAttribute : Attribute
    {
        public RepositoryAttribute(Type implementedRepository, bool discoverable = true)
        {
            ImplementedRepository = implementedRepository;
            Discoverable = discoverable;
        }

        /// <summary>
        /// Should this repository be registered when the repository manager search for repositories.
        /// </summary>
        public bool Discoverable { get; set; }

        /// <summary>
        /// The <seealso cref="IRepository"/> interface implemented in this repository.
        /// </summary>
        public Type ImplementedRepository { get; set; }
    }
}