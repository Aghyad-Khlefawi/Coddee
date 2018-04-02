using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Coddee.Security;
using Coddee.Services;
using Newtonsoft.Json;
using PCLStorage;
using FileAccess = PCLStorage.FileAccess;

namespace Coddee.Xamarin.Services.Configuration
{
    /// <summary>
    /// A configuration file that contains key value configurations for the application.
    /// </summary>
    public class ConfigurationFile : IConfigurationFile
    {
        /// <param name="name">The configuration file name</param>
        /// <param name="path">The configuration file full path</param>
        public ConfigurationFile(string name, string path)
        {
            Name = name;
            Path = path;
            _configurations = new Dictionary<string, string>();
        }

        /// <param name="name">The configuration file name</param>
        /// <param name="path">The configuration file full path</param>
        /// <param name="cryptoProvider">An encryption provide to encrypt and decrypt the file.</param>
        public ConfigurationFile(string name, string path, CryptoProvider cryptoProvider)
            : this(name, path)
        {
            _cryptoProvider = cryptoProvider;
        }

        /// <param name="name">The configuration file name</param>
        /// <param name="path">The configuration file full path</param>
        /// <param name="cryptoProvider">An encryption provide to encrypt and decrypt the file.</param>
        /// <param name="defaultValues">Default values in case the file did not exist.</param>
        public ConfigurationFile(string name, string path, CryptoProvider cryptoProvider,
            Dictionary<string, object> defaultValues)
            : this(name, path, cryptoProvider)
        {
            _defaultValues = defaultValues;
        }

        /// <inheritdoc />
        public string Name { get; }

        /// <inheritdoc />
        public string Path { get; }

        private readonly CryptoProvider _cryptoProvider;
        private readonly Dictionary<string, object> _defaultValues;
        private Dictionary<string, string> _configurations;

        private IFile _file;

        private bool _isLoaded;

        public TValue GetValue<TValue>(string key)
        {
            if (!_isLoaded)
                ReadFile();
            return JsonConvert.DeserializeObject<TValue>(_configurations[key]);
        }

        public void ReadFile()
        {
            if (!CheckIfFileExistsAndCreate())
            {
                _isLoaded = true;
                return;
            }
            var json = _file.ReadAllTextAsync().GetAwaiter().GetResult();
            
            if (!string.IsNullOrWhiteSpace(json))
            {
                if (_cryptoProvider != null)
                {
                    json = _cryptoProvider.Decryptor(json);
                }
                _configurations = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
            }
            _isLoaded = true;
        }

        public void SetValue<TValue>(string key, TValue value)
        {
            _configurations[key] = JsonConvert.SerializeObject(value);
            _file.WriteAllTextAsync(GetConfigString()).GetAwaiter().GetResult();
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

        private bool CheckIfFileExistsAndCreate()
        {
            if (FileSystem.Current.GetFileFromPathAsync(Path).GetAwaiter().GetResult() != null)
            {
                _file = FileSystem.Current.GetFileFromPathAsync(Path).GetAwaiter().GetResult();
                return true;
            }

            if (_defaultValues == null)
            {
                var folderPath = Path.Remove(Path.IndexOf(Name, StringComparison.InvariantCultureIgnoreCase));
                var folder = FileSystem.Current.LocalStorage;
                if (FileSystem.Current.GetFolderFromPathAsync(folderPath).GetAwaiter().GetResult() == null)
                    folder = folder
                        .CreateFolderAsync(folderPath, CreationCollisionOption.ReplaceExisting).GetAwaiter()
                        .GetResult();
                _file = folder.CreateFileAsync(Name, CreationCollisionOption.OpenIfExists).GetAwaiter().GetResult();
                return false;
            }

            _configurations =
                new Dictionary<string, string>(_defaultValues.ToDictionary(e => e.Key,
                    e => JsonConvert.SerializeObject(e.Value)));
            _file.WriteAllTextAsync(GetConfigString()).GetAwaiter().GetResult();

            return false;
        }

        private string GetConfigString()
        {
            var json = JsonConvert.SerializeObject(_configurations);
            return _cryptoProvider != null ? _cryptoProvider.Encryptor(json) : json;
        }
    }
}