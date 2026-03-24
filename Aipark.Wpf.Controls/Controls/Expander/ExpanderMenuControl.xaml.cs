using System;
using System.Windows;
using System.Windows.Controls;

namespace Aipark.Wpf.Controls
{
    /// <summary>
    /// ExpanderMenuControl.xaml 的交互逻辑
    /// </summary>
    public partial class ExpanderMenuControl : UserControl
    {
        /// <summary>
        /// 导航按钮点击事件
        /// </summary>
        public event Action<NavigationMenuItem> NavigationMenuItemClick;

        public ExpanderMenuControl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 导航菜单项点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RadioButton_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as RadioButton).Tag is NavigationMenuItem moduleItem)
                NavigationMenuItemClick?.Invoke(moduleItem);
        }
    }
}
