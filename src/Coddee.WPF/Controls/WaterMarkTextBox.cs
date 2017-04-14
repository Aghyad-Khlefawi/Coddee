// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Coddee.WPF.Controls
{
    [TemplatePart(Name = "PART_TEXTBOX", Type = typeof(TextBox))]
    [TemplatePart(Name = "PART_WATERMARK", Type = typeof(TextBlock))]
    public class WaterMarkTextBox : TextBox
    {
        static WaterMarkTextBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(WaterMarkTextBox), new FrameworkPropertyMetadata(typeof(WaterMarkTextBox)));
        }

        public static readonly DependencyProperty WaterMarkContentProperty = DependencyProperty.Register(
                                                                                                         "WaterMarkContent", typeof(string), typeof(WaterMarkTextBox), new PropertyMetadata(default(string)));

        public string WaterMarkContent
        {
            get { return (string)GetValue(WaterMarkContentProperty); }
            set { SetValue(WaterMarkContentProperty, value); }
        }


        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            var textBox = (TextBox)GetTemplateChild("PART_TEXTBOX");
            var waterMark = (TextBlock)GetTemplateChild("PART_WATERMARK");
            waterMark.Cursor = Cursors.IBeam;
            GotFocus += delegate
            {
                FocusBox(textBox);
            };
            textBox.TextChanged += delegate
            {
                if (!string.IsNullOrEmpty(textBox.Text))
                    waterMark.Visibility = Visibility.Collapsed;
            };
            textBox.LostFocus += delegate
            {
                if (string.IsNullOrEmpty(textBox.Text))
                    waterMark.Visibility = Visibility.Visible;
            };
            textBox.GotFocus += delegate
            {
                waterMark.Visibility = Visibility.Collapsed;
            };
            waterMark.MouseDown += delegate
            {
                FocusBox(textBox);
            };
            waterMark.GotFocus += delegate
            {
                FocusBox(textBox);
            };
            
        }

        private static void FocusBox(TextBox textBox)
        {
            textBox.Focus();
            Keyboard.Focus(textBox);
        }
    }

}
