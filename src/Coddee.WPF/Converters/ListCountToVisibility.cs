// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using System.Collections;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Coddee.WPF.Converters
{
    /// <summary>
    /// A Converter that returns Visibility.Visibliy if the value is an IList and it's items count is higher that the converter parameter and Visibility.Collapsed if less or null
    /// </summary>
    [ValueConversion(typeof(IList), typeof(Visibility), ParameterType = typeof(int))]
    public class ListCountGreaterThanToVisibility : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (parameter == null)
                parameter = "0";

            if (!int.TryParse(parameter.ToString(), out var count))
            {
                throw new ArgumentException("Converter parameter must be a valid integer.");
            }

            if (value == null)
                return Visibility.Collapsed;

            if (!(value is IList list))
                throw new ArgumentException("Converter value must be an IList.");

            return list.Count > count ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }

    /// <summary>
    /// A Converter that returns Visibility.Visibliy if the value is an IList and it's items count is higher that the converter parameter and Visibility.Collapsed if less or null
    /// </summary>
    [ValueConversion(typeof(IList), typeof(Visibility), ParameterType = typeof(int))]
    public class ListCountLowerThanToVisibility : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (parameter == null)
                parameter = "1";

            if (!int.TryParse(parameter.ToString(), out var count))
            {
                throw new ArgumentException("Converter parameter must be a valid integer.");
            }

            if (value == null)
                return Visibility.Collapsed;

            if (!(value is IList list))
                throw new ArgumentException("Converter value must be an IList.");

            return list.Count < count ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}
