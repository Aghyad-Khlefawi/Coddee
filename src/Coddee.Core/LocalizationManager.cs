using System;
using System.Collections.Generic;
using Coddee.Loggers;
using Microsoft.Practices.Unity;

namespace Coddee
{
   
    public class LocalizationManager : ILocalizationManager
    {
        private static LocalizationManager _defaultLocalizationManager;

        public static void SetDefaultLocalizationManager(LocalizationManager manager)
        {
            _defaultLocalizationManager = manager;
        }

        public static LocalizationManager DefaultLocalizationManager => _defaultLocalizationManager ??
                                                                        (_defaultLocalizationManager =
                                                                            new LocalizationManager());

        public LocalizationManager()
        {
            _localziationValues = new Dictionary<string, Dictionary<string, string>>();
        }

        private ILogger _logger;
        public event EventHandler<string> CultureChanged;
        public string DefaultCulture { get; set; }

        private readonly Dictionary<string, Dictionary<string, string>> _localziationValues;


        public void SetCulture(string newCulture)
        {
            DefaultCulture = newCulture;
            CultureChanged?.Invoke(this, newCulture);
        }

        public void Initialize(IUnityContainer container)
        {
            if (container.IsRegistered<ILogger>())
                _logger = container.Resolve<ILogger>();
        }

        public void AddValues(Dictionary<string, Dictionary<string, string>> values)
        {
            foreach (var value in values)
            {
                if (!_localziationValues.ContainsKey(value.Key))
                {
                    _localziationValues[value.Key] = new Dictionary<string, string>();
                }
                var existedDictionary = _localziationValues[value.Key];
                foreach (var localizationPair in value.Value)
                {
                    existedDictionary[localizationPair.Key] = localizationPair.Value;
                }
            }
        }

        public string GetValue(string key, string culture = null)
        {
            if (string.IsNullOrEmpty(culture))
                culture = DefaultCulture;

            if (!_localziationValues.ContainsKey(key) || !_localziationValues[key].ContainsKey(culture))
            {
                _logger?.Log(nameof(LocalizationManager), $"Localization values not found for '{key}'",LogRecordTypes.Debug);
                return key;
            }

            return _localziationValues[key][culture];
        }
    }
}