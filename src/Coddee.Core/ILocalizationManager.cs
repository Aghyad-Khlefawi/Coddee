using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Microsoft.Practices.Unity;

namespace Coddee
{
    public interface ILocalizationManager
    {
        string DefaultCulture { get; set; }

        event EventHandler<string> CultureChanged;

        void AddValues(Dictionary<string, Dictionary<string, string>> values);
        string GetValue(string key, string culture = null);
        string BindValue<T>(T item, Expression<Func<T, object>> property, string key, string culture = null);
        void Initialize(IUnityContainer container);
        void SetCulture(string newCulture);
    }
}