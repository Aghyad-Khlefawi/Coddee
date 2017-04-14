// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System.Windows;
using System.Windows.Input;

namespace Coddee.WPF.Commands
{
    /// <summary>
    /// Attachable command triggered when the Enter key is pressed on the target element
    /// </summary>
    public class OnEnterCommand
    {
        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.RegisterAttached(
                "Command",
                typeof(ICommand),
                typeof(OnEnterCommand),
                new PropertyMetadata(OnSetCommandCallback));

        private static void OnSetCommandCallback(
            DependencyObject dependencyObject,
            DependencyPropertyChangedEventArgs e)
        {
            UIElement el = dependencyObject as UIElement;
            el.KeyDown += (sender, args) =>
            {
                if (args.Key == Key.Enter)
                    ((ICommand) e.NewValue).Execute(null);
            };
        }

        public static void SetCommand(UIElement UIElement, ICommand command)
        {
            UIElement.SetValue(CommandProperty, command);
        }

        public static ICommand GetCommand(UIElement UIElement)
        {
            return (UIElement.GetValue(CommandProperty) as ICommand);
        }
    }
}