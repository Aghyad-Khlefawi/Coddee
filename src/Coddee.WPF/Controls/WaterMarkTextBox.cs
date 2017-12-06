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
        public static readonly DependencyProperty WaterMarkPaddingProperty = DependencyProperty.Register(
                                                        "WaterMarkPadding",
                                                        typeof(Thickness),
                                                        typeof(WaterMarkTextBox),
                                                        new PropertyMetadata(new Thickness(10, 0, 0, 0)));

        public Thickness WaterMarkPadding
        {
            get { return (Thickness)GetValue(WaterMarkPaddingProperty); }
            set { SetValue(WaterMarkPaddingProperty, value); }
        }
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
            { FocusBox(textBox); };
            TextChanged += delegate
            { UpdateWatermarkVisibility(waterMark); };
            waterMark.MouseDown += delegate
            { FocusBox(textBox); };
            waterMark.MouseLeftButtonDown += delegate
            { FocusBox(textBox); };
            waterMark.GotFocus += delegate
            { FocusBox(textBox); };
            MouseLeave += delegate
            {
                if(!textBox.IsFocused)
                UpdateWatermarkVisibility(waterMark);
            };
            MouseEnter += delegate
            {
                waterMark.Visibility = Visibility.Collapsed;
            };
            textBox.LostFocus += delegate
            {
                UpdateWatermarkVisibility(waterMark);
            };
            textBox.GotFocus += delegate
            {
                waterMark.Visibility = Visibility.Collapsed;
            };
        }

        private void UpdateWatermarkVisibility(TextBlock waterMark)
        {
            if (!string.IsNullOrWhiteSpace(Text))
                waterMark.Visibility = Visibility.Collapsed;
            else
                waterMark.Visibility = Visibility.Visible;
        }

        private static void FocusBox(TextBox textBox)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                textBox.Focus();
                Keyboard.Focus(textBox);
            });
        }
    }

}
