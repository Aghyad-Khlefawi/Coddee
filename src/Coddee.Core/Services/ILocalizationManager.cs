// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using System.Collections.Generic;
using System.Linq.Expressions;


namespace Coddee.Services
{
    public interface ILocalizationManager
    {
        string this[string key] { get; }
        string DefaultCulture { get; set; }

        event EventHandler<string> CultureChanged;

        void AddValues(Dictionary<string, Dictionary<string, string>> values);
        string GetValue(string key, string culture = null);
        string BindValue<T>(T item, Expression<Func<T, object>> property, string key, string culture = null);
        void Initialize(IContainer container);
        void SetCulture(string newCulture);
    }
}