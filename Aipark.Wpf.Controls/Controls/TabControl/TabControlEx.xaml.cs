using System.Windows;
using System.Windows.Controls;
using Aipark.Wpf.Controls.Extensions;

namespace Aipark.Wpf.Controls
{
    /// <summary>
    /// TabControlEx.xaml 的交互逻辑
    /// </summary>
    public partial class TabControlEx : TabControl
    {
        public TabControlEx()
        {
            InitializeComponent();
        }


        /// <summary>
        /// 标题栏卡牌间距
        /// </summary>
        public Thickness TabHeaderMargin
        {
            get { return (Thickness)GetValue(TabHeaderMarginProperty); }
            set { SetValue(TabHeaderMarginProperty, value); }
        }
        public static readonly DependencyProperty TabHeaderMarginProperty =
            DependencyProperty.Register(nameof(TabHeaderMargin), typeof(Thickness), typeof(TabControlEx), new PropertyMetadata(new Thickness(4, 0, 0, 0)));

        /// <summary>
        /// 标题栏弧度
        /// </summary>
        public CornerRadius TabHeaderCornerRadius
        {
            get { return (CornerRadius)GetValue(TabHeaderCornerRadiusProperty); }
            set { SetValue(TabHeaderCornerRadiusProperty, value); }
        }
        public static readonly DependencyProperty TabHeaderCornerRadiusProperty =
            DependencyProperty.Register(nameof(TabHeaderCornerRadius), typeof(CornerRadius), typeof(TabControlEx), new PropertyMetadata(new CornerRadius(3, 3, 0, 0)));

        /// <summary>
        /// 显示关闭全部选项卡的按钮
        /// </summary>
        public bool ShowCloseAllButton
        {
            get { return (bool)GetValue(ShowCloseAllButtonProperty); }
            set { SetValue(ShowCloseAllButtonProperty, value); }
        }
        public static readonly DependencyProperty ShowCloseAllButtonProperty =
            DependencyProperty.Register(nameof(ShowCloseAllButton), typeof(bool), typeof(TabControlEx), new PropertyMetadata(true));


        private void TabItemCloseButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button)
            {
                var tabItem = button.GetFirstParent<TabItem>();
                if (tabItem != null)
                {
                    this.Items.Remove(tabItem);
                }
            }
        }

        private void TabItemCloseAllButton_Click(object sender, RoutedEventArgs e)
        {
            this.Items.Clear();
        }
    }
}
