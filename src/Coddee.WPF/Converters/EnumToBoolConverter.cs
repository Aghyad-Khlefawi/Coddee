// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using System.Globalization;
using System.Windows.Data;

namespace Coddee.WPF.Converters
{

    [ValueConversion(typeof(Enum), typeof(bool), ParameterType = typeof(string))]
    public class EnumToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (parameter is int intParam)
                return ((int)value).Equals(intParam);
            if (parameter is Enum enumParam)
                return value.Equals(enumParam);

            throw new ArgumentException("Converter parameter must be an enum or an int", nameof(parameter));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}
