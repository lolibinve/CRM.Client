using CLog;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Aipark.Wpf.Controls.SuperImage
{
    /// <summary>
    /// ImageEx.xaml 的交互逻辑
    /// </summary>
    public partial class ImageEx : UserControl
    {
        private const string _defaultUriSource = "!@#$%^&*";

        /// <summary>
        /// 区域选取完成
        /// </summary>
        public event Action<ImageEx, SelectionEventArgs> SelectionCompleted;
        /// <summary>
        /// 图像地址解码完成
        /// </summary>
        public event Action<ImageEx> DecodeCompleted;
        /// <summary>
        /// 绘图画面尺寸变化事件
        /// </summary>
        public event Action<object, SizeChangedEventArgs> CanvasSizeChanged;

        public ImageEx()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 绘图画板
        /// </summary>
        public Canvas Canvas => this.drawCanvas;

        /// <summary>
        /// 图像原始分辨率宽度
        /// </summary>
        public int OriginalPixelWidth
        {
            get
            {
                return this._bitmapImage == null ? 0 : this._bitmapImage.PixelWidth;
            }
        }

        /// <summary>
        /// 图像原始分辨率高度
        /// </summary>
        public int OriginalPixelHeight
        {
            get
            {
                return this._bitmapImage == null ? 0 : this._bitmapImage.PixelHeight;
            }
        }

        /// <summary>
        /// 图像原始流
        /// </summary>
        public Stream OriginalStream
        {
            get
            {
                return this._bitmapImage == null ? null : this._bitmapImage.StreamSource;
            }
        }

        /// <summary>
        /// 底图
        /// </summary>
        private BitmapImage _bitmapImage;

        /// <summary>
        /// 缓存网络图像到本地,在后续的操作中使用本地图像
        /// </summary>
        public bool LocalCache
        {
            get { return (bool)GetValue(LocalCacheProperty); }
            set { SetValue(LocalCacheProperty, value); }
        }
        public static readonly DependencyProperty LocalCacheProperty =
            DependencyProperty.Register(nameof(LocalCache), typeof(bool), typeof(ImageEx), new PropertyMetadata(true));

        /// <summary>
        /// 取消令牌
        /// </summary>
        public CancellationTokenSource Cts
        {
            get { return (CancellationTokenSource)GetValue(CtsProperty); }
            private set { SetValue(CtsProperty, value); }
        }
        public static readonly DependencyProperty CtsProperty =
            DependencyProperty.Register(nameof(Cts), typeof(CancellationTokenSource), typeof(ImageEx), new PropertyMetadata(null));

        /// <summary>
        /// 图像地址
        /// </summary>
        public string UriSource
        {
            get { return (string)GetValue(UriSourceProperty); }
            set { SetValue(UriSourceProperty, value); }
        }
        public static readonly DependencyProperty UriSourceProperty =
            DependencyProperty.Register(nameof(UriSource), typeof(string), typeof(ImageEx), new PropertyMetadata(_defaultUriSource, OnUriPropertyChanged));
        public static async void OnUriPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ImageEx control)
            {
                control.Cts?.Cancel();

                control.SetTransformImageSource(null);

                await control.RefreshImageWithUriPropertyAsync();
            }
        }

        /// <summary>
        /// 图像地址
        /// </summary>
        public bool NarrowDecode
        {
            get { return (bool)GetValue(NarrowDecodeProperty); }
            set { SetValue(NarrowDecodeProperty, value); }
        }
        public static readonly DependencyProperty NarrowDecodeProperty =
            DependencyProperty.Register(nameof(NarrowDecode), typeof(bool), typeof(ImageEx), new PropertyMetadata(false));


        /// <summary>
        /// 图像缩放模式
        /// </summary>
        public ImageZoomMode ZoomMode
        {
            get { return (ImageZoomMode)GetValue(ZoomModeProperty); }
            set { SetValue(ZoomModeProperty, value); }
        }
        public static readonly DependencyProperty ZoomModeProperty =
            DependencyProperty.Register(nameof(ZoomMode), typeof(ImageZoomMode), typeof(ImageEx), new PropertyMetadata(ImageZoomMode.None, OnZoomModePropertyChanged));
        public static void OnZoomModePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ImageEx control)
            {
                control.RefreshImageWithStretchProperty();
            }
        }

        /// <summary>
        /// 图像填充方式
        /// </summary>
        public Stretch Stretch
        {
            get { return (Stretch)GetValue(StretchProperty); }
            set { SetValue(StretchProperty, value); }
        }
        public static readonly DependencyProperty StretchProperty =
            DependencyProperty.Register(nameof(Stretch), typeof(Stretch), typeof(ImageEx), new PropertyMetadata(Stretch.Uniform, OnStretchPropertyChanged));
        public static void OnStretchPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ImageEx control)
            {
                control.RefreshImageWithStretchProperty();
            }
        }

        /// <summary>
        /// 区域选择交互模式
        /// </summary>
        public SelectionMode SelectionMode
        {
            get { return (SelectionMode)GetValue(SelectionModeProperty); }
            set { SetValue(SelectionModeProperty, value); }
        }
        public static readonly DependencyProperty SelectionModeProperty =
            DependencyProperty.Register(nameof(SelectionMode), typeof(SelectionMode), typeof(ImageEx), new PropertyMetadata(SelectionMode.Disable, OnSelectionModePropertyChanged));
        public static void OnSelectionModePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ImageEx control)
            {
                if (control.SelectionMode == SelectionMode.Disable)
                {
                    control.SelectionSwitch = SelectionSwitch.OFF;
                }
            }
        }

        /// <summary>
        /// 选择区域形状
        /// </summary>
        public SelectionType SelectionType
        {
            get { return (SelectionType)GetValue(SelectionTypeProperty); }
            set { SetValue(SelectionTypeProperty, value); }
        }
        public static readonly DependencyProperty SelectionTypeProperty =
            DependencyProperty.Register(nameof(SelectionType), typeof(SelectionType), typeof(ImageEx), new PropertyMetadata(SelectionType.Point));

        /// <summary>
        /// 区域选择使能状态切换方式
        /// </summary>
        public SelectionSwitchChangeMode SelectionSwitchChangeMode
        {
            get { return (SelectionSwitchChangeMode)GetValue(SelectionSwitchChangeModeProperty); }
            set { SetValue(SelectionSwitchChangeModeProperty, value); }
        }
        public static readonly DependencyProperty SelectionSwitchChangeModeProperty =
            DependencyProperty.Register(nameof(SelectionSwitchChangeMode), typeof(SelectionSwitchChangeMode), typeof(ImageEx), new PropertyMetadata(SelectionSwitchChangeMode.Default));

        /// <summary>
        /// 区域选择使能状态
        /// </summary>
        public SelectionSwitch SelectionSwitch
        {
            get { return (SelectionSwitch)GetValue(SelectionSwitchProperty); }
            set { SetValue(SelectionSwitchProperty, value); }
        }
        public static readonly DependencyProperty SelectionSwitchProperty =
            DependencyProperty.Register(nameof(SelectionSwitch), typeof(SelectionSwitch), typeof(ImageEx), new PropertyMetadata(SelectionSwitch.OFF));

        /// <summary>
        /// 选取区域颜色
        /// </summary>
        public Brush SelectionStroke
        {
            get { return (Brush)GetValue(SelectionStrokeProperty); }
            set { SetValue(SelectionStrokeProperty, value); }
        }
        public static readonly DependencyProperty SelectionStrokeProperty =
            DependencyProperty.Register(nameof(SelectionStroke), typeof(Brush), typeof(ImageEx), new PropertyMetadata(new SolidColorBrush(Colors.Red), OnSelectionStrokePropertyChanged));
        public static void OnSelectionStrokePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ImageEx control)
            {
                if (control._dragShape != null)
                {
                    control._dragShape.Stroke = control.SelectionStroke;
                }
            }
        }

        /// <summary>
        /// 启用基准线
        /// </summary>
        public bool BaselineEnable
        {
            get { return (bool)GetValue(BaselineEnableProperty); }
            set { SetValue(BaselineEnableProperty, value); }
        }
        public static readonly DependencyProperty BaselineEnableProperty =
            DependencyProperty.Register(nameof(BaselineEnable), typeof(bool), typeof(ImageEx), new PropertyMetadata(true));

        /// <summary>
        /// 基准线颜色
        /// </summary>
        public Brush BaselineStroke
        {
            get { return (Brush)GetValue(BaselineStrokeProperty); }
            set { SetValue(BaselineStrokeProperty, value); }
        }

   


        public static readonly DependencyProperty BaselineStrokeProperty =
            DependencyProperty.Register(nameof(BaselineStroke), typeof(Brush), typeof(ImageEx), new PropertyMetadata(new SolidColorBrush(Colors.Gold)));

        /// <summary>
        /// 局部放大系数
        /// </summary>
        public double PartialZoomFactor
        {
            get { return (double)GetValue(PartialZoomFactorProperty); }
            set { SetValue(PartialZoomFactorProperty, value); }
        }
        public static readonly DependencyProperty PartialZoomFactorProperty =
            DependencyProperty.Register(nameof(PartialZoomFactor), typeof(double), typeof(ImageEx), new PropertyMetadata(1.0, OnPartialZoomFactorPropertyChanged));
        public static void OnPartialZoomFactorPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ImageEx control)
            {
                control.partialZoomControl.ZoomFactor = control.PartialZoomFactor;
            }
        }

        /// <summary>
        /// 局部放大区域半径
        /// </summary>
        public double PartialZoomRadius
        {
            get { return (double)GetValue(PartialZoomRadiusProperty); }
            set { SetValue(PartialZoomRadiusProperty, value); }
        }
        public static readonly DependencyProperty PartialZoomRadiusProperty =
            DependencyProperty.Register(nameof(PartialZoomRadius), typeof(double), typeof(ImageEx), new PropertyMetadata(100.0, OnPartialZoomRadiusPropertyChanged));
        public static void OnPartialZoomRadiusPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ImageEx control)
            {
                control.partialZoomControl.Radius = control.PartialZoomRadius;
            }
        }

        /// <summary>
        /// 局部放大区域水平偏移量
        /// </summary>
        public double PartialHorizontalOffset
        {
            get { return (double)GetValue(PartialHorizontalOffsetProperty); }
            set { SetValue(PartialHorizontalOffsetProperty, value); }
        }
        public static readonly DependencyProperty PartialHorizontalOffsetProperty =
            DependencyProperty.Register(nameof(PartialHorizontalOffset), typeof(double), typeof(ImageEx), new PropertyMetadata(0.0));

        /// <summary>
        /// 局部放大区域竖直偏移量
        /// </summary>
        public double PartialVerticalOffset
        {
            get { return (double)GetValue(PartialVerticalOffsetProperty); }
            set { SetValue(PartialVerticalOffsetProperty, value); }
        }
        public static readonly DependencyProperty PartialVerticalOffsetProperty =
            DependencyProperty.Register(nameof(PartialVerticalOffset), typeof(double), typeof(ImageEx), new PropertyMetadata(0.0));

        /// <summary>
        /// 滚轮缩放最小尺寸不小于容器(填充效果)
        /// </summary>
        public bool FullMinimizeLimit
        {
            get { return (bool)GetValue(FullMinimizeLimitProperty); }
            set { SetValue(FullMinimizeLimitProperty, value); }
        }
        public static readonly DependencyProperty FullMinimizeLimitProperty =
            DependencyProperty.Register(nameof(FullMinimizeLimit), typeof(bool), typeof(ImageEx), new PropertyMetadata(false, OnFullMinimizeLimitPropertyChanged));
        public static void OnFullMinimizeLimitPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ImageEx control)
            {
                control.RefreshImageWithStretchProperty();
            }
        }


        #region 公共逻辑处理


        public void SetLayerImageBitmapSource(BitmapImage bitmapImage)
        {
            this.Cts?.Cancel();

            this.SetTransformImageSource(null);

            this.DisposeLayerImageSource();

            this.defaultImageBorder.Visibility = Visibility.Collapsed;

            try
            {
                if (bitmapImage != null)
                {
                    this._bitmapImage = bitmapImage;

                    this.layerImage.Source = _bitmapImage;

                    RefreshImageWithStretchProperty();
                }
                else
                {
                    this.defaultImageBorder.Visibility = Visibility.Visible;
                }

                this.loadingControl.Status = ImageLoadingStatus.Loaded;
            }
            catch (Exception ex)
            {
                string message = ex.InnerException == null ? ex.Message : ex.InnerException.Message;

                loadingControl.Status = ImageLoadingStatus.Failed;
                loadingControl.Fault = $"{message}";

                TLog.Error(ex, $"图像解析失败");
            }
        }

        /// <summary>
        /// 从新的地址解析图像
        /// </summary>
        /// <returns></returns>
        private async Task RefreshImageWithUriPropertyAsync()
        {
            if (string.IsNullOrWhiteSpace(this.UriSource) || this.UriSource.Equals(_defaultUriSource))
            {
                this.defaultImageBorder.Visibility = Visibility.Visible;
                loadingControl.Status = ImageLoadingStatus.Waiting;
                DisposeLayerImageSource();
                return;
            }

            try
            {
                this.defaultImageBorder.Visibility = Visibility.Collapsed;
                this.Cts = new CancellationTokenSource();
                loadingControl.Status = ImageLoadingStatus.Loading;

                ImageDecodeRequest request = new ImageDecodeRequest() { Url = this.UriSource, Cts = this.Cts };
                var decodeResponse = await ImageExHelper.DecodeImageStreamAsync(request, this.LocalCache);

                DisposeLayerImageSource();

                if (decodeResponse != null && decodeResponse.Url.Equals(this.UriSource))
                {
                    if (decodeResponse.Stream != null)
                    {
                        Size size = Size.Empty;
                        decodeResponse.Stream.Seek(0, SeekOrigin.Begin);
                        using (MemoryStream stream = new MemoryStream())
                        {
                            await decodeResponse.Stream.CopyToAsync(stream);

                            stream.Seek(0, SeekOrigin.Begin);

                            //解析原始尺寸
                            using (System.Drawing.Image image = System.Drawing.Image.FromStream(stream))
                            {
                                size = new Size(image.Width, image.Height);
                            }
                        }
                        decodeResponse.Stream.Seek(0, SeekOrigin.Begin);

                        _bitmapImage = new BitmapImage();
                        _bitmapImage.DecodeFailed += BitmapImage_DecodeFailed;
                        _bitmapImage.BeginInit();
                        _bitmapImage.CacheOption = BitmapCacheOption.None;
                        _bitmapImage.StreamSource = decodeResponse.Stream;
                        if (this.NarrowDecode)
                        {
                            _bitmapImage.DecodePixelWidth = (int)(size.Width * 0.25);
                            _bitmapImage.DecodePixelHeight = (int)(size.Height * 0.25);
                        }
                        _bitmapImage.EndInit();
                        this.layerImage.Source = _bitmapImage;

                        loadingControl.Status = ImageLoadingStatus.Loaded;

                        RefreshImageWithStretchProperty();

                        this.DecodeCompleted?.Invoke(this);
                    }
                    else
                    {
                        if (decodeResponse.Exception == null)
                        {
                            loadingControl.Status = ImageLoadingStatus.Failed;
                            loadingControl.Fault = this.UriSource;
                        }
                        else
                        {
                            string message = decodeResponse.Exception.InnerException == null ? decodeResponse.Exception.Message : decodeResponse.Exception.InnerException.Message;

                            loadingControl.Status = ImageLoadingStatus.Failed;
                            loadingControl.Fault = $"{message}{Environment.NewLine}{this.UriSource}";

                            TLog.Error(decodeResponse.Exception, $"图像解析失败：{this.UriSource}{Environment.NewLine}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string message = ex.InnerException == null ? ex.Message : ex.InnerException.Message;

                loadingControl.Status = ImageLoadingStatus.Failed;
                loadingControl.Fault = $"{message}{Environment.NewLine}{this.UriSource}";
                DisposeLayerImageSource();

                TLog.Error(ex, $"图像解析失败：{this.UriSource}{Environment.NewLine}");
            }
        }

        /// <summary>
        /// 释放底图资源
        /// </summary>
        public void DisposeLayerImageSource()
        {
            this.layerImage.Source = null;

            if (this._bitmapImage != null)
            {
                this._bitmapImage.DecodeFailed -= BitmapImage_DecodeFailed;
                this._bitmapImage.StreamSource?.Dispose();
                this._bitmapImage = null;
            }
        }

        private void BitmapImage_DecodeFailed(object sender, ExceptionEventArgs e)
        {
            loadingControl.Status = ImageLoadingStatus.Failed;
            loadingControl.Fault = $"{e.ErrorException.Message}{Environment.NewLine}{this.UriSource}";

            TLog.Error(e.ErrorException, $"图像解析失败：{this.UriSource}{Environment.NewLine}");
        }

        /// <summary>
        /// 点击刷新
        /// </summary>
        private async void LoadingControl_Click()
        {
            Clipboard.SetDataObject(this.UriSource);
            await this.RefreshImageWithUriPropertyAsync();
        }

        /// <summary>
        /// 根据图片的填充方式刷新图像
        /// </summary>
        private void RefreshImageWithStretchProperty()
        {
            switch (this.ZoomMode)
            {
                case ImageZoomMode.None:
                    this.layerImage.Stretch = this.Stretch;
                    this.layerImage.Width = this.RootGrid.ActualWidth;
                    this.layerImage.Height = this.RootGrid.ActualHeight;
                    this.layerImage.HorizontalAlignment = HorizontalAlignment.Stretch;
                    this.layerImage.VerticalAlignment = VerticalAlignment.Stretch;
                    this.layerImage.Margin = new Thickness(0);
                    break;

                case ImageZoomMode.Full:
                    this.layerImage.Stretch = Stretch.Fill;
                    this.layerImage.HorizontalAlignment = HorizontalAlignment.Left;
                    this.layerImage.VerticalAlignment = VerticalAlignment.Top;
                    this.layerImage.Margin = new Thickness(0);
                    RefreshImageWithZoomFullProperty();
                    break;

                case ImageZoomMode.Partial:
                    this.layerImage.Stretch = this.Stretch == Stretch.Fill ? Stretch.Fill : Stretch.Uniform;
                    this.layerImage.Width = this.RootGrid.ActualWidth;
                    this.layerImage.Height = this.RootGrid.ActualHeight;
                    this.layerImage.HorizontalAlignment = HorizontalAlignment.Stretch;
                    this.layerImage.VerticalAlignment = VerticalAlignment.Stretch;
                    this.layerImage.Margin = new Thickness(0);
                    //
                    if (this.partialZoomControl.BitmapImage != this._bitmapImage)
                    {
                        this.partialZoomControl.BitmapImage = this._bitmapImage;
                    }
                    break;
            }
        }

        /// <summary>
        /// 根节点Grid双击切换交互开关状态
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RootGrid_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (SelectionMode == SelectionMode.Enable && SelectionSwitchChangeMode == SelectionSwitchChangeMode.RightButtonDoubleClicked && e.ClickCount == 2)
            {
                switch (SelectionSwitch)
                {
                    case SelectionSwitch.OFF:
                        SelectionSwitch = SelectionSwitch.ON;
                        this.partialZoomControl.Visibility = Visibility.Collapsed;
                        this.baselineControl.Visibility = Visibility.Visible;
                        break;

                    case SelectionSwitch.ON:
                        SelectionSwitch = SelectionSwitch.OFF;
                        this.baselineControl.Visibility = Visibility.Collapsed;
                        break;
                }
            }
        }

        private void RootGrid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            RefreshImageWithStretchProperty();
        }

        private void DragThumb_MouseMove(object sender, MouseEventArgs e)
        {
            Point position = e.GetPosition(this.layerImage);
            this.baselineControl.OffsetX = position.X;
            this.baselineControl.OffsetY = position.Y;

            //对齐线控制
            if (this.SelectionSwitch == SelectionSwitch.ON && this.BaselineEnable == true && e.LeftButton == MouseButtonState.Released)
            {
                this.baselineControl.Visibility = Visibility.Visible;
            }
            else
            {
                this.baselineControl.Visibility = Visibility.Collapsed;
            }

            //局部放大区域控制
            if (this.SelectionSwitch == SelectionSwitch.OFF && this.ZoomMode == ImageZoomMode.Partial)
            {
                this.partialZoomControl.Visibility = Visibility.Visible;
                this.partialZoomControl.NormalizedCenter = new Point() { X = position.X / this.layerImage.ActualWidth, Y = position.Y / this.layerImage.ActualHeight };


                double left = 0.0;
                if (this.PartialHorizontalOffset > 0)
                {
                    left = position.X + this.PartialHorizontalOffset;
                    if (left + 2 * this.PartialZoomRadius > this.layerImage.ActualWidth)
                    {
                        left = position.X - this.PartialHorizontalOffset - 2 * this.PartialZoomRadius;
                    }
                }
                else if (this.PartialHorizontalOffset < 0)
                {
                    left = position.X + this.PartialHorizontalOffset - 2 * this.PartialZoomRadius;
                    if (left < 0)
                    {
                        left = position.X - this.PartialHorizontalOffset;
                    }
                }
                else
                {
                    left = position.X - this.PartialZoomRadius;
                }

                double top = 0.0;
                if (this.PartialVerticalOffset > 0)
                {
                    top = position.Y + this.PartialVerticalOffset;
                    if (top + 2 * this.PartialZoomRadius > this.layerImage.ActualHeight)
                    {
                        top = position.Y - this.PartialVerticalOffset - 2 * this.PartialZoomRadius;
                    }
                }
                else if (this.PartialVerticalOffset < 0)
                {
                    top = position.Y + this.PartialVerticalOffset - 2 * this.PartialZoomRadius;
                    if (top < 0)
                    {
                        top = position.Y - this.PartialVerticalOffset;
                    }
                }
                else
                {
                    top = position.Y - this.PartialZoomRadius;
                }

                this.partialZoomControl.Margin = new Thickness(left, top, 0, 0);
            }
            else
            {
                this.partialZoomControl.Visibility = Visibility.Collapsed;
            }
        }

        private void DragThumb_MouseLeave(object sender, MouseEventArgs e)
        {
            this.baselineControl.Visibility = Visibility.Collapsed;
            this.partialZoomControl.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// 拖放起点
        /// </summary>
        private Point _dragStartPoint = new Point(0, 0);
        /// <summary>
        /// 拖放示意图形
        /// </summary>
        private Shape _dragShape;

        private void DragThumb_DragStarted(object sender, System.Windows.Controls.Primitives.DragStartedEventArgs e)
        {
            _dragStartPoint = new Point(e.HorizontalOffset, e.VerticalOffset);
        }

        private void DragThumb_DragDelta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        {
            Point offset = new Point(e.HorizontalChange, e.VerticalChange);

            if (this.SelectionSwitch == SelectionSwitch.OFF)
            {
                if (this.ZoomMode == ImageZoomMode.Full)
                {
                    double left = this.layerImage.Margin.Left + offset.X;
                    double top = this.layerImage.Margin.Top + offset.Y;
                    //左
                    if (this.layerImage.ActualWidth <= this.RootGrid.ActualWidth)
                    {
                        left = (this.RootGrid.ActualWidth - this.layerImage.Width) / 2;
                    }
                    else
                    {
                        left = Math.Min(left, 0);
                        left = Math.Max(this.RootGrid.ActualWidth - this.layerImage.ActualWidth, left);
                    }
                    //上
                    if (this.layerImage.ActualHeight <= this.RootGrid.ActualHeight)
                    {
                        top = (this.RootGrid.ActualHeight - this.layerImage.Height) / 2;
                    }
                    else
                    {
                        top = Math.Min(top, 0);
                        top = Math.Max(this.RootGrid.ActualHeight - this.layerImage.ActualHeight, top);
                    }
                    this.layerImage.Margin = new Thickness(left, top, 0, 0);
                }
                else if (this.ZoomMode == ImageZoomMode.Partial)
                {
                    Point position = Mouse.GetPosition(this.layerImage);

                    if (position.X < 0 || position.X > this.layerImage.ActualWidth || position.Y < 0 || position.Y > this.layerImage.ActualHeight)
                    {
                        this.partialZoomControl.Visibility = Visibility.Collapsed;
                    }
                    else
                    {
                        this.partialZoomControl.Visibility = Visibility.Visible;
                        this.partialZoomControl.NormalizedCenter = new Point() { X = position.X / this.layerImage.ActualWidth, Y = position.Y / this.layerImage.ActualHeight };
                        this.partialZoomControl.Margin = new Thickness(position.X - this.partialZoomControl.Radius, position.Y - this.partialZoomControl.Radius, 0, 0);
                    }
                }
            }
            else
            {
                Point endPoint = GetBoundaryLimitedEndPoint(_dragStartPoint, e.HorizontalChange, e.VerticalChange);

                switch (this.SelectionType)
                {
                    case SelectionType.Point:
                        //采点不提示位置
                        break;

                    case SelectionType.Line:
                        {
                            if (!(_dragShape is Line line))
                            {
                                if (_dragShape != null)
                                {
                                    this.drawCanvas.Children.Remove(_dragShape);
                                }

                                line = new Line() { Stroke = SelectionStroke, StrokeThickness = 2, IsHitTestVisible = false, X1 = _dragStartPoint.X, Y1 = _dragStartPoint.Y };
                                this.drawCanvas.Children.Add(line);
                                _dragShape = line;
                            }
                            line.X2 = endPoint.X;
                            line.Y2 = endPoint.Y;
                        }
                        break;

                    case SelectionType.Rectangle:
                        {
                            if (!(_dragShape is Rectangle rectangle))
                            {
                                if (_dragShape != null)
                                {
                                    this.drawCanvas.Children.Remove(_dragShape);
                                }

                                rectangle = new Rectangle() { Stroke = SelectionStroke, StrokeThickness = 2, IsHitTestVisible = false };
                                this.drawCanvas.Children.Add(rectangle);
                                _dragShape = rectangle;
                            }

                            Rect rect = new Rect(_dragStartPoint, endPoint);
                            rectangle.Width = rect.Width;
                            rectangle.Height = rect.Height;
                            Canvas.SetLeft(rectangle, rect.Left);
                            Canvas.SetTop(rectangle, rect.Top);
                        }
                        break;
                }
            }
        }

        /// <summary>
        /// 获取边界内的选中区域
        /// </summary>
        /// <returns></returns>
        private Point GetBoundaryLimitedEndPoint(Point startPoint, double offsetX, double offsetY)
        {
            double x = startPoint.X + offsetX;
            double y = startPoint.Y + offsetY;

            x = Math.Max(x, 0);
            x = Math.Min(x, this.layerImage.ActualWidth);

            y = Math.Max(y, 0);
            y = Math.Min(y, this.layerImage.ActualHeight);

            return new Point(x, y);
        }

        private void DragThumb_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            if (this.SelectionSwitch == SelectionSwitch.ON)
            {
                if (_dragShape != null)
                {
                    this.drawCanvas.Children.Remove(_dragShape);
                    _dragShape = null;
                }

                Point endPoint = GetBoundaryLimitedEndPoint(_dragStartPoint, e.HorizontalChange, e.VerticalChange);
                this.SelectionCompleted?.Invoke(this, new SelectionEventArgs()
                {
                    Point1 = new Point() { X = _dragStartPoint.X / this.layerImage.ActualWidth, Y = _dragStartPoint.Y / this.layerImage.ActualHeight },
                    Point2 = new Point() { X = endPoint.X / this.layerImage.ActualWidth, Y = endPoint.Y / this.layerImage.ActualHeight }
                });
            }

            e.Handled = true;
        }

        #endregion


        #region 滚轮缩放的逻辑

        private double _minimizeScale = 1.0;
        private double _maximizeScale = 1.0;
        private double _currentScale = 1.0;

        /// <summary>
        /// 全图滚轮缩放模式下的图像刷新
        /// </summary>
        private void RefreshImageWithZoomFullProperty()
        {
            if (this._bitmapImage == null || this._bitmapImage.PixelWidth == 0 || this._bitmapImage.PixelHeight == 0)
                return;

            double pixelWidth = this._bitmapImage.PixelWidth;
            double pixelHeight = this._bitmapImage.PixelHeight;
            double containerWidth = this.RootGrid.ActualWidth;
            double containerHeight = this.RootGrid.ActualHeight;

            double xFactor = containerWidth / pixelWidth;
            double yFactor = containerHeight / pixelHeight;
            double minFactor = Math.Min(xFactor, yFactor);
            double maxFactor = Math.Max(xFactor, yFactor);

            if (this._bitmapImage.PixelWidth >= this.RootGrid.ActualWidth || this._bitmapImage.PixelHeight >= this.RootGrid.ActualHeight)
            {
                this._currentScale = minFactor;
                this._minimizeScale = minFactor;

                if (minFactor < 0.5)
                {
                    this._maximizeScale = 2;
                }
                else
                {
                    this._maximizeScale = Math.Min(maxFactor * 2, 2);
                }
            }
            else
            {
                this._currentScale = 1;
                this._minimizeScale = 1;
                this._maximizeScale = Math.Min(minFactor * 2, 10);
            }

            double left = 0, top = 0;

            if (this.FullMinimizeLimit)
            {
                this.layerImage.Width = this.RootGrid.ActualWidth;
                this.layerImage.Height = this.RootGrid.ActualHeight;
            }
            else
            {
                this.layerImage.Width = this._bitmapImage.PixelWidth * this._currentScale;
                this.layerImage.Height = this._bitmapImage.PixelHeight * this._currentScale;
                left = (this.RootGrid.ActualWidth - this.layerImage.Width) / 2;
                top = (this.RootGrid.ActualHeight - this.layerImage.Height) / 2;
            }

            this.layerImage.Margin = new Thickness(left, top, 0, 0);
        }

        /// <summary>
        /// 基于鼠标中心点的缩放
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RootGrid_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (this._bitmapImage == null || this._bitmapImage.PixelWidth == 0 || this._bitmapImage.PixelHeight == 0)
                return;

            if (ZoomMode != ImageZoomMode.Full || SelectionSwitch != SelectionSwitch.OFF)
                return;

            Point offsetRoot = e.GetPosition(RootGrid);

            Rect layerImageRect = new Rect(this.layerImage.Margin.Left, this.layerImage.Margin.Top, this.layerImage.Width, this.layerImage.Height);

            if (this.FullMinimizeLimit)
            {
                if (layerImageRect.Width == this.RootGrid.ActualWidth && layerImageRect.Height == this.RootGrid.ActualHeight)
                {
                    //禁止缩小
                    if (e.Delta < 0)
                        return;

                    //还原等比尺寸，修正中心点
                    layerImageRect.Width = this._bitmapImage.PixelWidth * this._currentScale;
                    layerImageRect.Height = this._bitmapImage.PixelHeight * this._currentScale;
                    layerImageRect.X = (this.RootGrid.ActualWidth - layerImageRect.Width) / 2;
                    layerImageRect.Y = (this.RootGrid.ActualHeight - layerImageRect.Height) / 2;

                    double x_fix = offsetRoot.X / this.RootGrid.ActualWidth * layerImageRect.Width + layerImageRect.X;
                    double y_fix = offsetRoot.Y / this.RootGrid.ActualHeight * layerImageRect.Height + layerImageRect.Y;

                    offsetRoot = new Point(x_fix, y_fix);
                }
            }

            double preDelta = this._currentScale;

            this._currentScale += e.Delta / 600.0;
            this._currentScale = Math.Max(this._currentScale, this._minimizeScale);
            this._currentScale = Math.Min(this._currentScale, this._maximizeScale);

            //新旧比例系数
            var deltaScaleFactor = this._currentScale / preDelta;

            double left = offsetRoot.X - (offsetRoot.X - layerImageRect.X) * deltaScaleFactor;
            double top = offsetRoot.Y - (offsetRoot.Y - layerImageRect.Y) * deltaScaleFactor;

            double targetWidth = this._bitmapImage.PixelWidth * this._currentScale;
            double targetHeight = this._bitmapImage.PixelHeight * this._currentScale;

            //左
            if (targetWidth <= RootGrid.ActualWidth)
            {
                left = (RootGrid.ActualWidth - targetWidth) / 2;
            }
            else
            {
                left = Math.Min(left, 0);
                left = Math.Max(RootGrid.ActualWidth - targetWidth, left);
            }
            //上
            if (targetHeight <= RootGrid.ActualHeight)
            {
                top = (RootGrid.ActualHeight - targetHeight) / 2;
            }
            else
            {
                top = Math.Min(top, 0);
                top = Math.Max(RootGrid.ActualHeight - targetHeight, top);
            }

            //设置大小
            this.layerImage.Width = targetWidth;
            this.layerImage.Height = targetHeight;
            this.layerImage.Margin = new Thickness(left, top, 0, 0);

            //最小化限制
            if (this.FullMinimizeLimit && (this.layerImage.Width < this.RootGrid.ActualWidth || this.layerImage.Height < this.RootGrid.ActualHeight))
            {
                this.layerImage.Width = this.RootGrid.ActualWidth;
                this.layerImage.Height = this.RootGrid.ActualHeight;
                this.layerImage.Margin = new Thickness(0);
            }

            e.Handled = true;
        }

        #endregion


        private void DrawCanvas_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            this.CanvasSizeChanged?.Invoke(sender, e);
        }


        #region 图像处理

        public FrameworkElement HeaderContent
        {
            get { return (FrameworkElement)GetValue(HeaderContentProperty); }
            set { SetValue(HeaderContentProperty, value); }
        }
        public static readonly DependencyProperty HeaderContentProperty =
        DependencyProperty.Register(nameof(HeaderContent), typeof(FrameworkElement), typeof(ImageEx), new FrameworkPropertyMetadata(null));

        /// <summary>
        /// 设置变换后的图像
        /// </summary>
        /// <param name="bitmapImage"></param>
        public void SetTransformImageSource(BitmapImage bitmapImage)
        {
            if (this.transformImage.Source != null)
            {
                if (this.transformImage.Source is BitmapImage bitmap)
                {
                    bitmap.StreamSource.Dispose();
                    bitmap.StreamSource = null;
                    bitmap = null;
                }
            }
            this.transformImage.Source = bitmapImage;
        }

        #endregion


        #region 闪烁效果


        /// <summary>
        /// 闪烁效果颜色
        /// </summary>
        public Color FlashingColor
        {
            get { return (Color)GetValue(FlashingColorProperty); }
            set { SetValue(FlashingColorProperty, value); }
        }
        public static readonly DependencyProperty FlashingColorProperty =
            DependencyProperty.Register(nameof(FlashingColor), typeof(Color), typeof(ImageEx), new PropertyMetadata(Colors.Red));

        /// <summary>
        /// 开启闪烁
        /// </summary>
        /// <param name="repeatTimes">闪烁次数</param>
        public void Flashing(double repeatTimes = 5)
        {
            Storyboard storyboard = this.FindResource("FlashingStoryboard") as Storyboard;
            if (storyboard != null)
            {
                foreach (var timeline in storyboard.Children)
                {
                    timeline.RepeatBehavior = new RepeatBehavior(repeatTimes);
                }
                storyboard.Begin();
            }
        }

        #endregion

    }
}
