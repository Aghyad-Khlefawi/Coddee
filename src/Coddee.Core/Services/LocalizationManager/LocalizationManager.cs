using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using Coddee.Loggers;
using Microsoft.Practices.Unity;

namespace Coddee
{
    class BoundLocalizationObject
    {
        public object Item { get; set; }
        public PropertyInfo PropertyInfo { get; set; }

        public void SetValue(object value)
        {
            PropertyInfo.SetValue(Item,value);
        }
    }


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
            _boundObjects = new ConcurrentDictionary<string, List<BoundLocalizationObject>>();
        }

        private ILogger _logger;
        public event EventHandler<string> CultureChanged;
        public string this[string key]
        {
            get { return GetValue(key); }
        }
        public string DefaultCulture { get; set; }

        private readonly Dictionary<string, Dictionary<string, string>> _localziationValues;
        private readonly ConcurrentDictionary<string, List<BoundLocalizationObject>> _boundObjects;


        public void SetCulture(string newCulture)
        {
            DefaultCulture = newCulture;
            UpdateBoundObjects();
            CultureChanged?.Invoke(this, newCulture);
        }

        private void UpdateBoundObjects()
        {
            foreach (var boundObject in _boundObjects)
            {
                var newValue = GetValue(boundObject.Key);
                foreach (var boundLocalizationObject in boundObject.Value)
                {
                    boundLocalizationObject.SetValue(newValue);
                }
            }
        }

        public string BindValue<T>(T item, Expression<Func<T, object>> property, string key, string culture = null)
        {
            var currentValue = GetValue(key, culture);
            var propertyName = ((MemberExpression)property.Body).Member.Name;
            var boundValue = new BoundLocalizationObject
            {
                Item = item,
                PropertyInfo = item.GetType().GetTypeInfo().GetProperty(propertyName)
            };

            if (!_boundObjects.ContainsKey(key))
                _boundObjects.TryAdd(key, new List<BoundLocalizationObject>());
            List<BoundLocalizationObject> objects;
            if (_boundObjects.TryGetValue(key, out objects))
                objects.Add(boundValue);
            boundValue.SetValue(currentValue);
            return currentValue;
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
                _logger?.Log(nameof(LocalizationManager),
                             $"Localization values not found for '{key}'",
                             LogRecordTypes.Debug);
                return key;
            }

            return _localziationValues[key][culture];
        }
    }
}