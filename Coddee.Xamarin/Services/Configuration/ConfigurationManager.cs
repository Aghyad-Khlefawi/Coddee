﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Coddee.Services;
using Coddee.Services.Configuration;
using PCLStorage;

namespace Coddee.Xamarin.Services.Configuration
{
    public class ConfigurationManager : IConfigurationManager
    {
        /// <inheritdoc />
        public ConfigurationManager()
        {
            _configurationFiles = new Dictionary<string, IConfigurationFile>();
        }

        private const string _defaultConfigFile = "config.config";

        private readonly Dictionary<string, IConfigurationFile> _configurationFiles;
        public void Initialize(IConfigurationFile defaultConfigurationFile)
        {
            if (defaultConfigurationFile == null)
            {
                defaultConfigurationFile =
                    new ConfigurationFile($"{_defaultConfigFile}",
                        PortablePath.Combine(FileSystem.Current.LocalStorage.Path,
                            $"{_defaultConfigFile}"));}
            _configurationFiles.Clear();
            AddConfigurationFile(defaultConfigurationFile);
        }

        public void AddConfigurationFile(IConfigurationFile configFile)
        {
            if (_configurationFiles.ContainsKey(configFile.Name))
                throw new ConfigurationException($"There is already a configuration file with the same name '{configFile.Name}'");

            _configurationFiles.Add(configFile.Name, configFile);
        }

        public TValue GetValue<TValue>(string key, string fileName = null)
        {
            fileName = string.IsNullOrWhiteSpace(fileName) ? _defaultConfigFile : fileName;

            if (!string.IsNullOrWhiteSpace(fileName) && !_configurationFiles.ContainsKey(fileName))
                throw new ConfigurationException($"There is no configuration file with the name '{fileName}'");

            return _configurationFiles[fileName].GetValue<TValue>(key);
        }

        public bool TryGetValue<TValue>(string key, out TValue value, string fileName = null)
        {
            fileName = string.IsNullOrWhiteSpace(fileName) ? _defaultConfigFile : fileName;

            if (!string.IsNullOrWhiteSpace(fileName) && !_configurationFiles.ContainsKey(fileName))
                throw new ConfigurationException($"There is no configuration file with the name '{fileName}'");

            return _configurationFiles[fileName].TryGetValue(key, out value);
        }

        public void SetValue<TValue>(string key, TValue value, string fileName = null)
        {
            fileName = string.IsNullOrWhiteSpace(fileName) ? _defaultConfigFile : fileName;

            if (!string.IsNullOrWhiteSpace(fileName) && !_configurationFiles.ContainsKey(fileName))
                throw new ConfigurationException($"There is no configuration file with the name '{fileName}'");

            _configurationFiles[fileName].SetValue(key, value);
        }
    }
}