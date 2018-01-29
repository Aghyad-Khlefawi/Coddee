using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Coddee.Services.ApplicationSearch
{
    /// <summary>
    /// Interaction logic for SearchViewView.xaml
    /// </summary>
    public partial class SearchView : UserControl
    {
        public SearchView()
        {
            InitializeComponent();
        }
        private void EventSetter_OnHandler(object sender, RoutedEventArgs e)
        {
            Keyboard.Focus((ListBoxItem)sender);
        }
    }
}
