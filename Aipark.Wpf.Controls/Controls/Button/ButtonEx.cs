using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Aipark.Wpf.Controls
{
    public class ButtonEx : Button
    {
        /// <summary>
        /// 按钮样式
        /// </summary>
        public ButtonStyle ButtonStyle
        {
            get { return (ButtonStyle)GetValue(ButtonStyleProperty); }
            set { SetValue(ButtonStyleProperty, value); }
        }
        public static readonly DependencyProperty ButtonStyleProperty =
            DependencyProperty.Register(nameof(ButtonStyle), typeof(ButtonStyle), typeof(ButtonEx), new PropertyMetadata(ButtonStyle.Default));

        /// <summary>
        /// 按钮边框弧度
        /// </summary>
        public CornerRadius CornerRadius
        {
            get { return (CornerRadius)GetValue(CornerRadiusProperty); }
            set { SetValue(CornerRadiusProperty, value); }
        }
        public static readonly DependencyProperty CornerRadiusProperty =
            DependencyProperty.Register(nameof(CornerRadius), typeof(CornerRadius), typeof(ButtonEx), new PropertyMetadata(new CornerRadius(0)));

        /// <summary>
        /// 图标数据源(在Icon或IconText模式下生效)
        /// </summary>
        public ImageSource IconSource
        {
            get { return (ImageSource)GetValue(IconSourceProperty); }
            set { SetValue(IconSourceProperty, value); }
        }
        public static readonly DependencyProperty IconSourceProperty =
            DependencyProperty.Register(nameof(IconSource), typeof(ImageSource), typeof(ButtonEx), new PropertyMetadata(null));

        /// <summary>
        /// 鼠标移入前景色
        /// </summary>
        public Brush MouseOverForeground
        {
            get { return (Brush)GetValue(MouseOverForegroundProperty); }
            set { SetValue(MouseOverForegroundProperty, value); }
        }
        public static readonly DependencyProperty MouseOverForegroundProperty =
            DependencyProperty.Register(nameof(MouseOverForeground), typeof(Brush), typeof(ButtonEx), new PropertyMetadata(new SolidColorBrush(Colors.Black)));

        /// <summary>
        /// 鼠标移入背景色
        /// </summary>
        public Brush MouseOverBackground
        {
            get { return (Brush)GetValue(MouseOverBackgroundProperty); }
            set { SetValue(MouseOverBackgroundProperty, value); }
        }
        public static readonly DependencyProperty MouseOverBackgroundProperty =
            DependencyProperty.Register(nameof(MouseOverBackground), typeof(Brush), typeof(ButtonEx), new PropertyMetadata(new SolidColorBrush(Colors.AliceBlue)));

        /// <summary>
        /// 鼠标移入边框颜色
        /// </summary>
        public Brush MouseOverBorderBrush
        {
            get { return (Brush)GetValue(MouseOverBorderBrushProperty); }
            set { SetValue(MouseOverBorderBrushProperty, value); }
        }
        public static readonly DependencyProperty MouseOverBorderBrushProperty =
            DependencyProperty.Register(nameof(MouseOverBorderBrush), typeof(Brush), typeof(ButtonEx), new PropertyMetadata(new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF3C7FB1"))));

        /// <summary>
        /// 鼠标移入内间距(仅Icon模式生效)
        /// </summary>
        public Thickness MouseOverPadding
        {
            get { return (Thickness)GetValue(MouseOverPaddingProperty); }
            set { SetValue(MouseOverPaddingProperty, value); }
        }
        public static readonly DependencyProperty MouseOverPaddingProperty =
            DependencyProperty.Register(nameof(MouseOverPadding), typeof(Thickness), typeof(ButtonEx), new PropertyMetadata(new Thickness(1)));

        /// <summary>
        /// 鼠标按下前景色
        /// </summary>
        public Brush MousePressedForeground
        {
            get { return (Brush)GetValue(MousePressedForegroundProperty); }
            set { SetValue(MousePressedForegroundProperty, value); }
        }
        public static readonly DependencyProperty MousePressedForegroundProperty =
            DependencyProperty.Register(nameof(MousePressedForeground), typeof(Brush), typeof(ButtonEx), new PropertyMetadata(new SolidColorBrush(Colors.Black)));

        /// <summary>
        /// 鼠标按下背景色
        /// </summary>
        public Brush MousePressedBackground
        {
            get { return (Brush)GetValue(MousePressedBackgroundProperty); }
            set { SetValue(MousePressedBackgroundProperty, value); }
        }
        public static readonly DependencyProperty MousePressedBackgroundProperty =
            DependencyProperty.Register(nameof(MousePressedBackground), typeof(Brush), typeof(ButtonEx), new PropertyMetadata(new SolidColorBrush(Colors.LightSkyBlue)));

        /// <summary>
        /// 鼠标按下标框颜色
        /// </summary>
        public Brush MousePressedBorderBrush
        {
            get { return (Brush)GetValue(MousePressedBorderBrushProperty); }
            set { SetValue(MousePressedBorderBrushProperty, value); }
        }
        public static readonly DependencyProperty MousePressedBorderBrushProperty =
            DependencyProperty.Register(nameof(MousePressedBorderBrush), typeof(Brush), typeof(ButtonEx), new PropertyMetadata(new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF2C628B"))));

        /// <summary>
        /// 鼠标按下内间距(仅Icon模式生效)
        /// </summary>
        public Thickness MousePressedPadding
        {
            get { return (Thickness)GetValue(MousePressedPaddingProperty); }
            set { SetValue(MousePressedPaddingProperty, value); }
        }
        public static readonly DependencyProperty MousePressedPaddingProperty =
            DependencyProperty.Register(nameof(MousePressedPadding), typeof(Thickness), typeof(ButtonEx), new PropertyMetadata(new Thickness(2)));

        /// <summary>
        /// 未启用的前景色
        /// </summary>
        public Brush DisabledForeground
        {
            get { return (Brush)GetValue(DisabledForegroundProperty); }
            set { SetValue(DisabledForegroundProperty, value); }
        }
        public static readonly DependencyProperty DisabledForegroundProperty =
            DependencyProperty.Register(nameof(DisabledForeground), typeof(Brush), typeof(ButtonEx), new PropertyMetadata(new SolidColorBrush(Colors.DimGray)));

        /// <summary>
        /// 未启用的背景色
        /// </summary>
        public Brush DisabledBackground
        {
            get { return (Brush)GetValue(DisabledBackgroundProperty); }
            set { SetValue(DisabledBackgroundProperty, value); }
        }
        public static readonly DependencyProperty DisabledBackgroundProperty =
            DependencyProperty.Register(nameof(DisabledBackground), typeof(Brush), typeof(ButtonEx), new PropertyMetadata(new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFF4F4F4"))));

        /// <summary>
        /// 未启用的边框颜色
        /// </summary>
        public Brush DisabledBorderBrush
        {
            get { return (Brush)GetValue(DisabledBorderBrushProperty); }
            set { SetValue(DisabledBorderBrushProperty, value); }
        }
        public static readonly DependencyProperty DisabledBorderBrushProperty =
            DependencyProperty.Register(nameof(DisabledBorderBrush), typeof(Brush), typeof(ButtonEx), new PropertyMetadata(new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFADB2B5"))));

        /// <summary>
        /// 图标宽度(仅图文模式生效)
        /// </summary>
        public double IconWidth
        {
            get { return (double)GetValue(IconWidthProperty); }
            set { SetValue(IconWidthProperty, value); }
        }
        public static readonly DependencyProperty IconWidthProperty =
            DependencyProperty.Register(nameof(IconWidth), typeof(double), typeof(ButtonEx), new PropertyMetadata(16.0));

        /// <summary>
        /// 图标高度(仅图文模式生效)
        /// </summary>
        public double IconHeight
        {
            get { return (double)GetValue(IconHeightProperty); }
            set { SetValue(IconHeightProperty, value); }
        }
        public static readonly DependencyProperty IconHeightProperty =
            DependencyProperty.Register(nameof(IconHeight), typeof(double), typeof(ButtonEx), new PropertyMetadata(16.0));

        /// <summary>
        /// 文本间距(仅图文模式生效)
        /// </summary>
        public Thickness TextMargin
        {
            get { return (Thickness)GetValue(TextMarginProperty); }
            set { SetValue(TextMarginProperty, value); }
        }
        public static readonly DependencyProperty TextMarginProperty =
            DependencyProperty.Register(nameof(TextMargin), typeof(Thickness), typeof(ButtonEx), new PropertyMetadata(new Thickness(6, 0, 0, 0)));

        static ButtonEx()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ButtonEx), new FrameworkPropertyMetadata(typeof(ButtonEx)));
        }
    }

    public enum ButtonStyle
    {
        /// <summary>
        /// 默认样式
        /// </summary>
        Default,

        /// <summary>
        /// 图标按钮
        /// </summary>
        Icon,

        /// <summary>
        /// 文本按钮
        /// </summary>
        Text,

        /// <summary>
        /// 带图标的按钮
        /// </summary>
        IconText
    }
}
