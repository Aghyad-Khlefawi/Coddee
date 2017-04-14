// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using Microsoft.Practices.Unity;

namespace Coddee.WPF
{
    public static class Extensions
    {
        /// <summary>
        /// Register and instance by using the container to resolve it
        /// </summary>
        /// <typeparam name="TInterface"></typeparam>
        /// <typeparam name="TImplementation"></typeparam>
        /// <param name="container"></param>
        public static void RegisterInstance<TInterface, TImplementation>(this IUnityContainer container)
            where TImplementation : TInterface
        {
            container.RegisterInstance<TInterface>(container.Resolve<TImplementation>());
        }
    }
}