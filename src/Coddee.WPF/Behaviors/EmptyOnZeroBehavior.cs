using System.Windows.Controls;
using System.Windows.Interactivity;

namespace Coddee.WPF
{
    public sealed class EmptyOnZeroBehavior : Behavior<TextBox>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.GotFocus += AssociatedObject_GotFocus;
            AssociatedObject.LostFocus += AssociatedObject_LostFocus;
        }

   

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.GotFocus -= AssociatedObject_GotFocus;
            AssociatedObject.LostFocus -= AssociatedObject_LostFocus;
        }

        private void AssociatedObject_GotFocus(object sender, System.Windows.RoutedEventArgs e)
        {
            var tb = sender as TextBox;
            if (!string.IsNullOrEmpty(tb?.Text) && tb.Text == "0")
                tb.Text = "";
        }
        private void AssociatedObject_LostFocus(object sender, System.Windows.RoutedEventArgs e)
        {
            var tb = sender as TextBox;
            if (tb!=null && string.IsNullOrEmpty(tb.Text))
                tb.Text = "0";
        }
    }
}
