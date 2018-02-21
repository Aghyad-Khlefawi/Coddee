// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System.Windows;
using System.Windows.Input;

namespace Coddee.WPF.Commands
{
    /// <summary>
    /// Attachable command triggered when the a key is pressed on the target element
    /// </summary>
    public static class KeyDownCommand
    {
        /// <summary>
        /// The command that will be executed.
        /// </summary>
        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.RegisterAttached(
                "Command",
                typeof(ICommand),
                typeof(KeyDownCommand),
                new PropertyMetadata(OnSetCommandCallback));

        private static void OnSetCommandCallback(
            DependencyObject dependencyObject,
            DependencyPropertyChangedEventArgs e)
        {
            UIElement el = dependencyObject as UIElement;
            el.KeyDown += (sender, args) => { ((RelayCommand<KeyEventArgs>) e.NewValue).Execute(args); };
        }

        /// <summary>
        /// Set <see cref="CommandProperty"/>.
        /// </summary>
        public static void SetCommand(UIElement UIElement, ICommand command)
        {
            UIElement.SetValue(CommandProperty, command);
        }

        /// <summary>
        /// Get <see cref="CommandProperty"/>.
        /// </summary>
        public static ICommand GetCommand(UIElement UIElement)
        {
            return (UIElement.GetValue(CommandProperty) as ICommand);
        }
    }
}