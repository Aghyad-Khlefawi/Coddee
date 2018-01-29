// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Coddee
{
    /// <summary>
    /// A helper class for working with Reflection.
    /// </summary>
    public static class ReflectionHelper
    {
        /// <summary>
        /// Returns the declared and base types properties for type
        /// </summary>
        public static IEnumerable<PropertyInfo> GetProperties(
            this TypeInfo type,
            Func<PropertyInfo, bool> predicate = null)
        {
            List<PropertyInfo> properties = new List<PropertyInfo>();
            properties.AddRange(predicate == null ? type.DeclaredProperties : type.DeclaredProperties.Where(predicate));
            if (type.BaseType != null && type.BaseType.Name != typeof(object).Name)
                properties.AddRange(GetProperties(type.BaseType.GetTypeInfo()));
            return properties;
        }


        /// <summary>
        /// Returns a property defined in a type or one of its base types.
        /// </summary>
        /// <param name="type">The target type</param>
        /// <param name="name">The property name</param>
        public static PropertyInfo GetProperty(
            this TypeInfo type,
            string name)
        {
            foreach (var property in type.DeclaredProperties)
            {
                if (property.Name == name)
                    return property;
            }
            if (type.BaseType != null && type.BaseType.Name != typeof(object).Name)
                return type.BaseType.GetTypeInfo().GetProperty(name);
            return null;
        }
    }
}