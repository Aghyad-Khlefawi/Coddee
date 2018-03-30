// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;

namespace Coddee.Windows.Mapper
{
    /// <summary>
    /// An object mapper for that uses generate IL to map object 
    /// using dynamic methods created at runtime,
    /// The current implementation only supports one level of mapping but it's very fast for simple POCO objects mapping
    /// </summary>
    public class ILObjectsMapper : IObjectMapper
    {
        /// <inheritdoc />
        public ILObjectsMapper()
        {
            _mappings = new Dictionary<Type, Dictionary<Type, ILMappingInfo>>();
        }

        /// <summary>
        /// A dictionary to hold the mapping information
        /// The key is the target type, the inner dictionary key is the source type
        /// </summary>
        private readonly Dictionary<Type, Dictionary<Type, ILMappingInfo>> _mappings;


        /// <summary>
        /// Register a manual mapping information from source to target
        /// </summary>
        /// <typeparam name="TSource">Source type</typeparam>
        /// <typeparam name="TTarget">Target type</typeparam>
        public void RegisterMap<TSource, TTarget>(Action<TSource, TTarget> convert)
        {
            var sourceType = typeof(TSource);
            var targetType = typeof(TTarget);

            if (!_mappings.ContainsKey(sourceType))
                _mappings[sourceType] = new Dictionary<Type, ILMappingInfo>();

            ILMappingInfo ilMappingInfo = _mappings[sourceType].ContainsKey(targetType)
                ? _mappings[sourceType][targetType]
                : new IlMappingInfo<TSource, TTarget>();

            ilMappingInfo.ManualMapper = (s, t) => convert((TSource)s, (TTarget)t);
            ilMappingInfo.InstanceMapper = (s, t) => convert((TSource)s, (TTarget)t);
            _mappings[sourceType][targetType] = ilMappingInfo;

        }


