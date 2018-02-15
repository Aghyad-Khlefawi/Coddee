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
        /// <inheritdoc />
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
        public void RegisterMap<TSource, TTarget>(Action<TSource, TTarget> convert)
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
                convert((TSource)source, (TTarget)target);
            };
        }

        /// <inheritdoc />
        public void RegisterMap<TSource, TTarget>()
        {
            RegisterAutoMap<TSource, TTarget>();
        }

        /// <summary>
        /// Register mapping information from source to target
        /// </summary>
        /// <typeparam name="TSource">Source type</typeparam>
        /// <typeparam name="TTarget">Target type</typeparam>
        public void RegisterAutoMap<TSource, TTarget>(Action<TSource, TTarget> additionalMapping = null)
        {
            var targetType = typeof(TTarget);
            var sourceType = typeof(TSource);
            MappingInfo<TSource, TTarget> info = new MappingInfo<TSource, TTarget>
            {
                SourceType = sourceType,
                TargetType = targetType
            };


            var sourcePoperties = sourceType.GetTypeInfo().GetProperties();
            var targetPoperties = targetType.GetTypeInfo().GetProperties(e => e.SetMethod != null);

            List<PropertyMapper> props = new List<PropertyMapper>();
            foreach (var targetPoperty in targetPoperties)
            {
                bool sourceNullable = false;
                bool targetNullable = false;

                var sourceProperty =
                    sourcePoperties.FirstOrDefault(
                                                   e => e.Name == targetPoperty.Name &&
                                                        e.PropertyType == targetPoperty.PropertyType);
                if (sourceProperty == null &&
                    targetPoperty.PropertyType.GenericTypeArguments.Any() &&
                    targetPoperty.PropertyType == typeof(Nullable<>).MakeGenericType(targetPoperty.PropertyType.GenericTypeArguments))
                {
                    sourceProperty =
                        sourcePoperties.FirstOrDefault(e => e.Name == targetPoperty.Name &&
                        e.PropertyType == targetPoperty.PropertyType.GenericTypeArguments[0]);
                    targetNullable = true;
                }

                if (sourceProperty == null)
                {
                    sourceProperty =
                        sourcePoperties.FirstOrDefault(e => e.Name == targetPoperty.Name &&
                                                            e.PropertyType.GenericTypeArguments.Any() &&
                                                            e.PropertyType == typeof(Nullable<>).MakeGenericType(targetPoperty.PropertyType));
                    sourceNullable = true;
                }

                if (sourceProperty != null)
                {
                    props.Add(new PropertyMapper
                    {
                        TargetProperty = targetPoperty,
                        SourceProperty = sourceProperty,
                        SourceNullable = sourceNullable,
                        TargetNullable = targetNullable
                    });
                }
            }

            info.Properties = props;

            if (!_mappings.ContainsKey(targetType))
                _mappings[targetType] = new Dictionary<Type, MappingInfo>();
            if (_mappings[targetType].ContainsKey(sourceType))
                info.ManualConvertAction = _mappings[targetType][sourceType].ManualConvertAction;

            info.SetAdditionalMapping(additionalMapping);
            _mappings[targetType][sourceType] = info;
        }

        /// <summary>
        /// Register mapping information between two types
        /// </summary>
        /// <typeparam name="TType1">Type1</typeparam>
        /// <typeparam name="TType2">Type2</typeparam>
        public void RegisterTwoWayMap<TType1, TType2>(Action<TType1, TType2> additionalMappingT1T2 = null, Action<TType2, TType1> additionalMappingT2T1 = null)
        {
            RegisterMap<TType1, TType2>(additionalMappingT1T2);
            RegisterMap<TType2, TType1>(additionalMappingT2T1);
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
            var targetType = target.GetType();
            var sourceType = source.GetType();

            if (!_mappings.ContainsKey(sourceType) || !_mappings[sourceType].ContainsKey(targetType))
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
                if (property.SourceNullable == property.TargetNullable || property.TargetNullable)
                    property.TargetProperty.SetValue(target, property.SourceProperty.GetValue(source));
                else if (property.SourceNullable)
                    property.TargetProperty.SetValue(target, property.SourceProperty.PropertyType.GetTypeInfo().GetDeclaredMethod("GetValueOrDefault").Invoke(source, null));
            }
            info.AdditionalMap?.Invoke(source, target);
        }
    }

    /// <summary>
    /// Hold the information required to map between two types
    /// </summary>
    class MappingInfo : IMappingInfo
    {
        public Type SourceType { get; set; }
        public Type TargetType { get; set; }
        public Action<object, object> ManualConvertAction { get; set; }
        public List<PropertyMapper> Properties { get; set; }
        public Action<object, object> AdditionalMap { get; set; }
        public void SetAdditionalMapping(Action<object, object> map)
        {
            AdditionalMap = map;
        }
    }

    class MappingInfo<TSource, TTarget> : MappingInfo
    {
        public void SetAdditionalMapping(Action<TSource, TTarget> map)
        {
            AdditionalMap = (source, target) => map((TSource)source, (TTarget)target);
        }
    }

    class PropertyMapper
    {
        public PropertyInfo SourceProperty { get; set; }
        public PropertyInfo TargetProperty { get; set; }
        public bool SourceNullable { get; set; }
        public bool TargetNullable { get; set; }
    }

}