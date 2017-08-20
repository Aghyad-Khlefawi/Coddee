// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;

namespace Coddee
{
    /// <summary>
    /// A dependency container.
    /// </summary>
    public interface IContainer
    {
        /// <summary>
        /// Register an instance of a type to the container.
        /// </summary>
        /// <param name="type">The type of the object.</param>
        /// <param name="instance">The object instance.</param>
        void RegisterInstance(Type type,object instance);


        /// <summary>
        /// Register an instance of a type to the container.
        /// </summary>
        /// <param name="instance">The object instance.</param>
        /// <typeparam name="T">The type of the object.</typeparam>
        void RegisterInstance<T>(T instance);

        /// <summary>
        /// Register an instance of a type to the container.
        /// </summary>
        /// <typeparam name="TImplementation">The type of the implementation.</typeparam>
        /// <typeparam name="T">The type of the object.</typeparam>
        T RegisterInstance<T, TImplementation>() where TImplementation : T;

        /// <summary>
        /// Register a type to the container.
        /// </summary>
        /// <typeparam name="TImplementation">The type of the implementation.</typeparam>
        /// <typeparam name="T">The type of the object.</typeparam>
        void RegisterType<T, TImplementation>() where TImplementation : T;

        /// <summary>
        /// Resolve a dependency.
        /// </summary>
        object Resolve(Type type);

        /// <summary>
        /// Resolve a dependency.
        /// </summary>
        T Resolve<T>();

        /// <summary>
        /// Checks if a type is already is registered in the repository.
        /// </summary>
        bool IsRegistered<T>();

        /// <summary>
        /// Checks if a type is already is registered in the repository.
        /// </summary>
        bool IsRegistered(Type type);
    }
}
