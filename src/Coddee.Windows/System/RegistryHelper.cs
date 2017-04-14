// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Microsoft.Win32;

namespace Coddee.Windows.System
{
    public static class RegistryHelper
    {
        /// <summary>
        /// Reads a reigtery object and try to converted to a POCO object
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
        /// Reads a reigtery object and returns it's values as a dictionary
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="keyPath"></param>
        /// <returns></returns>
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
            var key = Registry.LocalMachine.OpenSubKey(keyPath, RegistryKeyPermissionCheck.ReadWriteSubTree) ??
                      Registry.LocalMachine.CreateSubKey(keyPath, RegistryKeyPermissionCheck.ReadWriteSubTree);

            foreach (var property in typeof(T).GetProperties())
            {
                var stringValue = property.GetValue(value)?.ToString() ?? "NULL";
                key.SetValue(property.Name, stringValue);
            }
        }
    }
    public class RegistryKeyNotFoundException : Exception
    {
        public RegistryKeyNotFoundException()
        {
        }

        public RegistryKeyNotFoundException(string message) : base(message)
        {
        }

        public RegistryKeyNotFoundException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected RegistryKeyNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }

}
