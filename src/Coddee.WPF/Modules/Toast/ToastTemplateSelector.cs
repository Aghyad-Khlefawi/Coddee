// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System.Windows;
using System.Windows.Controls;

namespace Coddee.Services.Toast
{
    /// <summary>
    /// A template selector for the toast service
    /// </summary>
    public class ToastTemplateSelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            var toast = item as Toast;
            if (toast != null)
            {
                switch (toast.Type)
                {
                    case ToastType.Information:
                        return (DataTemplate) Application.Current.FindResource("ToastInformationTemplate");
                    case ToastType.Success:
                        return (DataTemplate) Application.Current.FindResource("ToastSuccessTemplate");
                    case ToastType.Warning:
                        return (DataTemplate) Application.Current.FindResource("ToastWarningTemplate");
                    case ToastType.Error:
                        return (DataTemplate) Application.Current.FindResource("ToastErrorTemplate");
                }
            }
            return null;
        }
    }
}