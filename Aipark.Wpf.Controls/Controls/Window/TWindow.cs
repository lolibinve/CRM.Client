using System;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using Button = System.Windows.Controls.Button;
using HorizontalAlignment = System.Windows.HorizontalAlignment;

namespace Aipark.Wpf.Controls
{
    [TemplatePart(Name = HeaderContainerName, Type = typeof(FrameworkElement))]
    [TemplatePart(Name = MinimizeButtonName, Type = typeof(Button))]
    [TemplatePart(Name = MaximizeButtonName, Type = typeof(ToggleButton))]
    [TemplatePart(Name = CloseButtonName, Type = typeof(Button))]
    [TemplatePart(Name = LeftThumbName, Type = typeof(Thumb))]
    [TemplatePart(Name = TopThumbName, Type = typeof(Thumb))]
    [TemplatePart(Name = RightThumbName, Type = typeof(Thumb))]
    [TemplatePart(Name = BottomThumbName, Type = typeof(Thumb))]
    [TemplatePart(Name = RightBottomThumbName, Type = typeof(Thumb))]
    [TemplatePart(Name = RightTopThumbName, Type = typeof(Thumb))]
    [TemplatePart(Name = LeftTopThumbName, Type = typeof(Thumb))]
    [TemplatePart(Name = LeftBottomThumbName, Type = typeof(Thumb))]
    public class TWindow : MutexWindow
    {
        private const string HeaderContainerName = "TemplatePart_HeaderContainer";
        private const string MinimizeButtonName = "TemplatePart_MinimizeButton";
        private const string MaximizeButtonName = "TemplatePart_MaximizeButton";
        private const string CloseButtonName = "TemplatePart_CloseButton";

        private const string LeftThumbName = "TemplatePart_LeftThumb";
        private const string TopThumbName = "TemplatePart_TopThumb";
        private const string RightThumbName = "TemplatePart_RightThumb";
        private const string BottomThumbName = "TemplatePart_BottomThumb";

        private const string LeftTopThumbName = "TemplatePart_LeftTopThumb";
        private const string RightTopThumbName = "TemplatePart_RightTopThumb";
        private const string RightBottomThumbName = "TemplatePart_RightBottomThumb";
        private const string LeftBottomThumbName = "TemplatePart_LeftBottomThumb";

        public TWindow()
        {
            base.WindowStyle = WindowStyle.None;
            base.ResizeMode = ResizeMode.NoResize;
        }


        #region 图标

        /// <summary>
        /// 图标宽度
        /// </summary>
        public double IconWidth
        {
            get { return (double)GetValue(IconWidthProperty); }
            set { SetValue(IconWidthProperty, value); }
        }
        public static readonly DependencyProperty IconWidthProperty =
            DependencyProperty.Register(nameof(IconWidth), typeof(double), typeof(TWindow), new FrameworkPropertyMetadata(16.0));

        /// <summary>
        /// 图标高度
        /// </summary>
        public double IconHeight
        {
            get { return (double)GetValue(IconHeightProperty); }
            set { SetValue(IconHeightProperty, value); }
        }
        public static readonly DependencyProperty IconHeightProperty =
            DependencyProperty.Register(nameof(IconHeight), typeof(double), typeof(TWindow), new FrameworkPropertyMetadata(16.0));

        /// <summary>
        /// 图标显示
        /// </summary>
        public Visibility IconVisibility
        {
            get { return (Visibility)GetValue(IconVisibilityProperty); }
            set { SetValue(IconVisibilityProperty, value); }
        }
        public static readonly DependencyProperty IconVisibilityProperty =
            DependencyProperty.Register(nameof(IconVisibility), typeof(Visibility), typeof(TWindow), new FrameworkPropertyMetadata(Visibility.Collapsed));

        /// <summary>
        /// 图标间距
        /// </summary>
        public Thickness IconMargin
        {
            get { return (Thickness)GetValue(IconMarginProperty); }
            set { SetValue(IconMarginProperty, value); }
        }
        public static readonly DependencyProperty IconMarginProperty =
            DependencyProperty.Register(nameof(IconMargin), typeof(Thickness), typeof(TWindow), new PropertyMetadata(new Thickness(0, 0, 0, 0)));

        #endregion


        #region 标题

        /// <summary>
        /// 标题字体
        /// </summary>
        public FontFamily TitleFontFamily
        {
            get { return (FontFamily)GetValue(TitleFontFamilyProperty); }
            set { SetValue(TitleFontFamilyProperty, value); }
        }
        public static readonly DependencyProperty TitleFontFamilyProperty =
            DependencyProperty.Register(nameof(TitleFontFamily), typeof(FontFamily), typeof(TWindow), new FrameworkPropertyMetadata(new FontFamily("Simsun")));

