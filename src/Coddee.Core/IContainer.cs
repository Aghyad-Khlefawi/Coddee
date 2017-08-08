// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;

namespace Coddee
{
    public interface IContainer
    {
        void RegisterInstance(Type type,object instance);
        void RegisterInstance<T>(T instance);
        void RegisterInstance<T, TImplementation>() where TImplementation : T;
        void RegisterType<T, TImplementation>() where TImplementation : T;

        object Resolve(Type type);
        T Resolve<T>();

        bool IsRegistered<T>();
        bool IsRegistered(Type type);
    }
}
