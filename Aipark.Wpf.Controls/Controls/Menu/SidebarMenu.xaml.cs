using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Aipark.Wpf.Controls
{
    /// <summary>
    /// PrimaryMenu.xaml 的交互逻辑
    /// </summary>
    public partial class SidebarMenu : UserControl
    {
        /// <summary>
        /// 一级导航按钮点击事件
        /// </summary>
        public event Action<NavigationMenuItem> PrimaryMenuItemClick;
        /// <summary>
        /// 二级导航按钮点击事件
        /// </summary>
        public event Action<NavigationMenuItem> SecondaryMenuItemClick;

        public SidebarMenu()
        {
            InitializeComponent();
        }


        /// <summary>
        /// 元素展开方向
        /// </summary>
        public Orientation Orientation
        {
            get { return (Orientation)GetValue(OrientationProperty); }
            set { SetValue(OrientationProperty, value); }
        }
        public static readonly DependencyProperty OrientationProperty =
            DependencyProperty.Register(nameof(Orientation), typeof(Orientation), typeof(SidebarMenu), new PropertyMetadata(Orientation.Horizontal, OnOrientationPropertyChanged));
        public static void OnOrientationPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is SidebarMenu control && e.NewValue is Orientation orientation)
            {
                switch (orientation)
                {
                    case Orientation.Horizontal:
                        control.itemsControl.ItemTemplate = control.FindResource("PrimaryMenuItemDataTemplate") as DataTemplate;
                        break;
                    case Orientation.Vertical:
                        control.itemsControl.ItemTemplate = control.FindResource("SecondaryMenuItemDataTemplate") as DataTemplate;
                        break;
                }
            }
        }

        /// <summary>
        /// 元素边框弧度
        /// </summary>
        public CornerRadius CornerRadius
        {
            get { return (CornerRadius)GetValue(CornerRadiusProperty); }
            set { SetValue(CornerRadiusProperty, value); }
        }
        public static readonly DependencyProperty CornerRadiusProperty =
            DependencyProperty.Register(nameof(CornerRadius), typeof(CornerRadius), typeof(SidebarMenu), new PropertyMetadata(new CornerRadius(3)));

        /// <summary>
        /// 元素内间距
        /// </summary>
        public Thickness InnerPadding
        {
            get { return (Thickness)GetValue(InnerPaddingProperty); }
            set { SetValue(InnerPaddingProperty, value); }
        }
        public static readonly DependencyProperty InnerPaddingProperty =
            DependencyProperty.Register(nameof(InnerPadding), typeof(Thickness), typeof(SidebarMenu), new PropertyMetadata(new Thickness(12, 15, 12, 15)));

        /// <summary>
        /// 元素间距
        /// </summary>
        public Thickness InnerMargin
        {
            get { return (Thickness)GetValue(InnerMarginProperty); }
            set { SetValue(InnerMarginProperty, value); }
        }
        public static readonly DependencyProperty InnerMarginProperty =
            DependencyProperty.Register(nameof(InnerMargin), typeof(Thickness), typeof(SidebarMenu), new PropertyMetadata(new Thickness(3, 0, 3, 0)));

        /// <summary>
        /// 二级菜单的父菜单项
        /// </summary>
        public NavigationMenuItem ParentMenuItem
        {
            get { return (NavigationMenuItem)GetValue(ParentMenuItemProperty); }
            set { SetValue(ParentMenuItemProperty, value); }
        }
        public static readonly DependencyProperty ParentMenuItemProperty =
            DependencyProperty.Register(nameof(ParentMenuItem), typeof(NavigationMenuItem), typeof(SidebarMenu), new PropertyMetadata(null, OnParentMenuItemPropertyChanged));
        public static void OnParentMenuItemPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is SidebarMenu control && e.NewValue is NavigationMenuItem option)
            {
                control.border.Visibility = control.Orientation == Orientation.Horizontal || option == null ? Visibility.Collapsed : Visibility.Visible;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public IEnumerable<NavigationMenuItem> ItemsSource
        {
            get { return (IEnumerable<NavigationMenuItem>)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }
        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register(nameof(ItemsSource), typeof(IEnumerable<NavigationMenuItem>), typeof(SidebarMenu), new PropertyMetadata(null, OnItemsPropertyChanged));
        public static void OnItemsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is SidebarMenu control && e.NewValue is IEnumerable<NavigationMenuItem> itemsSource)
            {
                control.itemsControl.ItemsSource = itemsSource;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public NavigationMenuItem SelectedItem
        {
            get { return (NavigationMenuItem)GetValue(SelectedItemProperty); }
            set { SetValue(SelectedItemProperty, value); }
        }
        public static readonly DependencyProperty SelectedItemProperty =
            DependencyProperty.Register(nameof(SelectedItem), typeof(NavigationMenuItem), typeof(SidebarMenu), new PropertyMetadata(null));


        private void PrimaryMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (sender is RadioButton radioButton && radioButton.DataContext is NavigationMenuItem option)
            {
                this.SelectedItem = option;
                PrimaryMenuItemClick?.Invoke(option);
            }
        }

        private void SecondaryMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (sender is RadioButton radioButton && radioButton.DataContext is NavigationMenuItem option)
            {
                this.SelectedItem = option;
                SecondaryMenuItemClick?.Invoke(option);
            }
        }
    }
}