        /// <summary>
        /// Register mapping information from source to target
        /// </summary>
        /// <typeparam name="TSource">Source type</typeparam>
        /// <typeparam name="TTarget">Target type</typeparam>
        public void RegisterAutoMap<TSource, TTarget>(Action<TSource, TTarget> additionalMapping = null)
        {
            var sourceType = typeof(TSource);
            var targetType = typeof(TTarget);


            var sourcePoperties = sourceType.GetProperties();
            var targetPoperties = targetType.GetProperties().Where(e => e.SetMethod != null);

            List<PropertyMapper> props = new List<PropertyMapper>();
            foreach (var targetPoperty in targetPoperties)
            {
                bool sourceNullable = false;
                bool targetNullable = false;

                var sourceProperty =
                    sourcePoperties.FirstOrDefault(
                                                   e => e.Name == targetPoperty.Name &&
                                                        e.PropertyType == targetPoperty.PropertyType);
                try
                {
                    if (sourceProperty == null &&
                                targetPoperty.PropertyType.GenericTypeArguments.Any() &&
                                !targetPoperty.PropertyType.GenericTypeArguments[0].IsClass &&
                                targetPoperty.PropertyType == typeof(Nullable<>).MakeGenericType(targetPoperty.PropertyType.GenericTypeArguments))
                    {
                        sourceProperty =
                            sourcePoperties.FirstOrDefault(e => e.Name == targetPoperty.Name &&
                            e.PropertyType == targetPoperty.PropertyType.GenericTypeArguments[0]);
                        targetNullable = true;
                    }

                    if (sourceProperty == null)
                    {
                        sourceProperty = sourcePoperties.FirstOrDefault(e => e.Name == targetPoperty.Name &&
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
                catch
                {
                }
            }

            var targetConstrucotr = targetType.GetConstructor(Type.EmptyTypes);

            Func<TSource, TTarget> singleItemDelegate = null;
            Func<ILMappingInfo, IList<TSource>, TTarget[]> collectionDelegate = null;

            if (!_mappings.ContainsKey(sourceType))
                _mappings[sourceType] = new Dictionary<Type, ILMappingInfo>();

            IlMappingInfo<TSource, TTarget> ilMappingInfo = _mappings[sourceType].ContainsKey(targetType)
                                  ? (IlMappingInfo<TSource, TTarget>)_mappings[sourceType][targetType]
                                  : new IlMappingInfo<TSource, TTarget>();

            ilMappingInfo?.SetAdditionalMapping(additionalMapping);

            if (targetConstrucotr != null)
            {
                singleItemDelegate = GenerateSingleItemDelegate<TSource, TTarget>(props);
                collectionDelegate = GenerateCollectionDelegate<TSource, TTarget>(props, ilMappingInfo);
            }

            var instanfceDelegate = GenerateInstanceDelegate<TSource, TTarget>(props);



            if (targetConstrucotr != null)
            {
                ilMappingInfo.SingleMapper = s => singleItemDelegate((TSource)s);
                ilMappingInfo.CollectionMapper = s => collectionDelegate(ilMappingInfo, (IList<TSource>)s);
            }
            ilMappingInfo.InstanceMapper = (s, t) => instanfceDelegate((TSource)s, (TTarget)t);
            _mappings[sourceType][targetType] = ilMappingInfo;
        }

        /// <summary>
        /// Register mapping information between two types
        /// </summary>
        /// <typeparam name="TType1">Type1</typeparam>
        /// <typeparam name="TType2">Type2</typeparam>
        public void RegisterTwoWayMap<TType1, TType2>(Action<TType1, TType2> additionalMappingT1T2 = null, Action<TType2, TType1> additionalMappingT2T1 = null)
        {
            RegisterAutoMap<TType1, TType2>(additionalMappingT1T2);
            RegisterAutoMap<TType2, TType1>(additionalMappingT2T1);
        }

        /// <summary>
        /// Map an object to a specific type
        /// </summary>
        /// <typeparam name="TTarget">Target type</typeparam>
        /// <param name="source">object to map</param>
        /// <returns>A new object of the target type with the source object values</returns>
        public TTarget Map<TTarget>(object source) where TTarget : new()
        {
            var sourceType = source.GetType();
            var targetType = typeof(TTarget);
            if (!_mappings.ContainsKey(sourceType) || !_mappings[sourceType].ContainsKey(targetType))
                throw new
                    InvalidOperationException($"The mapping from {sourceType.Name} to {targetType.Name} was not defined or the target type doesn't have a parameterless constructor");

            var mappings = _mappings[sourceType][targetType];
            if (mappings.ManualMapper != null)
            {
                var target = new TTarget();
                mappings.ManualMapper(source, target);
                mappings.AdditionalMap?.Invoke(source, target);
                return target;
            }
            if (mappings.SingleMapper != null)
            {
                var res = (TTarget)mappings.SingleMapper(source);
                mappings.AdditionalMap?.Invoke(source, res);
                return res;
            }
            throw new
                InvalidOperationException($"The mapping from {sourceType.Name} to {targetType.Name} was not defined or the target type doesn't have a parameterless constructor");
        }

        /// <inheritdoc />
        public void RegisterMap<TSource, TTarget>()
        {
            RegisterAutoMap<TSource, TTarget>();
        }

        /// <summary>
        /// Map a collection of objects to a specific type
        /// </summary>
        /// <typeparam name="TTarget">Target type</typeparam>
        /// <param name="source">the source collection</param>
        /// <returns>A new collection of the target type with the source object values</returns>
        public IEnumerable<TTarget> MapCollection<TTarget>(IList source) where TTarget : new()
        {
            if (source != null && source.Count > 0)
            {
                var sourceType = source[0].GetType();
                var targetType = typeof(TTarget);
                if (!_mappings.ContainsKey(sourceType) || !_mappings[sourceType].ContainsKey(targetType))
                    throw new
                        InvalidOperationException($"The mapping from {sourceType.Name} to {targetType.Name} was not defined or the target type doesn't have a parameterless constructor");

                var mappings = _mappings[sourceType][targetType];
                if (mappings.ManualMapper != null)
                {
                    var result = new TTarget[source.Count];
                    for (int i = 0; i < source.Count; i++)
                    {
                        var target = new TTarget();
                        mappings.ManualMapper(source[i], target);
                        mappings.AdditionalMap?.Invoke(source[i], target);
                        result[i] = target;
                    }
                    return result;
                }
                if (mappings.CollectionMapper != null)
                {
                    return (TTarget[])mappings.CollectionMapper(source);
                }
                throw new
                    InvalidOperationException($"The mapping from {sourceType.Name} to {targetType.Name} was not defined or the target type doesn't have a parameterless constructor");
            }
            return new List<TTarget>();
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
            var sourceType = source.GetType();
            var targetType = typeof(TTarget);
            if (!_mappings.ContainsKey(sourceType) || !_mappings[sourceType].ContainsKey(targetType))
                throw new
                    InvalidOperationException($"The mapping from {sourceType.Name} to {targetType.Name} was not defined ");

            var mappings = _mappings[sourceType][targetType];
            if (mappings.InstanceMapper != null)
            {
                mappings.InstanceMapper(source, target);
                if (mappings.AdditionalMap != null)
                    mappings.ExecuteAdditionalMap(source, target);
            }
            else
                throw new
                    InvalidOperationException($"The mapping from {source.GetType().Name} to {typeof(TTarget).Name} was not defined ");
        }

        /// <summary>
        /// Generate dynamic method for instance mapping
        /// </summary>
        Action<TSource, TTarget> GenerateInstanceDelegate<TSource, TTarget>(
            List<PropertyMapper> properties)
        {
            var sourceType = typeof(TSource);
            var targetType = typeof(TTarget);

            var dm = new DynamicMethod($"_MAP_{sourceType.Name}_{targetType.Name}",
                                       typeof(void),
                                       new[] { sourceType, targetType },
                                       typeof(TTarget).Module);
            var il = dm.GetILGenerator();

            var source = il.DeclareLocal(sourceType);
            var target = il.DeclareLocal(targetType);

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Stloc, source);

            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Stloc, target);

            foreach (var property in properties)
            {
                EmitPropertyMap(property, il, source, target);
            }


            il.Emit(OpCodes.Ret);

            return (Action<TSource, TTarget>)dm.CreateDelegate(typeof(Action<TSource, TTarget>));
        }

        /// <summary>
        /// Generate dynamic method for single item mapping
        /// </summary>
        Func<TSource, TTarget> GenerateSingleItemDelegate<TSource, TTarget>(
            List<PropertyMapper> properties)
        {
            var sourceType = typeof(TSource);
            var targetType = typeof(TTarget);
            var targetConstrucotr = targetType.GetConstructor(Type.EmptyTypes);

            if (targetConstrucotr == null)
                throw new ArgumentException("Target type must have a parameterless constructor");


            var dm = new DynamicMethod($"_MAP_{sourceType.Name}_{targetType.Name}",
                                       targetType,
                                       new[] { sourceType },
                                       typeof(TTarget).Module);
            var il = dm.GetILGenerator();

            var source = il.DeclareLocal(sourceType);
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Stloc, source);

            var target = il.DeclareLocal(targetType);
            il.Emit(OpCodes.Newobj, targetConstrucotr);
            il.Emit(OpCodes.Stloc, target);

            foreach (var property in properties)
            {
                EmitPropertyMap(property, il, source, target);
            }

            il.Emit(OpCodes.Ldloc_1);
            il.Emit(OpCodes.Ret);

            return (Func<TSource, TTarget>)dm.CreateDelegate(typeof(Func<TSource, TTarget>));
        }

