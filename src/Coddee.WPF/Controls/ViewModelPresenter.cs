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
                                                        new PropertyMetadata(default(IPresentableViewModel), ViewModelSet));



        public static readonly DependencyProperty ContentProperty = DependencyProperty.Register(
                                                        "Content",
                                                        typeof(UIElement),
                                                        typeof(ViewModelPresenter),
                                                        new PropertyMetadata(default(UIElement)));

        public UIElement Content
        {
            get { return (UIElement)GetValue(ContentProperty); }
            set { SetValue(ContentProperty, value); }
        }

        public IPresentableViewModel ViewModel
        {
            get { return (IPresentableViewModel)GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }

        void UpdateContent()
        {
            Content = ViewModel?.GetView();
        }

        private static void ViewModelSet(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ViewModelPresenter presenter)
            {
                presenter.UpdateContent();
            }
        }
    }
}
