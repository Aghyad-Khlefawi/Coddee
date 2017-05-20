// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Coddee
{
    /// <summary>
    /// Basic object mapper for PCL implementation
    /// </summary>
    public class BasicObjectMapper : IObjectMapper
    {
        public BasicObjectMapper()
        {
            _mappings = new Dictionary<Type, Dictionary<Type, MappingInfo>>();
        }

        /// <summary>
        /// A dictionary to hold the mapping information
        /// The key is the target type, the inner dictionary key is the source type
        /// </summary>
        private readonly Dictionary<Type, Dictionary<Type, MappingInfo>> _mappings;

        /// <summary>
        /// Register a manual mapping information from source to target
        /// </summary>
        /// <typeparam name="TSource">Source type</typeparam>
        /// <typeparam name="TTarget">Target type</typeparam>
        public void RegisterMap<TSource, TTarget>(Action<TSource, TTarget> convert) where TTarget : new()
        {
            var targetType = typeof(TTarget);
            var sourceType = typeof(TSource);

            if (!_mappings.ContainsKey(targetType))
                _mappings[targetType] = new Dictionary<Type, MappingInfo>();
            if (!_mappings[targetType].ContainsKey(sourceType))
                _mappings[targetType][sourceType] = new MappingInfo
                {
                    SourceType = sourceType,
                    TargetType = targetType
                };

            _mappings[targetType][sourceType].ManualConvertAction = (source, target) =>
            {
                convert((TSource) source, (TTarget) target);
            };
        }

        /// <summary>
        /// Register mapping information from source to target
        /// </summary>
        /// <typeparam name="TSource">Source type</typeparam>
        /// <typeparam name="TTarget">Target type</typeparam>
        public void RegisterMap<TSource, TTarget>() where TTarget : new()
        {
            var targetType = typeof(TTarget);
            var sourceType = typeof(TSource);
            var info = new MappingInfo
            {
                SourceType = sourceType,
                TargetType = targetType
            };

            var propertiesInfo = new List<MapPropertyInfo>();

            var sourcePoperties = sourceType.GetTypeInfo().GetProperties();
            var targetPoperties = targetType.GetTypeInfo().GetProperties(e => e.SetMethod != null);

            foreach (var targetProperty in targetPoperties)
            {
                var sourceProperty =
                    sourcePoperties.FirstOrDefault(e => e.Name == targetProperty.Name &&
                                                        e.PropertyType == targetProperty.PropertyType);
                if (sourceProperty != null)
                {
                    propertiesInfo.Add(new MapPropertyInfo
                    {
                        SourceProperty = sourceProperty,
                        TargetProperty = targetProperty
                    });
                }
            }
            info.Properties = propertiesInfo;

            if (!_mappings.ContainsKey(targetType))
                _mappings[targetType] = new Dictionary<Type, MappingInfo>();
            if (_mappings[targetType].ContainsKey(sourceType))
                info.ManualConvertAction = _mappings[targetType][sourceType].ManualConvertAction;

            _mappings[targetType][sourceType] = info;
        }

        /// <summary>
        /// Register mapping information between two types
        /// </summary>
        /// <typeparam name="TType1">Type1</typeparam>
        /// <typeparam name="TType2">Type2</typeparam>
        public void RegisterTwoWayMap<TType1, TType2>() where TType1 : new() where TType2 : new()
        {
            RegisterMap<TType1, TType2>();
            RegisterMap<TType2, TType1>();
        }

        /// <summary>
        /// Map an object to a specific type
        /// </summary>
        /// <typeparam name="TTarget">Target type</typeparam>
        /// <param name="item">object to map</param>
        /// <returns>A new object of the target type with the source object values</returns>
        public TTarget Map<TTarget>(object item) where TTarget : new()
        {
            var newItem = new TTarget();
            MapInstance(item, newItem);
            return newItem;
        }

        /// <summary>
        /// Map a collection of objects to a specific type
        /// </summary>
        /// <typeparam name="TTarget">Target type</typeparam>
        /// <param name="source">the source collection</param>
        /// <returns>A new collection of the target type with the source object values</returns>
        public IEnumerable<TTarget> MapCollection<TTarget>(IList source) where TTarget : new()
        {
            var res = new List<TTarget>();
            foreach (var item in source)
                res.Add(Map<TTarget>(item));
            return res;
        }

        /// <summary>
        /// Maps the values of the source object to the target object
        /// </summary>
        /// <typeparam name="TSource">Source type</typeparam>
        /// <typeparam name="TTarget">Target type</typeparam>
        /// <param name="source">Source object</param>
        /// <param name="target">Target object</param>
        public void MapInstance<TSource, TTarget>(TSource source, TTarget target)
        {
            var targetType = typeof(TTarget);
            var sourceType = typeof(TSource);

            if (!_mappings.ContainsKey(targetType) || !_mappings[sourceType].ContainsKey(targetType))
                throw new InvalidOperationException(
                    $"No mapping is defined between {sourceType.Name} and {targetType.Name}");

            var info = _mappings[sourceType][targetType];
            if (info.ManualConvertAction != null)
            {
                info.ManualConvertAction(source, target);
                return;
            }
            foreach (var property in info.Properties)
            {
                if (!RequireMapping(property.SourceProperty))
                    property.TargetProperty.SetValue(target, property.SourceProperty.GetValue(source));
                else
                {
                    var item = (TTarget)Activator.CreateInstance(property.TargetProperty.PropertyType);
                    MapInstance((TSource)property.SourceProperty.GetValue(source),item);
                    property.TargetProperty.SetValue(target, item);
                }
            }
        }

        private bool RequireMapping(PropertyInfo property)
        {
            if (!property.PropertyType.GetTypeInfo().IsClass)
                return false;

            if (_mappings.ContainsKey(property.PropertyType))
                return true;

            return false;
        }


        }

    /// <summary>
    /// Hold the information required to map between two types
    /// </summary>
    class MappingInfo
    {
        public Type SourceType { get; set; }
        public Type TargetType { get; set; }
        public Action<object, object> ManualConvertAction { get; set; }
        public List<MapPropertyInfo> Properties { get; set; }
    }

    /// <summary>
    /// Hold the information required to map between to properties
    /// </summary>
    class MapPropertyInfo
    {
        public PropertyInfo SourceProperty { get; set; }
        public PropertyInfo TargetProperty { get; set; }
    }
}