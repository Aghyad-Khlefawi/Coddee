using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace Coddee.WPF.Controls
{
    public enum ViewIndexMode
    {
        UseViewModelIndex,
        Explicit,
    }
    public class ViewModelPresenter : Control
    {
        static ViewModelPresenter()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ViewModelPresenter), new FrameworkPropertyMetadata(typeof(ViewModelPresenter)));
        }

        public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(
                                                        "ViewModel",
                                                        typeof(IPresentableViewModel),
                                                        typeof(ViewModelPresenter),
                                                        new PropertyMetadata(default(IPresentableViewModel), UpdatePresenterContent));

        public static readonly DependencyProperty ViewIndexProperty = DependencyProperty.Register(
                                                        "ViewIndex",
                                                        typeof(int),
                                                        typeof(ViewModelPresenter),
                                                        new PropertyMetadata(0, UpdatePresenterContent));

        public static readonly DependencyPropertyKey ContentPropertyKey = DependencyProperty.RegisterReadOnly(
                                                                      "Content",
                                                                      typeof(UIElement),
                                                                      typeof(ViewModelPresenter),
                                                                      new PropertyMetadata(default(UIElement)));

        public static readonly DependencyProperty ViewIndexModeProperty = DependencyProperty.Register(
                                                        "ViewIndexMode",
                                                        typeof(ViewIndexMode),
                                                        typeof(ViewModelPresenter),
                                                        new PropertyMetadata(default(ViewIndexMode)));

        public ViewIndexMode ViewIndexMode
        {
            get { return (ViewIndexMode)GetValue(ViewIndexModeProperty); }
            set { SetValue(ViewIndexModeProperty, value); }
        }

        public static readonly DependencyProperty ContentProperty = ContentPropertyKey.DependencyProperty;

        public UIElement Content
        {
            get { return (UIElement)GetValue(ContentProperty); }
            protected set { SetValue(ContentPropertyKey, value); }
        }

        public int ViewIndex
        {
            get { return (int)GetValue(ViewIndexProperty); }
            set { SetValue(ViewIndexProperty, value); }
        }

        public IPresentableViewModel ViewModel
        {
            get { return (IPresentableViewModel)GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }

        private static void UpdatePresenterContent(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ViewModelPresenter vmp)
            {
                if (e.NewValue is IPresentableViewModel vm && vmp.ViewIndexMode == ViewIndexMode.UseViewModelIndex)
                {
                    vm.ViewIndexChanged += delegate
                    { vmp.UpdateContent(); };
                }
                vmp.UpdateContent();
            }
        }

        void UpdateContent()
        {
            Debug.WriteLine("Updating ViewModelPresenter content");
            if (ViewModel != null)
            {
                if (ViewIndexMode == ViewIndexMode.UseViewModelIndex)
                {
                    ViewIndex = ViewModel.CurrentViewIndex;
                }
                Content = ViewModel?.GetView(ViewIndex);
            }
        }

    }
}
