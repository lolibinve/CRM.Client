using System.Windows;
using System.Windows.Controls;

namespace Aipark.Wpf.Controls
{
    public class NavigationMenuShrinkDataTemplateSelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            var fe = container as FrameworkElement;
            DataTemplate dt = null;

            if (item is NavigationMenuItem option && option.Children != null)
            {
                dt = fe.FindResource("ShrinkExpanderTemplate") as DataTemplate;
            }
            else
            {
                dt = fe.FindResource("ShrinkRadioButtonTemplate") as DataTemplate;
            }
            return dt;
        }
    }
}
