// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Security.AccessControl;
using Microsoft.Win32;

namespace Coddee.Windows
{
    /// <summary>
    /// Helper class for working with the windows registry
    /// </summary>
    public static class RegistryHelper
    {
        /// <summary>
        /// Reads a registry object and try to converted to a POCO object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="keyPath"></param>
        /// <returns></returns>
        public static T ReadKeyValues<T>(string keyPath) where T : new()
        {
            var key = Registry.LocalMachine.OpenSubKey(keyPath);
            if (key == null)
                throw new RegistryKeyNotFoundException($"Key {keyPath} was not found.");
            var res = new T();
            foreach (var property in typeof(T).GetProperties())
            {
                var stringValue = key.GetValue(property.Name).ToString();
                if (stringValue != "NULL")
                {
                    var parse = property.PropertyType.Name != "Nullable`1" ?
                        property.PropertyType.GetMethod("Parse", new[] { typeof(string) }) :
                        property.PropertyType.GenericTypeArguments[0].GetMethod("Parse", new[] { typeof(string) });

                    var value = parse?.Invoke(null, new object[] { stringValue }) ?? stringValue;
                    property.SetValue(res, value);
                }
                else
                    property.SetValue(res, null);
            }
            return res;
        }

        /// <summary>
        /// Reads a registry object and returns it's values as a dictionary
        /// </summary>
        public static Dictionary<string, string> ReadKeyValues(string keyPath)
        {
            var key = Registry.LocalMachine.OpenSubKey(keyPath);
            if (key == null)
                throw new RegistryKeyNotFoundException($"Key {keyPath} was not found.");
            return key.GetValueNames().ToDictionary(e => e, e => key.GetValue(e).ToString());
        }

        /// <summary>
        /// Store a POCO object in a registry key
        /// </summary>
        /// <returns></returns>
        public static void WriteKeyValues<T>(string keyPath, T value)
        {
#if NET45
            var key = Registry.LocalMachine.OpenSubKey(keyPath, RegistryKeyPermissionCheck.ReadWriteSubTree) ??
                      Registry.LocalMachine.CreateSubKey(keyPath, RegistryKeyPermissionCheck.ReadWriteSubTree);

#else
            var key = Registry.LocalMachine.OpenSubKey(keyPath, RegistryRights.ReadPermissions) ??
                      Registry.LocalMachine.CreateSubKey(keyPath, true);
#endif

            foreach (var property in typeof(T).GetProperties())
            {
                var stringValue = property.GetValue(value)?.ToString() ?? "NULL";
                key.SetValue(property.Name, stringValue);
            }
        }
    }

    /// <summary>
    /// An exception the occurs while dealing with windows registry keys.
    /// </summary>
    public class RegistryKeyNotFoundException : Exception
    {
        /// <inheritdoc />
        public RegistryKeyNotFoundException()
        {
        }

        /// <inheritdoc />
        public RegistryKeyNotFoundException(string message) : base(message)
        {
        }

        /// <inheritdoc />
        public RegistryKeyNotFoundException(string message, Exception innerException) : base(message, innerException)
        {
        }

    }

}
