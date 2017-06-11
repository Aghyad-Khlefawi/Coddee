// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using System.Collections;
using System.Collections.Generic;

namespace Coddee
{
    /// <summary>
    /// Mapper to convert between types with same properties
    /// </summary>
    public interface IObjectMapper
    {

        /// <summary>
        /// Register a manual mapping information from source to target
        /// </summary>
        /// <typeparam name="TSource">Source type</typeparam>
        /// <typeparam name="TTarget">Target type</typeparam>
        void RegisterMap<TSource, TTarget>(Action<TSource, TTarget> convert);

        /// <summary>
        /// Register mapping information from source to target
        /// </summary>
        /// <typeparam name="TSource">Source type</typeparam>
        /// <typeparam name="TTarget">Target type</typeparam>
        void RegisterMap<TSource, TTarget>();

        /// <summary>
        /// Register mapping information between two types
        /// </summary>
        /// <typeparam name="TType1">Type1</typeparam>
        /// <typeparam name="TType2">Type2</typeparam>
        void RegisterTwoWayMap<TType1, TType2>();

        /// <summary>
        /// Map an object to a specific type
        /// </summary>
        /// <typeparam name="TTarget">Target type</typeparam>
        /// <param name="item">object to map</param>
        /// <returns>A new object of the target type with the source object values</returns>
        TTarget Map<TTarget>(object item) where TTarget : new();

        /// <summary>
        /// Map a collection of objects to a specific type
        /// </summary>
        /// <typeparam name="TTarget">Target type</typeparam>
        /// <param name="source">the source collection</param>
        /// <returns>A new collection of the target type with the source object values</returns>
        IEnumerable<TTarget> MapCollection<TTarget>(IList source) where TTarget : new();

        /// <summary>
        /// Maps the values of the source object to the target object
        /// </summary>
        /// <typeparam name="TSource">Source type</typeparam>
        /// <typeparam name="TTarget">Target type</typeparam>
        /// <param name="source">Source object</param>
        /// <param name="target">Target object</param>
        void MapInstance<TSource, TTarget>(TSource source, TTarget target);

    }
}
