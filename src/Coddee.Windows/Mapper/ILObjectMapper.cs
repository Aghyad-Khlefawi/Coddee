// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
        public ILObjectsMapper()
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
            var sourceType = typeof(TSource);
            var targetType = typeof(TTarget);

            if (!_mappings.ContainsKey(sourceType))
                _mappings[sourceType] = new Dictionary<Type, MappingInfo>();

            var mappingInfo = _mappings[sourceType].ContainsKey(targetType)
                ? _mappings[sourceType][targetType]
                : new MappingInfo();

            mappingInfo.ManulaMapper = (s, t) => convert((TSource) s, (TTarget) t);
            mappingInfo.InstanceMapper = (s, t) => convert((TSource) s, (TTarget) t);
            _mappings[sourceType][targetType] = mappingInfo;
        }

        /// <summary>
        /// Register mapping information from source to target
        /// </summary>
        /// <typeparam name="TSource">Source type</typeparam>
        /// <typeparam name="TTarget">Target type</typeparam>
        public void RegisterMap<TSource, TTarget>() where TTarget : new()
        {
            var sourceType = typeof(TSource);
            var targetType = typeof(TTarget);

            var availableProperties = new Dictionary<PropertyInfo, PropertyInfo>();

            var sourcePoperties = sourceType.GetProperties();
            var targetPoperties = targetType.GetProperties().Where(e => e.SetMethod != null);


            foreach (var targetPoperty in targetPoperties)
            {
                var sourceProperty =
                    sourcePoperties.FirstOrDefault(
                                                   e => e.Name == targetPoperty.Name &&
                                                        e.PropertyType == targetPoperty.PropertyType);
                if (sourceProperty != null)
                    availableProperties[targetPoperty] = sourceProperty;
            }

            var singleItemDelegate = GenerateSingleItemDelegate<TSource, TTarget>(availableProperties);
            var collectionDelegate = GenerateCollectionDelegate<TSource, TTarget>(availableProperties);
            var instanfceDelegate = GenerateInstanceDelegate<TSource, TTarget>(availableProperties);

            if (!_mappings.ContainsKey(sourceType))
                _mappings[sourceType] = new Dictionary<Type, MappingInfo>();

            var mappingInfo = _mappings[sourceType].ContainsKey(targetType)
                ? _mappings[sourceType][targetType]
                : new MappingInfo();

            mappingInfo.SingleMapper = s => singleItemDelegate((TSource) s);
            mappingInfo.CollectionMapper = s => collectionDelegate((IList<TSource>) s);
            mappingInfo.InstanceMapper = (s, t) => instanfceDelegate((TSource) s, (TTarget) t);
            _mappings[sourceType][targetType] = mappingInfo;
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
        /// <param name="source">object to map</param>
        /// <returns>A new object of the target type with the source object values</returns>
        public TTarget Map<TTarget>(object source) where TTarget : new()
        {
            var sourceType = source.GetType();
            var targetType = typeof(TTarget);
            if (!_mappings.ContainsKey(sourceType) || !_mappings[sourceType].ContainsKey(targetType))
                throw new
                    InvalidOperationException($"The mapping from {sourceType.Name} to {targetType.Name} was not defined ");

            var mappings = _mappings[sourceType][targetType];
            if (mappings.ManulaMapper != null)
            {
                var target = new TTarget();
                mappings.ManulaMapper(source, target);
                return target;
            }
            if (mappings.SingleMapper != null)
            {
                return (TTarget) mappings.SingleMapper(source);
            }
            throw new
                InvalidOperationException($"The mapping from {sourceType.Name} to {targetType.Name} was not defined ");
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
                        InvalidOperationException($"The mapping from {sourceType.Name} to {targetType.Name} was not defined ");

                var mappings = _mappings[sourceType][targetType];
                if (mappings.ManulaMapper != null)
                {
                    var result = new TTarget[source.Count];
                    for (int i = 0; i < source.Count; i++)
                    {
                        var target = new TTarget();
                        mappings.ManulaMapper(source, target);
                        result[i] = target;
                    }
                    return result;
                }
                if (mappings.CollectionMapper != null)
                {
                    return (TTarget[]) mappings.CollectionMapper(source);
                }
                throw new
                    InvalidOperationException($"The mapping from {sourceType.Name} to {targetType.Name} was not defined ");
            }
            return null;
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
            }
            else
                throw new
                    InvalidOperationException($"The mapping from {source.GetType().Name} to {typeof(TTarget).Name} was not defined ");
        }

        /// <summary>
        /// Generate dynamic method for instance mapping
        /// </summary>
        Action<TSource, TTarget> GenerateInstanceDelegate<TSource, TTarget>(
            Dictionary<PropertyInfo, PropertyInfo> properties)
        {
            var sourceType = typeof(TSource);
            var targetType = typeof(TTarget);
            var targetConstrucotr = targetType.GetConstructor(Type.EmptyTypes);

            if (targetConstrucotr == null)
                throw new ArgumentException("Target type must have a parameterless constructor");


            var dm = new DynamicMethod($"_MAP_{sourceType.Name}_{targetType.Name}",
                                       typeof(void),
                                       new[] {sourceType, targetType},
                                       typeof(ILObjectsMapper).Module);
            var il = dm.GetILGenerator();

            var source = il.DeclareLocal(sourceType);
            var target = il.DeclareLocal(targetType);

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Stloc, source);

            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Stloc, target);

            foreach (var propertyPair in properties)
            {
                il.Emit(OpCodes.Ldloc, target);
                il.Emit(OpCodes.Ldloc, source);
                il.Emit(OpCodes.Call, propertyPair.Value.GetMethod);
                il.Emit(OpCodes.Call, propertyPair.Key.SetMethod);
            }

            il.Emit(OpCodes.Ret);

            return (Action<TSource, TTarget>) dm.CreateDelegate(typeof(Action<TSource, TTarget>));
        }

        /// <summary>
        /// Generate dynamic method for single item mapping
        /// </summary>
        Func<TSource, TTarget> GenerateSingleItemDelegate<TSource, TTarget>(
            Dictionary<PropertyInfo, PropertyInfo> properties)
        {
            var sourceType = typeof(TSource);
            var targetType = typeof(TTarget);
            var targetConstrucotr = targetType.GetConstructor(Type.EmptyTypes);

            if (targetConstrucotr == null)
                throw new ArgumentException("Target type must have a parameterless constructor");


            var dm = new DynamicMethod($"_MAP_{sourceType.Name}_{targetType.Name}",
                                       targetType,
                                       new[] {sourceType},
                                       typeof(ILObjectsMapper).Module);
            var il = dm.GetILGenerator();

            il.DeclareLocal(sourceType);
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Stloc_0);

            il.DeclareLocal(targetType);
            il.Emit(OpCodes.Newobj, targetConstrucotr);
            il.Emit(OpCodes.Stloc_1);

            foreach (var propertyPair in properties)
            {
                il.Emit(OpCodes.Ldloc_1);
                il.Emit(OpCodes.Ldloc_0);
                il.Emit(OpCodes.Call, propertyPair.Value.GetMethod);
                il.Emit(OpCodes.Call, propertyPair.Key.SetMethod);
            }

            il.Emit(OpCodes.Ldloc_1);
            il.Emit(OpCodes.Ret);

            return (Func<TSource, TTarget>) dm.CreateDelegate(typeof(Func<TSource, TTarget>));
        }

        /// <summary>
        /// Generate dynamic method for collection mapping
        /// </summary>
        Func<IList<TSource>, TTarget[]> GenerateCollectionDelegate<TSource, TTarget>(
            Dictionary<PropertyInfo, PropertyInfo> properties)
        {
            var sourceType = typeof(TSource);
            var targetType = typeof(TTarget);
            var targetCollectionType = typeof(TTarget[]);

            var targetConstrucotr = targetType.GetConstructor(Type.EmptyTypes);
            if (targetConstrucotr == null)
                throw new ArgumentException("Target type must have a parameterless constructor");


            var targetCollectionConstrucotr = targetCollectionType.GetConstructor(new[] {typeof(int)});
            if (targetCollectionConstrucotr == null)
                throw new ArgumentException();

            var dm = new DynamicMethod($"_MAP_LIST_{sourceType.Name}_{targetType.Name}",
                                       typeof(TTarget[]),
                                       new[] {typeof(IList<TSource>)},
                                       typeof(ILObjectsMapper).Module);
            var il = dm.GetILGenerator();


            var loopStart = il.DefineLabel();
            var loopExit = il.DefineLabel();


            // Store argument list in local
            var source = il.DeclareLocal(typeof(IList<TSource>));
            var result = il.DeclareLocal(typeof(TTarget[]));
            var count = il.DeclareLocal(typeof(IList<TSource>));
            var index = il.DeclareLocal(typeof(IList<TSource>));
            var sourceItem = il.DeclareLocal(typeof(IList<TSource>));
            var targetItem = il.DeclareLocal(typeof(IList<TSource>));


            il.Emit(OpCodes.Ldarg_0);
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
            foreach (var propertyPair in properties)
            {
                il.Emit(OpCodes.Ldloc, targetItem);
                il.Emit(OpCodes.Ldloc, sourceItem);
                il.Emit(OpCodes.Call, propertyPair.Value.GetMethod);
                il.Emit(OpCodes.Call, propertyPair.Key.SetMethod);
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

            return (Func<IList<TSource>, TTarget[]>) dm.CreateDelegate(typeof(Func<IList<TSource>, TTarget[]>));
        }
    }

    class MappingInfo
    {
        public Func<object, object> SingleMapper { get; set; }
        public Func<object, object> CollectionMapper { get; set; }
        public Action<object, object> ManulaMapper { get; set; }
        public Action<object, object> InstanceMapper { get; set; }
    }
}