        private static void EmitPropertyMap(PropertyMapper propertyPair, ILGenerator il, LocalBuilder sourceVar, LocalBuilder targetVar)
        {

            if (propertyPair.SourceNullable == propertyPair.TargetNullable)
            {
                il.Emit(OpCodes.Ldloc, targetVar);
                il.Emit(OpCodes.Ldloc, sourceVar);
                il.Emit(OpCodes.Call, propertyPair.SourceProperty.GetMethod);
                il.Emit(OpCodes.Call, propertyPair.TargetProperty.SetMethod);
            }
            else if (propertyPair.TargetNullable)
            {
                il.Emit(OpCodes.Ldloc, targetVar);
                il.Emit(OpCodes.Ldloc, sourceVar);
                il.Emit(OpCodes.Callvirt, propertyPair.SourceProperty.GetMethod);
                il.Emit(OpCodes.Box, propertyPair.TargetProperty.PropertyType);
                il.Emit(OpCodes.Call, propertyPair.TargetProperty.SetMethod);
            }
            else if (propertyPair.SourceNullable)
            {
                var hasValue = propertyPair.SourceProperty.PropertyType.GetMethod("get_HasValue");
                if (hasValue != null)
                {
                    var endLabel = il.DefineLabel();
                    var sourceProp = il.DeclareLocal(propertyPair.SourceProperty.PropertyType);
                    var hasValueResult = il.DeclareLocal(hasValue.ReturnType);

                    il.Emit(OpCodes.Ldloc, sourceVar);
                    il.Emit(OpCodes.Callvirt, propertyPair.SourceProperty.GetMethod);
                    il.Emit(OpCodes.Stloc, sourceProp);
                    il.Emit(OpCodes.Ldloc, sourceProp);
                    il.Emit(OpCodes.Box, propertyPair.SourceProperty.PropertyType);
                    il.Emit(OpCodes.Ldnull);
                    il.Emit(OpCodes.Ceq);
                    il.Emit(OpCodes.Stloc, hasValueResult);
                    il.Emit(OpCodes.Ldloc, hasValueResult);

                    il.Emit(OpCodes.Brtrue, endLabel);

                    il.Emit(OpCodes.Ldloc, targetVar);
                    il.Emit(OpCodes.Ldloc, sourceProp);
                    il.Emit(OpCodes.Box, propertyPair.SourceProperty.PropertyType);
                    il.Emit(OpCodes.Unbox_Any, propertyPair.TargetProperty.PropertyType);
                    il.Emit(OpCodes.Callvirt, propertyPair.TargetProperty.SetMethod);

                    il.MarkLabel(endLabel);
                }
            }

        }

