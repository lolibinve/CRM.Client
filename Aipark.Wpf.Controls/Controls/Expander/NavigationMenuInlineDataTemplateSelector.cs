using System.Windows;
using System.Windows.Controls;

namespace Aipark.Wpf.Controls
{
    public class NavigationMenuInlineDataTemplateSelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            var fe = container as FrameworkElement;
            DataTemplate dt = null;

            if (item is NavigationMenuItem option && option.Children != null)
            {
                dt = fe.FindResource("InlineExpanderTemplate") as DataTemplate;
            }
            else
            {
                dt = fe.FindResource("InlineRadioButtonTemplate") as DataTemplate;
            }
            return dt;
        }
    }
}
