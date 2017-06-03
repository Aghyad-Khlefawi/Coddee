// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using System.Windows.Media;
using Coddee.Validation;

namespace Concord.HIS1.UI.Converters
{
    public class RequiredBrushConverter : IMultiValueConverter
    {
        public static SolidColorBrush ErrorBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#D91D1D"));
        public static SolidColorBrush GrayBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFABADB3"));
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values[0] is RequiredFieldCollection required && parameter is string fieldName)
            {
                var field = required.FirstOrDefault(e => e.FieldName == fieldName);
                if(field!=null)
                return !field.ValidateField(values[1]) ? ErrorBrush: GrayBrush;
            }
            return GrayBrush;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}