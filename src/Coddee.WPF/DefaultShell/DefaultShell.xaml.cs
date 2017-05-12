// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System.Windows;
using System.Windows.Input;

namespace Coddee.WPF.DefaultShell
{
    /// <summary>
    /// The default shell
    /// this Shell will be used if you don't specify a custom shell at the application build
    /// </summary>
    public partial class DefaultShellView : Window, IShell
    {
        public DefaultShellView()
        {
            InitializeComponent();
            StateChanged += DefaultShellView_StateChanged;
        }

        private void DefaultShellView_StateChanged(object sender, System.EventArgs e)
        {
            if (WindowState == WindowState.Maximized)
            {
                //GlowBorder.Visibility = Visibility.Collapsed;
                ToolBar.MouseDown -= DragWindow;
                MaximizeButton.Visibility = Visibility.Collapsed;
                RestoreButton.Visibility = Visibility.Visible;
            }
            else
            {
                //GlowBorder.Visibility = Visibility.Visible;
                ToolBar.MouseDown += DragWindow;
                MaximizeButton.Visibility = Visibility.Visible;
                RestoreButton.Visibility = Visibility.Collapsed;
            }
        }

        public void SetState(WindowState state)
        {
            WindowState = state;
            DefaultShellView_StateChanged(null, null);
        }

        private void DragWindow(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left && e.ButtonState == MouseButtonState.Pressed)
                DragMove();
        }

        private void Restore(object sender, RoutedEventArgs e)
        {
            SetState(WindowState.Normal);
        }

        private void Maximaize(object sender, RoutedEventArgs e)
        {
            SetState(WindowState.Maximized);
        }
    }
}