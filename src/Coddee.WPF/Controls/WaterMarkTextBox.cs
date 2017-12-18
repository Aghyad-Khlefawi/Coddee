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

        public TextBox InnerTextBox { get; set; }

        private TextBlock _waterMark;
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            InnerTextBox = (TextBox)GetTemplateChild("PART_TEXTBOX");
            _waterMark = (TextBlock)GetTemplateChild("PART_WATERMARK");
            _waterMark.Cursor = Cursors.IBeam;

            GotFocus += delegate
            { FocusBox(); };
            TextChanged += delegate
            { UpdateWatermarkVisibility(); };
            _waterMark.MouseDown += delegate
            { FocusBox(); };
            _waterMark.MouseLeftButtonDown += delegate
            { FocusBox(); };
            _waterMark.GotFocus += delegate
            { FocusBox(); };
            MouseLeave += delegate
            {
                if (!InnerTextBox.IsFocused)
                    UpdateWatermarkVisibility();
            };
            MouseEnter += delegate
            {
                _waterMark.Visibility = Visibility.Collapsed;
            };
            InnerTextBox.LostFocus += delegate
            {
                UpdateWatermarkVisibility();
            };
            InnerTextBox.GotFocus += delegate
            {
                _waterMark.Visibility = Visibility.Collapsed;
            };
            UpdateWatermarkVisibility();
        }

        protected override void OnTextChanged(TextChangedEventArgs e)
        {
            base.OnTextChanged(e);
            UpdateWatermarkVisibility();
        }

        private void UpdateWatermarkVisibility()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                if (_waterMark == null)
                    return;
                if (!string.IsNullOrWhiteSpace(Text))
                    _waterMark.Visibility = Visibility.Collapsed;
                else
                    _waterMark.Visibility = Visibility.Visible;
            });
        }

        public void FocusBox()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                InnerTextBox.Focus();
                Keyboard.Focus(InnerTextBox);
            });
        }
    }

}
