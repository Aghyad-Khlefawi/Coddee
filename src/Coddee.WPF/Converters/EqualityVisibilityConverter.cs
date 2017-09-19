// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Coddee.WPF.Converters
{
    /// <summary>
    /// A Converter that returns Visibility.Visibliy if the value is equal to the parameter and Visibility.Collapsed if not
    /// </summary>
    [ValueConversion(typeof(object), typeof(Visibility), ParameterType = typeof(object))]
    public class EqualityVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value.Equals(parameter) ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}
