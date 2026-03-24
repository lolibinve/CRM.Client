using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace Aipark.Wpf.Controls
{
    /// <summary>
    /// NavigationMenu.xaml 的交互逻辑
    /// </summary>
    public partial class NavigationMenu : UserControl
    {
        /// <summary>
        /// 导航按钮点击事件
        /// </summary>
        public event Action<NavigationMenuItem> NavigationMenuItemClick;
        /// <summary>
        /// 样式改变事件
        /// </summary>
        public event Action<NavigationMenu> StyleChanged;

        public NavigationMenu()
        {
            InitializeComponent();
        }


        /// <summary>
        /// 内联菜单样式
        /// </summary>
        public bool InlineMode
        {
            get { return (bool)GetValue(InlineModeProperty); }
            set { SetValue(InlineModeProperty, value); }
        }
        public static readonly DependencyProperty InlineModeProperty =
            DependencyProperty.Register(nameof(InlineMode), typeof(bool), typeof(NavigationMenu), new PropertyMetadata(false, OnIsInlineStylePropertyChanged));
        public static void OnIsInlineStylePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is NavigationMenu control && e.NewValue is bool inline)
            {
                if (inline)
                {
                    control.InlineItemsControl.Visibility = Visibility.Visible;
                    control.ShrinkItemsControl.Visibility = Visibility.Collapsed;
                    control.Column1.Width = new GridLength(208);
                    control.IsOpen = false;
                }
                else
                {
                    control.InlineItemsControl.Visibility = Visibility.Collapsed;
                    control.ShrinkItemsControl.Visibility = Visibility.Visible;
                    control.Column1.Width = new GridLength(48);
                }

                control.StyleChanged?.Invoke(control);
            }
        }

        /// <summary>
        /// 面板展开
        /// </summary>
        public bool IsOpen
        {
            get { return (bool)GetValue(IsOpenProperty); }
            set { SetValue(IsOpenProperty, value); }
        }
        public static readonly DependencyProperty IsOpenProperty =
            DependencyProperty.Register(nameof(IsOpen), typeof(bool), typeof(NavigationMenu), new PropertyMetadata(false, OnIsOpenPropertyChanged));
        public static void OnIsOpenPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is NavigationMenu control && e.NewValue is bool isOpen)
            {
                if (isOpen)
                {
                    control.Column2.Width = new GridLength(160);
                    control.ShrinkChildrenPanel.Visibility = Visibility.Visible;
                }
                else
                {
                    control.Column2.Width = new GridLength(0);
                    control.ShrinkChildrenPanel.Visibility = Visibility.Collapsed;
                }

                control.StyleChanged?.Invoke(control);
            }
        }

        /// <summary>
        /// 二级面板的数据上下文
        /// </summary>
        public NavigationMenuItem ShrinkChildrenPanelDataContext
        {
            get { return (NavigationMenuItem)GetValue(ShrinkChildrenPanelDataContextProperty); }
            set { SetValue(ShrinkChildrenPanelDataContextProperty, value); }
        }
        public static readonly DependencyProperty ShrinkChildrenPanelDataContextProperty =
            DependencyProperty.Register(nameof(ShrinkChildrenPanelDataContext), typeof(NavigationMenuItem), typeof(NavigationMenu), new PropertyMetadata(null));

        /// <summary>
        /// 数据源
        /// </summary>
        public ObservableCollection<NavigationMenuItem> ItemsSource
        {
            get { return (ObservableCollection<NavigationMenuItem>)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }
        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register(nameof(ItemsSource), typeof(ObservableCollection<NavigationMenuItem>), typeof(NavigationMenu), new PropertyMetadata(null, OnItemsSourcePropertyChanged));
        public static void OnItemsSourcePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is NavigationMenu control && e.NewValue is ObservableCollection<NavigationMenuItem> items)
            {
                control.ShrinkItemsControl.ItemsSource = items;
            }
        }

        private void ShrinkMenuButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is NavigationMenuItem option)
            {
                if (IsOpen && option == ShrinkChildrenPanelDataContext)
                {
                    IsOpen = false;
                }
                else
                {
                    ShrinkChildrenPanelDataContext = option;
                    IsOpen = true;
                }
            }
        }

        /// <summary>
        /// 菜单项点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItemRadioButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is RadioButton radioButton && radioButton.DataContext is NavigationMenuItem option)
            {
                NavigationMenuItemClick?.Invoke(option);
            }
        }
    }


}
