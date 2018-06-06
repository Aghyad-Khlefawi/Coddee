// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;

namespace Coddee.AspNet
{
    /// <summary>
    /// <see cref="IContainer"/> Implementation for ASP core applications
    /// </summary>
    public class AspCoreContainer : IContainer
    {
        private readonly IServiceCollection _service;
        private IServiceProvider _provider;


        /// <inheritdoc />
        public AspCoreContainer(IServiceCollection service)
        {
            _service = service;
            _service.AddSingleton<IContainer>(this);
        }

        /// <summary>
        /// Returns the internal service provider instance.
        /// </summary>
        public IServiceProvider ServiceProvider => _provider;

        /// <inheritdoc />
        public void RegisterInstance(Type type, object instance)
        {
            _service.AddSingleton(type, instance);
            _provider = _service.BuildServiceProvider();
        }

        /// <inheritdoc />
        public void RegisterInstance<T>(T instance)
        {
            RegisterInstance(typeof(T), instance);
        }

        /// <inheritdoc />
        public T RegisterInstance<T, TImplementation>() where TImplementation : T
        {
            var instance = Resolve<T>();
            RegisterInstance(typeof(T), instance);
            return instance;
        }

        /// <inheritdoc />
        public void RegisterType<T, TImplementation>() where TImplementation : T
        {
            _service.AddTransient(typeof(T), typeof(TImplementation));
            _provider = _service.BuildServiceProvider();
        }

        /// <inheritdoc />
        public object Resolve(Type type)
        {
            return GetProvider().GetService(type);
        }

        /// <inheritdoc />
        public T Resolve<T>()
        {
            return GetProvider().GetService<T>();
        }

        /// <inheritdoc />
        public bool IsRegistered<T>()
        {
            return IsRegistered(typeof(T));
        }

        /// <inheritdoc />
        public bool IsRegistered(Type type)
        {
            return _service.Any(e => e.ServiceType == type);
        }

        IServiceProvider GetProvider()
        {
            return _provider ?? (_provider = _service.BuildServiceProvider());
        }
    }
}
