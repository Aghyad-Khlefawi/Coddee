// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System.Windows.Markup;
using Coddee.Services;

namespace Coddee.WPF
{
    /// <summary>
    /// Localization markup extension for XAML
    /// </summary>
    public class Localize : UpdatableMarkupExtension
    {
        public static LocalizationManager Localization => LocalizationManager.DefaultLocalizationManager;

        public Localize(object key)
        {
            Key = key;
            Localization.CultureChanged += (sender, args) =>
            {
                UpdateValue(Localization.GetValue((string) Key));
            };
        }

        /// <summary>
        /// The localization key
        /// </summary>
        [ConstructorArgument("key")]
        public object Key { get; set; }

        public override object ProvideValue()
        {
            return Key != null ? Localization.GetValue((string)Key) : null;
        }
    }
}