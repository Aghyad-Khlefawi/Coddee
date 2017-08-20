// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using System.Collections.Generic;
using System.Linq.Expressions;


namespace Coddee.Services
{
    /// <summary>
    /// A service responsible for storing the localization values.
    /// </summary>
    public interface ILocalizationManager
    {
        /// <summary>
        /// Returns the value based on the current culture.
        /// </summary>
        string this[string key] { get; }

        /// <summary>
        /// The default culture.
        /// </summary>
        string DefaultCulture { get; set; }

        /// <summary>
        /// Triggered when the LocalizationManager culture is changed.
        /// </summary>
        event EventHandler<string> CultureChanged;

        /// <summary>
        /// Add new values to the dictionary.
        /// </summary>
        void AddValues(Dictionary<string, Dictionary<string, string>> values);

        /// <summary>
        /// Returns the value.
        /// </summary>
        string GetValue(string key, string culture = null);

        /// <summary>
        /// Binds a value to an object property and updated the object value whenever the culture is changed.
        /// </summary>
        /// <typeparam name="T">The type of the object that contains the property.</typeparam>
        /// <param name="item">The object that contains the property.</param>
        /// <param name="property">The property to bind.</param>
        /// <param name="key">The value key.</param>
        string BindValue<T>(T item, Expression<Func<T, object>> property, string key, string culture = null);
        void Initialize(IContainer container);
        
        /// <summary>
        /// Change the current culture.
        /// </summary>
        void SetCulture(string newCulture);
    }
}