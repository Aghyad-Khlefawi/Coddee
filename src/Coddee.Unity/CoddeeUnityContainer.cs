// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using Microsoft.Practices.Unity;

namespace Coddee.Unity
{
    public class CoddeeUnityContainer:IContainer
    {
        private readonly IUnityContainer _container;
        public CoddeeUnityContainer()
        {
            _container = new UnityContainer();
            RegisterInstance<IContainer>(this);
        }
        public void RegisterInstance(Type type, object instance)
        {
            _container.RegisterInstance(type, instance);
        }

        public void RegisterInstance<T>(T instance)
        {
            _container.RegisterInstance<T>(instance);
        }

        public void RegisterInstance<T, TImplementation>() where TImplementation:T
        {
            _container.RegisterInstance<T>(_container.Resolve<TImplementation>());
        }

        public void RegisterType<T, TImplementation>() where TImplementation : T
        {
            _container.RegisterType<T, TImplementation>();
        }

        public object Resolve(Type type)
        {
            return _container.Resolve(type);
        }

        public T Resolve<T>()
        {
            return _container.Resolve<T>();
        }

        public bool IsRegistered<T>()
        {
            return _container.IsRegistered<T>();
        }

        public bool IsRegistered(Type type)
        {
            return _container.IsRegistered(type);
        }
    }
}