        /// <summary>
        /// Generate dynamic method for collection mapping
        /// </summary>
        Func<ILMappingInfo, IList<TSource>, TTarget[]> GenerateCollectionDelegate<TSource, TTarget>(List<PropertyMapper> properties, ILMappingInfo ilMappingInfo)
        {
            var sourceType = typeof(TSource);
            var targetType = typeof(TTarget);
            var targetCollectionType = typeof(TTarget[]);

            var targetConstrucotr = targetType.GetConstructor(Type.EmptyTypes);
            if (targetConstrucotr == null)
                throw new ArgumentException("Target type must have a parameterless constructor");


            var targetCollectionConstrucotr = targetCollectionType.GetConstructor(new[] { typeof(int) });
            if (targetCollectionConstrucotr == null)
                throw new ArgumentException();

            var dm = new DynamicMethod($"_MAP_LIST_{sourceType.Name}_{targetType.Name}",
                                       typeof(TTarget[]),
                                       new[] { typeof(ILMappingInfo), typeof(IList<TSource>) },
                                       typeof(TTarget).Module);
            var il = dm.GetILGenerator();


            var loopStart = il.DefineLabel();
            var loopExit = il.DefineLabel();


            // Store argument list in local
            var mappingInfoLocal = il.DeclareLocal(typeof(ILMappingInfo));
            var source = il.DeclareLocal(typeof(IList<TSource>));
            var result = il.DeclareLocal(typeof(TTarget[]));
            var count = il.DeclareLocal(typeof(IList<TSource>));
            var index = il.DeclareLocal(typeof(IList<TSource>));
            var sourceItem = il.DeclareLocal(typeof(TSource));
            var targetItem = il.DeclareLocal(typeof(TTarget));

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Stloc, mappingInfoLocal);

            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Stloc, source);

