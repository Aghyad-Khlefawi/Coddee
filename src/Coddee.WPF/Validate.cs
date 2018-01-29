// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System.Windows;

namespace Coddee.WPF
    {
    /// <summary>
    /// A XAML helper for the validation of input elements
    /// </summary>
    public class Validate
    {
        public static readonly DependencyProperty FieldNameProperty = DependencyProperty.RegisterAttached(
                                                                "FieldName",
                                                                typeof(string),
                                                                typeof(Validate),
                                                                new PropertyMetadata(default(string)));
        public static void SetFieldName(DependencyObject element, string value)
        {
            element.SetValue(FieldNameProperty, value);
        }

        public static string GetFieldName(DependencyObject element)
        {
            return (string) element.GetValue(FieldNameProperty);
        }
    }
}
