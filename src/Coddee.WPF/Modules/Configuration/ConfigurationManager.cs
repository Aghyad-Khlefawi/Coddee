// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using System.Collections.Generic;
using System.IO;
using Coddee.Crypto;
using Coddee.WPF.Modules.Interfaces;
using Newtonsoft.Json;

namespace Coddee.WPF.Configuration
{
    public class BuiltInConfigurationKeys
    {
        public const string SQLDBConnection = nameof(SQLDBConnection);
    }


    /// <summary>
    /// service for storing application configurations in a file
    /// </summary>
    public class ConfigurationManager : IConfigurationManager
    {
        private FileInfo _file;
        private Dictionary<string, string> _configurations;
        private bool _encrpyt;
        private string _key;
        private Dictionary<string, object> _defaultValues;

        /// <summary>
        /// Initialize the configurations manager
        /// On calling this method the configurations will be created or loaded if it exists
        /// </summary>
        /// <param name="configFile">The file path without the extension</param>
        /// <param name="defaultValues"></param>
        public void Initialize(string configFile = "config", Dictionary<string, object> defaultValues = null)
        {
            _file = new FileInfo(configFile + ".cfg");
            _defaultValues = defaultValues;
        }

        /// <summary>
        /// Read the configuration file values
        /// </summary>
        public void ReadFile()
        {
            if (!_file.Exists)
            {
                _configurations = new Dictionary<string, string>();
                _file.Create().Dispose();
                if (_defaultValues != null)
                {
                    foreach (var defaultValue in _defaultValues)
                    {
                        SetValue(defaultValue.Key, defaultValue.Value);
                    }
                }
                UpdateFile();
            }
            using (var fs = _file.OpenRead())
            {
                try
                {
                    if (!_encrpyt)
                    {
                        using (var sr = new StreamReader(fs))
                        {
                           _configurations = JsonConvert.DeserializeObject<Dictionary<string, string>>(sr.ReadToEnd());
                        }
                    }
                    else
                    {
                        var buffer = new byte[fs.Length];
                        fs.Read(buffer, 0, buffer.Length);
                         _configurations = JsonConvert.DeserializeObject<Dictionary<string, string>>(EncryptionHelper.Decrypt(buffer,_key));
                    }
                }
                catch (Exception e)
                {
                    throw new FileFormatException("Couldn't read the configurations file", e);
                }
            }
        }

        /// <summary>
        /// Upsert a configuration value
        /// </summary>
        /// <param name="key">The configuration key</param>
        /// <param name="value">The new value</param>
        public void SetValue(string key, object value)
        {
            _configurations[key] = JsonConvert.SerializeObject(value);
            UpdateFile();
        }

        /// <summary>
        /// Try to read configuration value
        /// </summary>
        /// <typeparam name="TResult">The result value type</typeparam>
        /// <param name="key">The configuration key</param>
        /// <param name="value">The new value</param>
        public bool TryGetValue<TResult>(string key, out TResult value)
        {
            if (_configurations.ContainsKey(key))
            {
                value = JsonConvert.DeserializeObject<TResult>(_configurations[key]);
                return true;
            }
            value = default(TResult);
            return false;
        }

        /// <summary>
        /// When called the configuration manager will use encrpyted configuration file
        /// </summary>
        /// <param name="key">Encrpytion key</param>
        public void SetEncrpytion(string key)
        {
            _encrpyt = true;
            _key = key;
        }

        /// <summary>
        /// Update the configuration files with the new values
        /// </summary>
        private void UpdateFile()
        {
            using (var fs = _file.OpenWrite())
            {
                if (!_encrpyt)
                {
                    using (var sw = new StreamWriter(fs))
                    {
                        sw.WriteLine(JsonConvert.SerializeObject(_configurations));
                    }
                }
                else
                {
                    var buffer = EncryptionHelper.EncryptString(JsonConvert.SerializeObject(_configurations), _key);
                    fs.Write(buffer,0,buffer.Length);
                }
            }
        }
    }
}