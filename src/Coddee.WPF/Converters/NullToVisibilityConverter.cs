// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using System.Windows;
using System.Windows.Data;

namespace Coddee.WPF.Converters
{
    /// <summary>
    /// A Converter that returns Visibility.Visibliy if the value is not null and Visibility.Collapsed if null
    /// giving the ConverterParameter the value "R" will reverse the converter result
    /// </summary>
    [ValueConversion(typeof(object), typeof(Visibility), ParameterType = typeof(string))]
    public class NullToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var returnValue = value is string ?
                (string.IsNullOrEmpty((string)value) ? Visibility.Collapsed : Visibility.Visible) :
                (value == null ? Visibility.Collapsed : Visibility.Visible);

            if (parameter == null || parameter.ToString() != "R")
            {
                return returnValue;
            }
            return returnValue == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value;
        }
    }

}
