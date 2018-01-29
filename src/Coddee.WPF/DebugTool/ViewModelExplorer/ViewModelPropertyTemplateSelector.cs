using System.Windows;
using System.Windows.Controls;

namespace Coddee.WPF.DebugTool
{
    public class ViewModelPropertyTemplateSelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if(item!=null)
            return (DataTemplate)((ContentPresenter)container).FindResource((item.GetType().Name+"Template"));
            return base.SelectTemplate(item, container);
        }
    }
}
