// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using System.Windows.Data;

namespace Coddee.WPF.Converters
{
    /// <summary>
    /// A Converter that returns Visibility.Visibliy if the value is not null and Visibility.Collapsed if null
    /// giving the ConverterParameter the value "R" will reverse the converter result
    /// </summary>
    [ValueConversion(typeof(object), typeof(bool), ParameterType = typeof(string))]
    public class NullToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var returnValue = !(value is string ?
                string.IsNullOrEmpty((string)value) :
                value == null);

            if (parameter == null || parameter.ToString() != "R")
            {
                return returnValue;
            }
            return !returnValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }
    }

}
