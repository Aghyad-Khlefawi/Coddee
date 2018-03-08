// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Coddee.WPF.Converters
{

    /// <summary>
    /// A XAML converter that checks an <see cref="Enum"/> value and returns <see cref="Visibility.Visible"/> or <see cref="Visibility.Collapsed"/>.
    /// </summary>
    [ValueConversion(typeof(Enum), typeof(Visibility), ParameterType = typeof(string))]
    public class EnumToVisibilityConverter : IValueConverter
    {
        /// <inheritdoc />
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (parameter is int intParam)
                return ((int)value).Equals(intParam) ? Visibility.Visible : Visibility.Collapsed;
            if (parameter is Enum enumParam)
                return value.Equals(enumParam) ? Visibility.Visible : Visibility.Collapsed;

            throw new ArgumentException("Converter parameter must be an enum or an int", nameof(parameter));
        }

        /// <inheritdoc />
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}
