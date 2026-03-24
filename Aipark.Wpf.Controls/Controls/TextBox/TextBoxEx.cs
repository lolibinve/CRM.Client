using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Aipark.Wpf.Controls
{
    public class TextBoxEx : TextBox
    {
        /// <summary>
        /// 水印文本
        /// </summary>
        public string Watermark
        {
            get { return (string)GetValue(WatermarkProperty); }
            set { SetValue(WatermarkProperty, value); }
        }
        public static readonly DependencyProperty WatermarkProperty =
            DependencyProperty.Register(nameof(Watermark), typeof(string), typeof(TextBoxEx), new PropertyMetadata(string.Empty));

        /// <summary>
        /// 边框弧度
        /// </summary>
        public CornerRadius CornerRadius
        {
            get { return (CornerRadius)GetValue(CornerRadiusProperty); }
            set { SetValue(CornerRadiusProperty, value); }
        }
        public static readonly DependencyProperty CornerRadiusProperty =
            DependencyProperty.Register("CornerRadius", typeof(CornerRadius), typeof(TextBoxEx), new PropertyMetadata(new CornerRadius(0)));

        /// <summary>
        /// 鼠标移入时边框颜色
        /// </summary>
        public Brush MouseOverBorderBrush
        {
            get { return (Brush)GetValue(MouseOverBorderBrushProperty); }
            set { SetValue(MouseOverBorderBrushProperty, value); }
        }
        public static readonly DependencyProperty MouseOverBorderBrushProperty =
            DependencyProperty.Register("MouseOverBorderBrush", typeof(Brush), typeof(TextBoxEx), new PropertyMetadata(new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF7EB4EA"))));

        static TextBoxEx()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TextBoxEx), new FrameworkPropertyMetadata(typeof(TextBoxEx)));
        }
    }
}