            // Define result array
            ;
            il.Emit(OpCodes.Ldloc, source);
            il.Emit(OpCodes.Call, typeof(ICollection).GetMethod("get_Count"));
            il.Emit(OpCodes.Stloc, count);
            il.Emit(OpCodes.Ldloc, count);
            il.Emit(OpCodes.Newarr, typeof(TTarget));
            il.Emit(OpCodes.Stloc, result);

            il.Emit(OpCodes.Ldc_I4_0);
            il.Emit(OpCodes.Stloc, index);


            // Mark loop start
            il.MarkLabel(loopStart);


            //Get item at index
            il.Emit(OpCodes.Ldloc, source);
            il.Emit(OpCodes.Ldloc, index);
            il.Emit(OpCodes.Call, typeof(IList<TSource>).GetMethod("get_Item"));
            il.Emit(OpCodes.Stloc, sourceItem);


            // Create new item
            il.Emit(OpCodes.Newobj, targetConstrucotr);
            il.Emit(OpCodes.Stloc, targetItem);


            // Map properties
            foreach (var property in properties)
            {
                EmitPropertyMap(property, il, sourceItem, targetItem);
            }


            if (ilMappingInfo.AdditionalMap != null)
            {
                var method = ilMappingInfo.GetType().GetMethod(nameof(ILMappingInfo.ExecuteAdditionalMap));
                if (method != null)
                {
                    il.Emit(OpCodes.Ldloc, mappingInfoLocal);
                    il.Emit(OpCodes.Ldloc, sourceItem);
                    il.Emit(OpCodes.Ldloc, targetItem);
                    il.Emit(OpCodes.Call, method);
                }
            }

            // Add item to list
            il.Emit(OpCodes.Ldloc, result);
            il.Emit(OpCodes.Ldloc, index);
            il.Emit(OpCodes.Ldloc, targetItem);
            il.Emit(OpCodes.Stelem_Ref);

            // increment index
            il.Emit(OpCodes.Ldloc, index);
            il.Emit(OpCodes.Ldc_I4_1);
            il.Emit(OpCodes.Add);
            il.Emit(OpCodes.Stloc, index);

            // Check loop end
            il.Emit(OpCodes.Ldloc, index);
            il.Emit(OpCodes.Ldloc, count);
            il.Emit(OpCodes.Beq, loopExit);

            // Repeat      
            il.Emit(OpCodes.Br, loopStart);

            il.MarkLabel(loopExit);

            il.Emit(OpCodes.Ldloc, result);
            il.Emit(OpCodes.Ret);

            return (Func<ILMappingInfo, IList<TSource>, TTarget[]>)dm.CreateDelegate(typeof(Func<ILMappingInfo, IList<TSource>, TTarget[]>));
        }
    }

    /// <summary>
    /// Provides the information to map between two objects using IL methods
    /// </summary>
    public class ILMappingInfo
    {
        internal Func<object, object> SingleMapper { get; set; }
        internal Func<object, object> CollectionMapper { get; set; }
        internal Action<object, object> ManualMapper { get; set; }
        internal Action<object, object> InstanceMapper { get; set; }

        /// <summary>
        /// Additional mapping action.
        /// </summary>
        public Action<object, object> AdditionalMap { get; private set; }

        /// <summary>
        /// Calls the <see cref="AdditionalMap"/> action.
        /// </summary>
        public void ExecuteAdditionalMap(object source, object target)
        {
            AdditionalMap(source, target);
        }

        /// <summary>
        /// Sets the <see cref="AdditionalMap"/> action.
        /// </summary>
        /// <param name="map"></param>
        public void SetAdditionalMapping(Action<object, object> map)
        {
            AdditionalMap = map;
        }
    }

    class IlMappingInfo<TSource, TTarget> : ILMappingInfo
    {
        public void SetAdditionalMapping(Action<TSource, TTarget> map)
        {
            if (map != null)
                base.SetAdditionalMapping((source, target) => map((TSource)source, (TTarget)target));
        }
    }
}