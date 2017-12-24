// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System.Collections.Generic;
using System.IO;
using System.Linq;
using Coddee.Security;
using Newtonsoft.Json;

namespace Coddee.Services.Configuration
{

    public class ConfigurationFile : IConfigurationFile
    {
        public ConfigurationFile(string name, string path)
        {
            Name = name;
            Path = path;
            _file = new FileInfo(Path);
            _configurations = new Dictionary<string, string>();
        }

        public ConfigurationFile(string name, string path, CryptoProvider cryptoProvider)
            : this(name, path)
        {
            _cryptoProvider = cryptoProvider;
        }

        public ConfigurationFile(string name, string path, CryptoProvider cryptoProvider, Dictionary<string, object> defaultValues)
            : this(name, path, cryptoProvider)
        {
            _defaultValues = defaultValues;
        }

        public string Name { get; }
        public string Path { get; }

        private readonly CryptoProvider _cryptoProvider;
        private readonly Dictionary<string, object> _defaultValues;
        private Dictionary<string, string> _configurations;

        private readonly FileInfo _file;

        private bool _isLoaded;

        public TValue GetValue<TValue>(string key)
        {
            if (!_isLoaded)
                ReadFile();
            return JsonConvert.DeserializeObject<TValue>(_configurations[key]);
        }

        public bool TryGetValue<TValue>(string key, out TValue value)
        {
            if (!_isLoaded)
                ReadFile();

            if (!_configurations.ContainsKey(key))
            {
                value = default(TValue);
                return false;
            }

            value = GetValue<TValue>(key);
            return true;
        }

        public void SetValue<TValue>(string key, TValue value)
        {
            _configurations[key] = JsonConvert.SerializeObject(value);
            UpdateFile();
        }



        public void ReadFile()
        {
            if (!CheckIfFileExistsAndCreate())
            {
                _isLoaded = true;
                return;
            }

            string json = string.Empty;
            using (var sr = _file.OpenText())
            {
                json = sr.ReadToEnd();
            }
            if (_cryptoProvider != null)
            {
                json = _cryptoProvider.Decryptor(json);
            }
            _configurations = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
            _isLoaded = true;
        }

        private bool CheckIfFileExistsAndCreate()
        {
            if (_file.Exists)
                return true;
            if (_defaultValues == null)
            {
                if (!_file.Directory.Exists)
                    _file.Directory.Create();
                _file.Create().Dispose();
                return false;
            }
            using (var sw = _file.CreateText())
            {
                _configurations = new Dictionary<string, string>(_defaultValues.ToDictionary(e => e.Key, e => JsonConvert.SerializeObject(e.Value)));
                sw.WriteLineAsync(GetConfigString());
            }
            return false;
        }

        private void UpdateFile()
        {
            File.WriteAllText(Path, GetConfigString());
        }

        string GetConfigString()
        {
            var json = JsonConvert.SerializeObject(_configurations);
            return _cryptoProvider != null ? _cryptoProvider.Encryptor(json) : json;
        }
    }
}
