// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Coddee.WPF.Controls
{
    [TemplatePart(Name = "PART_PASSWORDBOX", Type = typeof(PasswordBox))]
    [TemplatePart(Name = "PART_WATERMARK", Type = typeof(TextBlock))]
    public class WaterMarkPasswordBox : TextBox
    {
        static WaterMarkPasswordBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(WaterMarkPasswordBox),
                                                     new FrameworkPropertyMetadata(typeof(WaterMarkPasswordBox)));
        }

        public static readonly DependencyProperty WaterMarkContentProperty = DependencyProperty.Register(
                                                                                                         "WaterMarkContent",
                                                                                                         typeof(string),
                                                                                                         typeof(
                                                                                                             WaterMarkPasswordBox
                                                                                                         ),
                                                                                                         new
                                                                                                             PropertyMetadata(default
                                                                                                                              (
                                                                                                                                  string
                                                                                                                              )))
            ;


        public string WaterMarkContent
        {
            get { return (string) GetValue(WaterMarkContentProperty); }
            set { SetValue(WaterMarkContentProperty, value); }
        }


        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            var passwordBox = (PasswordBox) GetTemplateChild("PART_PASSWORDBOX");
            var waterMark = (TextBlock) GetTemplateChild("PART_WATERMARK");
            waterMark.Cursor = Cursors.IBeam;
            TextChanged += delegate
            {
                if (passwordBox.Password != Text)
                    passwordBox.Password = Text;
            };
            GotFocus += delegate { FocusBox(passwordBox); };
            passwordBox.PasswordChanged += delegate
            {
                SetValue(TextProperty, passwordBox.Password);
                if (!string.IsNullOrEmpty(passwordBox.Password))
                    waterMark.Visibility = Visibility.Collapsed;
            };
            passwordBox.LostFocus += delegate
            {
                if (string.IsNullOrEmpty(passwordBox.Password))
                    waterMark.Visibility = Visibility.Visible;
            };
            passwordBox.GotFocus += delegate { waterMark.Visibility = Visibility.Collapsed; };
            waterMark.MouseDown += delegate { FocusBox(passwordBox); };
            waterMark.GotFocus += delegate { FocusBox(passwordBox); };
        }

        private static void FocusBox(PasswordBox textBox)
        {
            textBox.Focus();
            Keyboard.Focus(textBox);
        }
    }
}