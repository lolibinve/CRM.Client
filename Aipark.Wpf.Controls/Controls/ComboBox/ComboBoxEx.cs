using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Aipark.Wpf.Controls
{
    public class ComboBoxEx : ComboBox
    {
        static ComboBoxEx()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ComboBoxEx), new FrameworkPropertyMetadata(typeof(ComboBoxEx)));
        }

        /// <summary>
        /// 边框弧度
        /// </summary>
        public CornerRadius CornerRadius
        {
            get { return (CornerRadius)GetValue(CornerRadiusProperty); }
            set { SetValue(CornerRadiusProperty, value); }
        }
        public static readonly DependencyProperty CornerRadiusProperty =
            DependencyProperty.Register(nameof(CornerRadius), typeof(CornerRadius), typeof(ComboBoxEx), new PropertyMetadata(new CornerRadius(0)));

        /// <summary>
        /// 下拉框展开位置
        /// </summary>
        public PlacementMode PopupPlacement
        {
            get { return (PlacementMode)GetValue(PopupPlacementProperty); }
            set { SetValue(PopupPlacementProperty, value); }
        }
        public static readonly DependencyProperty PopupPlacementProperty =
            DependencyProperty.Register(nameof(PopupPlacement), typeof(PlacementMode), typeof(ComboBoxEx), new PropertyMetadata(PlacementMode.Bottom));

        /// <summary>
        /// 下拉框元素内间距
        /// </summary>
        public Thickness PopupItemPadding
        {
            get { return (Thickness)GetValue(PopupItemPaddingProperty); }
            set { SetValue(PopupItemPaddingProperty, value); }
        }
        public static readonly DependencyProperty PopupItemPaddingProperty =
            DependencyProperty.Register(nameof(PopupItemPadding), typeof(Thickness), typeof(ComboBoxEx), new PropertyMetadata(new Thickness(4, 1, 4, 1)));

        /// <summary>
        /// 下拉框最大高度
        /// </summary>
        public double PopupMaxHeight
        {
            get { return (double)GetValue(PopupMaxHeightProperty); }
            set { SetValue(PopupMaxHeightProperty, value); }
        }
        public static readonly DependencyProperty PopupMaxHeightProperty =
            DependencyProperty.Register(nameof(PopupMaxHeight), typeof(double), typeof(ComboBoxEx), new PropertyMetadata(365.0));

        /// <summary>
        /// 下拉按钮间距
        /// </summary>
        public Thickness ToggleButtonPadding
        {
            get { return (Thickness)GetValue(ToggleButtonPaddingProperty); }
            set { SetValue(ToggleButtonPaddingProperty, value); }
        }
        public static readonly DependencyProperty ToggleButtonPaddingProperty =
            DependencyProperty.Register(nameof(ToggleButtonPadding), typeof(Thickness), typeof(ComboBoxEx), new PropertyMetadata(new Thickness(4, 0, 4, 0)));
    }
}
