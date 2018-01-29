// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System.Text.RegularExpressions;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Interactivity;

namespace Coddee.WPF
{
    public class InputAlphabetMaskBehavior : Behavior<TextBoxBase>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.PreviewTextInput += AssociatedObject_PreviewTextInput;
        }

        private void AssociatedObject_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !Regex.IsMatch(e.Text, @"^[A-Z a-z ء-ي]+$");
        }
    }
}
