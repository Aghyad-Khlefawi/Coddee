// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Coddee.WPF.Converters
{
    /// <summary>
    /// A Converter that returns Visibility.Visibliy if the value is true and Visibility.Collapsed if false
    /// giving the ConverterParameter the value "R" will reverse the converter result
    /// </summary>
    [ValueConversion(typeof(bool), typeof(Visibility), ParameterType = typeof(string))]
    public class BoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var val = value as bool?;
            if (val == null)
                throw new ArgumentException("The provided value must be of type Boolean");

            if (!string.IsNullOrEmpty(parameter?.ToString()))
            {
                if (parameter.ToString() == "R")
                    val = !val;
            }

            return val.Value ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var val = value as Visibility?;
            if (val == null)
                throw new ArgumentException("The provided value must be of type Visibility");


            var res = val.Value == Visibility.Visible;
            if (!string.IsNullOrEmpty(parameter?.ToString()))
            {
                if (parameter.ToString() == "R")
                    res = !res;
            }
            return res;
        }
    }
}