        /// <summary>
        /// 标题字体粗细
        /// </summary>
        public FontWeight TitleFontWeight
        {
            get { return (FontWeight)GetValue(TitleFontWeightProperty); }
            set { SetValue(TitleFontWeightProperty, value); }
        }
        public static readonly DependencyProperty TitleFontWeightProperty =
            DependencyProperty.Register(nameof(TitleFontWeight), typeof(FontWeight), typeof(TWindow), new FrameworkPropertyMetadata(FontWeights.Normal));

        /// <summary>
        /// 标题字体大小
        /// </summary>
        public double TitleFontSize
        {
            get { return (double)GetValue(TitleFontSizeProperty); }
            set { SetValue(TitleFontSizeProperty, value); }
        }
        public static readonly DependencyProperty TitleFontSizeProperty =
            DependencyProperty.Register(nameof(TitleFontSize), typeof(double), typeof(TWindow), new FrameworkPropertyMetadata(14.0));

        /// <summary>
        /// 标题字体颜色
        /// </summary>
        public Brush TitleForeground
        {
            get { return (Brush)GetValue(TitleForegroundProperty); }
            set { SetValue(TitleForegroundProperty, value); }
        }
        public static readonly DependencyProperty TitleForegroundProperty =
            DependencyProperty.Register(nameof(TitleForeground), typeof(Brush), typeof(TWindow), new FrameworkPropertyMetadata(new SolidColorBrush(Colors.White)));

        /// <summary>
        /// 标题水平方向对齐方式
        /// </summary>
        public HorizontalAlignment TitleHorizontalAlignment
        {
            get { return (HorizontalAlignment)GetValue(TitleHorizontalAlignmentProperty); }
            set { SetValue(TitleHorizontalAlignmentProperty, value); }
        }
        public static readonly DependencyProperty TitleHorizontalAlignmentProperty =
            DependencyProperty.Register(nameof(TitleHorizontalAlignment), typeof(HorizontalAlignment), typeof(TWindow), new PropertyMetadata(HorizontalAlignment.Left));

        /// <summary>
        /// 标题可被选中
        /// </summary>
        public bool TitleSelectable
        {
            get { return (bool)GetValue(TitleSelectableProperty); }
            set { SetValue(TitleSelectableProperty, value); }
        }
        public static readonly DependencyProperty TitleSelectableProperty =
            DependencyProperty.Register(nameof(TitleSelectable), typeof(bool), typeof(TWindow), new PropertyMetadata(false));

        /// <summary>
        /// 标题字体距图标的间距
        /// </summary>
        public Thickness TitleMargin
        {
            get { return (Thickness)GetValue(TitleMarginProperty); }
            set { SetValue(TitleMarginProperty, value); }
        }
        public static readonly DependencyProperty TitleMarginProperty =
            DependencyProperty.Register(nameof(TitleMargin), typeof(Thickness), typeof(TWindow), new FrameworkPropertyMetadata(new Thickness(0, 0, 0, 0)));

        #endregion


        #region 标题栏

        /// <summary>
        /// 标题栏背景画刷
        /// </summary>
        public Brush HeaderBackground
        {
            get { return (Brush)GetValue(HeaderBackgroundProperty); }
            set { SetValue(HeaderBackgroundProperty, value); }
        }
        public static readonly DependencyProperty HeaderBackgroundProperty =
            DependencyProperty.Register(nameof(HeaderBackground), typeof(Brush), typeof(TWindow), new FrameworkPropertyMetadata(new SolidColorBrush(Colors.Transparent)));

        /// <summary>
        /// 标题栏高度
        /// </summary>
        public double HeaderHeight
        {
            get { return (double)GetValue(HeaderHeightProperty); }
            set { SetValue(HeaderHeightProperty, value); }
        }
        public static readonly DependencyProperty HeaderHeightProperty =
            DependencyProperty.Register(nameof(HeaderHeight), typeof(double), typeof(TWindow), new FrameworkPropertyMetadata(32.0));

        public FrameworkElement HeaderContent
        {
            get { return (FrameworkElement)GetValue(HeaderContentProperty); }
            set { SetValue(HeaderContentProperty, value); }
        }
        public static readonly DependencyProperty HeaderContentProperty =
        DependencyProperty.Register(nameof(HeaderContent), typeof(FrameworkElement), typeof(TWindow), new FrameworkPropertyMetadata(null, OnHeaderChanged));
        private static void OnHeaderChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            TWindow win = sender as TWindow;
            win.RemoveLogicalChild(e.OldValue);
            win.AddLogicalChild(e.NewValue);
        }

