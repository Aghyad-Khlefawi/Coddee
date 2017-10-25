using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace Coddee.WPF.Controls
{
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
                                                        new PropertyMetadata(default(int), UpdatePresenterContent));

        public static readonly DependencyPropertyKey ContentPropertyKey = DependencyProperty.RegisterReadOnly(
                                                                      "Content",
                                                                      typeof(UIElement),
                                                                      typeof(ViewModelPresenter),
                                                                      new PropertyMetadata(default(UIElement)));

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
                vmp.UpdateContent();
        }

        void UpdateContent()
        {
            Debug.WriteLine("Updating ViewModelPresenter content");
            Content = ViewModel?.GetView(ViewIndex);
        }

    }
}
