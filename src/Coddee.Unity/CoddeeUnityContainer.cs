// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using Unity;

namespace Coddee.Unity
{
    /// <summary>
    /// <see cref="UnityContainer"/> wrapper for Coddee
    /// </summary>
    public class CoddeeUnityContainer:IContainer
    {
        private readonly IUnityContainer _container;


        /// <summary>
        /// The internal unity container instance.
        /// </summary>
        public IUnityContainer Container
        {
            get { return _container; }
        }

        /// <inheritdoc />
        public CoddeeUnityContainer()
        {
            _container = new UnityContainer();
            RegisterInstance<IContainer>(this);
        }

        /// <inheritdoc />
        public void RegisterInstance(Type type, object instance)
        {
            _container.RegisterInstance(type, instance);
        }

        /// <inheritdoc />
        public void RegisterInstance<T>(T instance)
        {
            _container.RegisterInstance<T>(instance);
        }

        /// <inheritdoc />
        public T RegisterInstance<T, TImplementation>() where TImplementation:T
        {
            var res = _container.Resolve<TImplementation>();
            RegisterInstance<T>(res);
            return res;
        }

        /// <inheritdoc />
        public void RegisterType<T, TImplementation>() where TImplementation : T
        {
            _container.RegisterType<T, TImplementation>();
        }

        /// <inheritdoc />
        public object Resolve(Type type)
        {
            return _container.Resolve(type);
        }

        /// <inheritdoc />
        public T Resolve<T>()
        {
            return _container.Resolve<T>();
        }

        /// <inheritdoc />
        public bool IsRegistered<T>()
        {
            return _container.IsRegistered<T>();
        }

        /// <inheritdoc />
        public bool IsRegistered(Type type)
        {
            return _container.IsRegistered(type);
        }
    }
}
