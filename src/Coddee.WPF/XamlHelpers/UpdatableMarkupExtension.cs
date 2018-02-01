// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using System.Reflection;
using System.Windows;
using System.Windows.Markup;

namespace Coddee.WPF
{
    /// <summary>
    /// A XAML Markup extension that can update its value
    /// </summary>
    public abstract class UpdatableMarkupExtension : MarkupExtension
    {
        private object _targetObject;
        private object _targetProperty;

        protected object TargetObject => _targetObject;

        protected object TargetProperty => _targetProperty;

        public sealed override object ProvideValue(IServiceProvider serviceProvider)
        {
            IProvideValueTarget target = serviceProvider.GetService(typeof(IProvideValueTarget)) as IProvideValueTarget;
            if (target != null)
            {
                _targetObject = target.TargetObject;
                _targetProperty = target.TargetProperty;
            }

            return ProvideValue();
        }

        protected void UpdateValue(object value)
        {
            if (_targetObject != null)
            {
                if (_targetProperty is DependencyProperty)
                {
                    DependencyObject obj = _targetObject as DependencyObject;
                    DependencyProperty prop = _targetProperty as DependencyProperty;

                    Action updateAction = () => obj.SetValue(prop, value);
                    if (obj != null)
                        if (obj.CheckAccess())
                            updateAction();
                        else
                            obj.Dispatcher.Invoke(updateAction);
                }
                else
                {
                    PropertyInfo prop = _targetProperty as PropertyInfo;
                    prop.SetValue(_targetObject, value, null);
                }
            }
        }

        public abstract object ProvideValue();
    }
}