        /// <summary>
        /// 标题栏内间距
        /// </summary>
        public Thickness HeaderPadding
        {
            get { return (Thickness)GetValue(HeaderPaddingProperty); }
            set { SetValue(HeaderPaddingProperty, value); }
        }
        public static readonly DependencyProperty HeaderPaddingProperty =
            DependencyProperty.Register(nameof(HeaderPadding), typeof(Thickness), typeof(TWindow), new FrameworkPropertyMetadata(new Thickness(0, 0, 0, 0)));

        public FrameworkElement AttachedContent
        {
            get { return (FrameworkElement)GetValue(AttachedContentProperty); }
            set { SetValue(AttachedContentProperty, value); }
        }
        public static readonly DependencyProperty AttachedContentProperty =
        DependencyProperty.Register(nameof(AttachedContent), typeof(FrameworkElement), typeof(TWindow), new FrameworkPropertyMetadata(null, OnAttachedContentPropertyChanged));
        private static void OnAttachedContentPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            TWindow win = sender as TWindow;
            win.RemoveLogicalChild(e.OldValue);
            win.AddLogicalChild(e.NewValue);
        }

        #endregion


        #region 窗口按钮

        /// <summary>
        /// 窗口按钮宽度
        /// </summary>
        public double WindowButtonWidth
        {
            get { return (double)GetValue(WindowButtonWidthProperty); }
            set { SetValue(WindowButtonWidthProperty, value); }
        }
        public static readonly DependencyProperty WindowButtonWidthProperty =
            DependencyProperty.Register(nameof(WindowButtonWidth), typeof(double), typeof(TWindow), new FrameworkPropertyMetadata(36.0));

        /// <summary>
        /// 窗口按钮高度
        /// </summary>
        public double WindowButtonHeight
        {
            get { return (double)GetValue(WindowButtonHeightProperty); }
            set { SetValue(WindowButtonHeightProperty, value); }
        }
        public static readonly DependencyProperty WindowButtonHeightProperty =
            DependencyProperty.Register(nameof(WindowButtonHeight), typeof(double), typeof(TWindow), new FrameworkPropertyMetadata(28.0));

        public Brush WindowButtonForeground
        {
            get { return (Brush)GetValue(WindowButtonForegroundProperty); }
            set { SetValue(WindowButtonForegroundProperty, value); }
        }
        public static readonly DependencyProperty WindowButtonForegroundProperty =
            DependencyProperty.Register(nameof(WindowButtonForeground), typeof(Brush), typeof(TWindow), new PropertyMetadata(new SolidColorBrush(Colors.Black)));

        public Brush WindowButtonMouseOverBackground
        {
            get { return (Brush)GetValue(WindowButtonMouseOverBackgroundProperty); }
            set { SetValue(WindowButtonMouseOverBackgroundProperty, value); }
        }
        public static readonly DependencyProperty WindowButtonMouseOverBackgroundProperty =
            DependencyProperty.Register(nameof(WindowButtonMouseOverBackground), typeof(Brush), typeof(TWindow), new PropertyMetadata(new SolidColorBrush(Colors.Transparent)));

        public Brush WindowButtonPressedBackground
        {
            get { return (Brush)GetValue(WindowButtonPressedBackgroundProperty); }
            set { SetValue(WindowButtonPressedBackgroundProperty, value); }
        }
        public static readonly DependencyProperty WindowButtonPressedBackgroundProperty =
            DependencyProperty.Register(nameof(WindowButtonPressedBackground), typeof(Brush), typeof(TWindow), new PropertyMetadata(new SolidColorBrush(Colors.Transparent)));

        #endregion

        /// <summary>
        /// 窗口大小调整模式
        /// </summary>
        public new ResizeMode ResizeMode
        {
            get { return (ResizeMode)GetValue(ResizeModeProperty); }
            set { SetValue(ResizeModeProperty, value); }
        }
        public new static readonly DependencyProperty ResizeModeProperty =
            DependencyProperty.Register("ResizeMode", typeof(ResizeMode), typeof(TWindow), new FrameworkPropertyMetadata(ResizeMode.CanResize));


        static TWindow()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TWindow), new FrameworkPropertyMetadata(typeof(TWindow)));
        }


        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            //标题栏
            FrameworkElement headerContainer = GetTemplateChild<FrameworkElement>(HeaderContainerName);
            headerContainer.MouseLeftButtonDown += HeaderContainerMouseLeftButtonDown;

            //最小化按钮
            Button minimizeButton = GetTemplateChild<Button>(MinimizeButtonName);
            minimizeButton.Click += MinimizeButton_Click;

            //最大化/还原按钮
            ToggleButton maximizeButton = GetTemplateChild<ToggleButton>(MaximizeButtonName);
            maximizeButton.Click += MaximizeButton_Click;

            //关闭按钮
            Button closeButton = GetTemplateChild<Button>(CloseButtonName);
            closeButton.Click += (s, e) => { this.Close(); };

            Thumb leftThumb = GetTemplateChild<Thumb>(LeftThumbName);
            leftThumb.DragDelta += new DragDeltaEventHandler(ResizeLeft);
            Thumb topThumb = GetTemplateChild<Thumb>(TopThumbName);
            topThumb.DragDelta += new DragDeltaEventHandler(ResizeTop);
            Thumb rightThumb = GetTemplateChild<Thumb>(RightThumbName);
            rightThumb.DragDelta += new DragDeltaEventHandler(ResizeRight);
            Thumb bottomThumb = GetTemplateChild<Thumb>(BottomThumbName);
            bottomThumb.DragDelta += new DragDeltaEventHandler(ResizeBottom);

            Thumb leftTopThumb = GetTemplateChild<Thumb>(LeftTopThumbName);
            leftTopThumb.DragDelta += new DragDeltaEventHandler(ResizeLeftTop);
            Thumb rightTopThumb = GetTemplateChild<Thumb>(RightTopThumbName);
            rightTopThumb.DragDelta += new DragDeltaEventHandler(ResizeRightTop);
            Thumb rightBottomThumb = GetTemplateChild<Thumb>(RightBottomThumbName);
            rightBottomThumb.DragDelta += new DragDeltaEventHandler(ResizeRightBottom);
            Thumb leftBottomThumb = GetTemplateChild<Thumb>(LeftBottomThumbName);
            leftBottomThumb.DragDelta += new DragDeltaEventHandler(ResizeLeftBottom);
        }

        private T GetTemplateChild<T>(string childName) where T : FrameworkElement, new()
        {
            return (GetTemplateChild(childName) as T) ?? new T();
        }


        /// <summary>
        /// 窗体处于最大化(不允许拖动)
        /// </summary>
        private bool IsMaximized = false;
        /// <summary>
        /// 窗口尺寸位置
        /// </summary>
        private Rect windowState;

        /// <summary>
        /// 设置窗口最大化
        /// </summary>
        public void SetMaximizeWindowState()
        {
            MaximizeButton_Click(null, null);
        }

        private void HeaderContainerMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2 && (this.ResizeMode == ResizeMode.CanResize || this.ResizeMode == ResizeMode.CanResizeWithGrip))
            {
                MaximizeButton_Click(null, null);
                return;
            }

            if (e.LeftButton == MouseButtonState.Pressed && !IsMaximized)
                base.DragMove();
        }

        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void MaximizeButton_Click(object sender, RoutedEventArgs e)
        {
            ToggleButton btn = GetTemplateChild<ToggleButton>(MaximizeButtonName);
            switch (btn.ToolTip.ToString())
            {
                case "最大化":
                    btn.ToolTip = "向下还原";
                    btn.IsChecked = true;
                    MaximizeWindowSize();
                    break;
                case "向下还原":
                    btn.ToolTip = "最大化";
                    btn.IsChecked = false;
                    RestoreWindowSize();
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// 窗口最大化
        /// </summary>
        private void MaximizeWindowSize()
        {
            windowState = new Rect(this.Left, this.Top, this.Width, this.Height);

            double left = Math.Max(this.Left, 0);

            //计算中点（页面的中心的横坐标）
            var median = (this.Left + (this.Left + this.ActualWidth)) / 2;
            int len = Screen.AllScreens.Length;

            //中点小于第一个屏幕的最小横坐标，则设置为第一个屏幕最大化
            if (median <= Screen.AllScreens[0].Bounds.Left)
            {
                this.Left = Screen.AllScreens[0].Bounds.Left;
            }
            //中点大于最后一个屏幕的最大横坐标，则设置为最后一个屏幕最大化
            else if (median >= Screen.AllScreens[len - 1].Bounds.Right)
            {
                this.Left = Screen.AllScreens[len - 1].Bounds.Left;
            }
            else
            {
                //中点属于哪个屏幕，则在对应屏幕最大化
                foreach (var screen in Screen.AllScreens)
                {
                    if (median >= screen.Bounds.Left && median < screen.Bounds.Right)
                    {
                        this.Left = screen.Bounds.Left;
                    }
                }
            }
            //之前设置的方式
            //this.Left = Math.Floor(left / SystemParameters.WorkArea.Width) * SystemParameters.WorkArea.Width;
            this.Top = 0;
            this.Width = SystemParameters.PrimaryScreenWidth;
            this.Height = left > SystemParameters.WorkArea.Width ? SystemParameters.PrimaryScreenHeight : SystemParameters.WorkArea.Height;

            IsMaximized = true;
        }
        /// <summary>
        /// 还原窗口位置和尺寸
        /// </summary>
        private void RestoreWindowSize()
        {
            this.Width = windowState.Width;
            this.Height = windowState.Height;
            this.Left = windowState.Left;
            this.Top = windowState.Top;
            //
            IsMaximized = false;
        }


        #region 窗口尺寸调整

        private void ResizeLeftTop(object sender, DragDeltaEventArgs e)
        {
            ResizeLeft(sender, e);
            ResizeTop(sender, e);
        }

        private void ResizeRightTop(object sender, DragDeltaEventArgs e)
        {
            ResizeRight(sender, e);
            ResizeTop(sender, e);
        }

        private void ResizeRightBottom(object sender, DragDeltaEventArgs e)
        {
            ResizeBottom(sender, e);
            ResizeRight(sender, e);
        }

        private void ResizeLeftBottom(object sender, DragDeltaEventArgs e)
        {
            ResizeLeft(sender, e);
            ResizeBottom(sender, e);
        }

        private void ResizeLeft(object sender, DragDeltaEventArgs e)
        {
            Width = double.IsNaN(Width) ? ActualWidth : Width;

            double targetWidth = Width - e.HorizontalChange;
            if (e.HorizontalChange < 0)
            {
                targetWidth = double.IsInfinity(MaxWidth) || targetWidth < MaxWidth ? targetWidth : MaxWidth;
                Left -= Math.Abs(targetWidth - Width);
                Width = targetWidth;
            }
            else
            {
                if (double.IsInfinity(MinWidth))
                {
                    targetWidth = targetWidth > 100 ? targetWidth : 100;
                }
                else
                {
                    targetWidth = targetWidth > MinWidth ? targetWidth : MinWidth;
                }

                Left += Math.Abs(targetWidth - Width);
                Width = targetWidth;
            }
        }

        private void ResizeTop(object sender, DragDeltaEventArgs e)
        {
            Height = double.IsNaN(Height) ? ActualHeight : Height;

            double targetHeight = Height - e.VerticalChange;
            if (e.VerticalChange < 0)
            {
                targetHeight = double.IsInfinity(MaxHeight) || targetHeight < MaxHeight ? targetHeight : MaxHeight;
                Top -= Math.Abs(targetHeight - Height);
                Height = targetHeight;
            }
            else
            {
                if (double.IsInfinity(MinHeight))
                {
                    targetHeight = targetHeight > 30 ? targetHeight : 30;
                }
                else
                {
                    targetHeight = targetHeight > MinHeight ? targetHeight : MinHeight;
                }
                Top += Math.Abs(targetHeight - Height);
                Height = targetHeight;
            }
        }

        private void ResizeRight(object sender, DragDeltaEventArgs e)
        {
            Width = double.IsNaN(Width) ? ActualWidth : Width;

            double targetWidth = Width + e.HorizontalChange;
            if (e.HorizontalChange > 0)
            {
                Width = double.IsInfinity(MaxWidth) || targetWidth < MaxWidth ? targetWidth : MaxWidth;
            }
            else
            {
                if (double.IsInfinity(MinWidth))
                {
                    Width = targetWidth > 100 ? targetWidth : 100;
                }
                else
                {
                    Width = targetWidth > MinWidth ? targetWidth : MinWidth;
                }
            }
        }

        private void ResizeBottom(object sender, DragDeltaEventArgs e)
        {
            Height = double.IsNaN(Height) ? ActualHeight : Height;

            double targetHeight = Height + e.VerticalChange;
            if (e.VerticalChange > 0)
            {
                Height = double.IsInfinity(MaxHeight) || targetHeight < MaxHeight ? targetHeight : MaxHeight;
            }
            else
            {
                if (double.IsInfinity(MinHeight))
                {
                    Height = targetHeight > 30 ? targetHeight : 30;
                }
                else
                {
                    Height = targetHeight > MinHeight ? targetHeight : MinHeight;
                }
            }
        }

        #endregion
    }
}
