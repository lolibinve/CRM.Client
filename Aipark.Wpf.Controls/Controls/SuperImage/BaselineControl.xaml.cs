using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Aipark.Wpf.Controls.SuperImage
{
    /// <summary>
    /// BaselineControl.xaml 的交互逻辑
    /// </summary>
    public partial class BaselineControl : UserControl
    {
        public BaselineControl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 基准线颜色
        /// </summary>
        public Brush BaselineStroke
        {
            get { return (Brush)GetValue(BaselineStrokeProperty); }
            set { SetValue(BaselineStrokeProperty, value); }
        }
        public static readonly DependencyProperty BaselineStrokeProperty =
            DependencyProperty.Register(nameof(BaselineStroke), typeof(Brush), typeof(BaselineControl), new PropertyMetadata(new SolidColorBrush(Colors.Gold), OnBaselineStrokePropertyChanged));
        public static void OnBaselineStrokePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is BaselineControl control)
            {
                control.yRectangle.Stroke = control.BaselineStroke;
                control.xRectangle.Stroke = control.BaselineStroke;
            }
        }

        /// <summary>
        /// 水平偏移量
        /// </summary>
        public double OffsetX
        {
            get { return (double)GetValue(OffsetXProperty); }
            set { SetValue(OffsetXProperty, value); }
        }
        public static readonly DependencyProperty OffsetXProperty =
            DependencyProperty.Register(nameof(OffsetX), typeof(double), typeof(BaselineControl), new PropertyMetadata(0.0, OnOffsetXPropertyChanged));
        public static void OnOffsetXPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is BaselineControl control)
            {
                control.xRectangle.Margin = new Thickness(control.OffsetX, 0, 0, 0);
            }
        }

        /// <summary>
        /// 竖直偏移量
        /// </summary>
        public double OffsetY
        {
            get { return (double)GetValue(OffsetYProperty); }
            set { SetValue(OffsetYProperty, value); }
        }
        public static readonly DependencyProperty OffsetYProperty =
            DependencyProperty.Register(nameof(OffsetY), typeof(double), typeof(BaselineControl), new PropertyMetadata(0.0, OnOffsetYPropertyChanged));
        public static void OnOffsetYPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is BaselineControl control)
            {
                control.yRectangle.Margin = new Thickness(0, control.OffsetY, 0, 0);
            }
        }

    }
}
