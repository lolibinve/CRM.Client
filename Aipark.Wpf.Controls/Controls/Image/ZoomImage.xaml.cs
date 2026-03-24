using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Aipark.Wpf.Extensions;
using T.Wpf.Controls.Extensions;

namespace Aipark.Wpf.Controls
{
    /// <summary>
    /// ZoomImage.xaml 的交互逻辑
    /// </summary>
    public partial class ZoomImage : UserControl
    {
        /// <summary>
        /// 区域选取完成事件
        /// </summary>
        public event Action<ZoomImage, Rect> DragCompleted;

        /// <summary>
        /// 图像解码完成事件
        /// </summary>
        public event Action<ZoomImage> DecodeCompleted;

        public ZoomImage()
        {
            InitializeComponent();
        }

        public Canvas Canvas => this.canvas;

        /// <summary>
        /// 图像原始分辨率宽度
        /// </summary>
        public int OriginalPixelWidth
        {
            get { return (int)GetValue(OriginalPixelWidthProperty); }
            private set { SetValue(OriginalPixelWidthProperty, value); }
        }
        public static readonly DependencyProperty OriginalPixelWidthProperty =
            DependencyProperty.Register(nameof(OriginalPixelWidth), typeof(int), typeof(ZoomImage), new PropertyMetadata(0));

        /// <summary>
        /// 图像原始分辨率高度
        /// </summary>
        public int OriginalPixelHeight
        {
            get { return (int)GetValue(OriginalPixelHeightProperty); }
            private set { SetValue(OriginalPixelHeightProperty, value); }
        }
        public static readonly DependencyProperty OriginalPixelHeightProperty =
            DependencyProperty.Register(nameof(OriginalPixelHeight), typeof(int), typeof(ZoomImage), new PropertyMetadata(0));

        /// <summary>
        /// 获取或设置图像解码分辨率倍数
        /// </summary>
        public double DecodeFactor
        {
            get { return (double)GetValue(DecodeFactorProperty); }
            set { SetValue(DecodeFactorProperty, value); }
        }
        public static readonly DependencyProperty DecodeFactorProperty =
            DependencyProperty.Register(nameof(DecodeFactor), typeof(double), typeof(ZoomImage), new PropertyMetadata(1.0, DecodeFactorPropertyChanged));
        public static void DecodeFactorPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ZoomImage control)
            {
                control.RefreshImage();
            }
        }

        /// <summary>
        /// 图像字节信息
        /// </summary>
        public byte[] OriginalBytes
        {
            get { return (byte[])GetValue(OriginalBytesProperty); }
            set { SetValue(OriginalBytesProperty, value); }
        }
        public static readonly DependencyProperty OriginalBytesProperty =
            DependencyProperty.Register(nameof(OriginalBytes), typeof(byte[]), typeof(ZoomImage), new PropertyMetadata(null));

        /// <summary>
        /// 图像源地址(支持网络图像或本地图像)
        /// </summary>
        public string UriSource
        {
            get { return (string)GetValue(UriSourceProperty); }
            set { SetValue(UriSourceProperty, value); }
        }
        public static readonly DependencyProperty UriSourceProperty =
            DependencyProperty.Register("UriSource", typeof(string), typeof(ZoomImage), new PropertyMetadata("!@#$%^&*", OnUriSourcePropertyChanged));
        public static async void OnUriSourcePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ZoomImage control)
            {
                await control.DecodeUriSourceAsync();
            }
        }

        private async Task DecodeUriSourceAsync()
        {
            if (!string.IsNullOrWhiteSpace(this.UriSource))
            {
                this.progressControl.SetUriSource(this.UriSource);
                this.progressControl.State = ImageState.Loading;

                MemoryStream memoryStream = null;

                Uri uri = new Uri(this.UriSource, UriKind.RelativeOrAbsolute);
                try
                {
                    if (uri.IsAbsoluteUri && (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps))
                    {
                        memoryStream = await ImageCache.GetMemoryStream(uri);
                    }
                    else
                    {
                        memoryStream = await uri.GetImageStreamAsync();
                    }

                    //缓存流
                    this.OriginalBytes = this.OriginalBytes ?? memoryStream?.ToArray();

                    if (memoryStream == null)
                    {
                        this.progressControl.State = ImageState.Failed;
                        this.layerImage.Source = null;
                        this.OriginalPixelWidth = 0;
                        this.OriginalPixelHeight = 0;
                    }
                    else
                    {
                        using (System.Drawing.Image image = System.Drawing.Image.FromStream(memoryStream))
                        {
                            this.OriginalPixelWidth = image.Width;
                            this.OriginalPixelHeight = image.Height;
                        }

                        //底图渲染
                        this.RefreshImage(memoryStream);

                        //放大镜基准图渲染
                        this.RefreshMagnifyingImage(memoryStream);

                        this.progressControl.State = ImageState.Loaded;
                    }
                }
                catch (Exception ex)
                {
                    this.progressControl.State = ImageState.Failed;
                    this.layerImage.Source = null;
                    this.OriginalBytes = null;
                    this.OriginalPixelWidth = 0;
                    this.OriginalPixelHeight = 0;
#if DEBUG
                    Console.WriteLine($"图像解析失败({nameof(ZoomImage)})：{uri.OriginalString},{ex.StackTrace}");
#endif
                }
                finally
                {
                    memoryStream?.Dispose();
                }
            }
            else
            {
                this.progressControl.State = ImageState.EmptyUrl;
                this.layerImage.Source = null;
                this.OriginalBytes = null;
                this.OriginalPixelWidth = 0;
                this.OriginalPixelHeight = 0;
            }

            this.DecodeCompleted?.Invoke(this);
        }

        public void RefreshImage(MemoryStream memoryStream = null)
        {
            if (this.OriginalBytes != null || memoryStream != null)
            {
                int decodePixelWidth = this.OriginalPixelWidth;
                int decodePixelHeight = this.OriginalPixelHeight;
                if (this.DecodeFactor > 0)
                {
                    decodePixelWidth = (int)(this.OriginalPixelWidth * this.DecodeFactor);
                    decodePixelHeight = (int)(this.OriginalPixelHeight * this.DecodeFactor);
                }

                memoryStream = memoryStream ?? new MemoryStream(OriginalBytes);
                var bitmapImage = BitmapImageExtensions.FromStream(memoryStream, decodePixelWidth, decodePixelHeight);
                this.layerImage.Source = bitmapImage;
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
            DependencyProperty.Register("Stretch", typeof(Stretch), typeof(ZoomImage), new PropertyMetadata(Stretch.Uniform));

        /// <summary>
        /// 开启放大镜效果
        /// </summary>
        public bool OpenMagnifying
        {
            get { return (bool)GetValue(OpenMagnifyingProperty); }
            set { SetValue(OpenMagnifyingProperty, value); }
        }
        public static readonly DependencyProperty OpenMagnifyingProperty =
            DependencyProperty.Register(nameof(OpenMagnifying), typeof(bool), typeof(ZoomImage), new PropertyMetadata(true, OpenMagnifyingPropertyChanged));
        public static void OpenMagnifyingPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ZoomImage control)
            {
                control.RefreshMagnifyingImage();
            }
        }

        /// <summary>
        /// 放大镜倍率(相对图像原始分辨率)
        /// </summary>
        public double MagnifyingFactor
        {
            get { return (double)GetValue(MagnifyingFactorProperty); }
            set { SetValue(MagnifyingFactorProperty, value); }
        }
        public static readonly DependencyProperty MagnifyingFactorProperty =
            DependencyProperty.Register(nameof(MagnifyingFactor), typeof(double), typeof(ZoomImage), new PropertyMetadata(1.0, MagnifyingFactorPropertyChanged));
        public static void MagnifyingFactorPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ZoomImage control)
            {
                control.RefreshMagnifyingImage();
            }
        }

        /// <summary>
        /// 放大镜区域宽度
        /// </summary>
        public int MagnifyingWidth
        {
            get { return (int)GetValue(MagnifyingWidthProperty); }
            set { SetValue(MagnifyingWidthProperty, value); }
        }
        public static readonly DependencyProperty MagnifyingWidthProperty =
            DependencyProperty.Register(nameof(MagnifyingWidth), typeof(int), typeof(ZoomImage), new PropertyMetadata(200, MagnifyingWidthPropertyChanged));
        public static void MagnifyingWidthPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ZoomImage control)
            {
                control.RefreshMagnifyingCanvas();
            }
        }

        /// <summary>
        /// 放大镜区域高度
        /// </summary>
        public int MagnifyingHeight
        {
            get { return (int)GetValue(MagnifyingHeightProperty); }
            set { SetValue(MagnifyingHeightProperty, value); }
        }
        public static readonly DependencyProperty MagnifyingHeightProperty =
            DependencyProperty.Register(nameof(MagnifyingHeight), typeof(int), typeof(ZoomImage), new PropertyMetadata(200, MagnifyingHeightPropertyChanged));
        public static void MagnifyingHeightPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ZoomImage control)
            {
                control.RefreshMagnifyingCanvas();
            }
        }

        /// <summary>
        /// 放大镜中心点
        /// </summary>
        public Point MagnifyingCenterPoint
        {
            get { return (Point)GetValue(MagnifyingCenterPointProperty); }
            set { SetValue(MagnifyingCenterPointProperty, value); }
        }
        public static readonly DependencyProperty MagnifyingCenterPointProperty =
            DependencyProperty.Register(nameof(MagnifyingCenterPoint), typeof(Point), typeof(ZoomImage), new PropertyMetadata(new Point(100, 100)));

        /// <summary>
        /// 
        /// </summary>
        public double MagnifyingRadiusX
        {
            get { return (double)GetValue(MagnifyingRadiusXProperty); }
            set { SetValue(MagnifyingRadiusXProperty, value); }
        }
        public static readonly DependencyProperty MagnifyingRadiusXProperty =
            DependencyProperty.Register(nameof(MagnifyingRadiusX), typeof(double), typeof(ZoomImage), new PropertyMetadata(100.0));

        /// <summary>
        /// 
        /// </summary>
        public double MagnifyingRadiusY
        {
            get { return (double)GetValue(MagnifyingRadiusYProperty); }
            set { SetValue(MagnifyingRadiusYProperty, value); }
        }
        public static readonly DependencyProperty MagnifyingRadiusYProperty =
            DependencyProperty.Register(nameof(MagnifyingRadiusY), typeof(double), typeof(ZoomImage), new PropertyMetadata(100.0));

        /// <summary>
        /// 放大镜区域边框颜色
        /// </summary>
        public Brush MagnifyingBorderBrush
        {
            get { return (Brush)GetValue(MagnifyingBorderBrushProperty); }
            set { SetValue(MagnifyingBorderBrushProperty, value); }
        }
        public static readonly DependencyProperty MagnifyingBorderBrushProperty =
            DependencyProperty.Register(nameof(MagnifyingBorderBrush), typeof(Brush), typeof(ZoomImage), new PropertyMetadata(new SolidColorBrush(Colors.Silver)));

        /// <summary>
        /// 放大镜区域边框粗细
        /// </summary>
        public double MagnifyingBorderThickness
        {
            get { return (double)GetValue(MagnifyingBorderThicknessProperty); }
            set { SetValue(MagnifyingBorderThicknessProperty, value); }
        }
        public static readonly DependencyProperty MagnifyingBorderThicknessProperty =
            DependencyProperty.Register(nameof(MagnifyingBorderThickness), typeof(double), typeof(ZoomImage), new PropertyMetadata(1.0));

        /// <summary>
        /// 放大镜区域的水平对齐方式
        /// </summary>
        public HorizontalAlignment MagnifyingHorizontalAlignment
        {
            get { return (HorizontalAlignment)GetValue(MagnifyingHorizontalAlignmentProperty); }
            set { SetValue(MagnifyingHorizontalAlignmentProperty, value); }
        }
        public static readonly DependencyProperty MagnifyingHorizontalAlignmentProperty =
            DependencyProperty.Register(nameof(MagnifyingHorizontalAlignment), typeof(HorizontalAlignment), typeof(ZoomImage), new PropertyMetadata(HorizontalAlignment.Stretch));

        /// <summary>
        /// 放大镜区域竖直对齐方式
        /// </summary>
        public VerticalAlignment MagnifyingVerticalAlignment
        {
            get { return (VerticalAlignment)GetValue(MagnifyingVerticalAlignmentProperty); }
            set { SetValue(MagnifyingVerticalAlignmentProperty, value); }
        }
        public static readonly DependencyProperty MagnifyingVerticalAlignmentProperty =
            DependencyProperty.Register(nameof(MagnifyingVerticalAlignment), typeof(VerticalAlignment), typeof(ZoomImage), new PropertyMetadata(VerticalAlignment.Stretch));

        /// <summary>
        /// 放大镜区域的对齐间距
        /// </summary>
        public Thickness MagnifyingThickness
        {
            get { return (Thickness)GetValue(MagnifyingThicknessProperty); }
            set { SetValue(MagnifyingThicknessProperty, value); }
        }
        public static readonly DependencyProperty MagnifyingThicknessProperty =
            DependencyProperty.Register("MagnifyingThickness", typeof(Thickness), typeof(ZoomImage), new PropertyMetadata(new Thickness(0)));

        public void RefreshMagnifyingImage(MemoryStream memoryStream = null)
        {
            if (this.OpenMagnifying && (this.OriginalBytes != null || memoryStream != null))
            {
                int decodePixelWidth = this.OriginalPixelWidth;
                int decodePixelHeight = this.OriginalPixelHeight;
                if (this.MagnifyingFactor > 0)
                {
                    decodePixelWidth = (int)(this.OriginalPixelWidth * this.MagnifyingFactor);
                    decodePixelHeight = (int)(this.OriginalPixelHeight * this.MagnifyingFactor);
                }

                memoryStream = memoryStream ?? new MemoryStream(this.OriginalBytes);
                var bitmapImage = BitmapImageExtensions.FromStream(memoryStream, decodePixelWidth, decodePixelHeight);
                this.magnifyingImage.Source = bitmapImage;
            }
            else
            {
                this.magnifyingCanvas.Visibility = Visibility.Collapsed;
                this.magnifyingImage.Source = null;
            }
        }

        /// <summary>
        /// 开启区域选取操作
        /// </summary>
        public bool OpenDrag
        {
            get { return (bool)GetValue(OpenDragProperty); }
            set { SetValue(OpenDragProperty, value); }
        }
        public static readonly DependencyProperty OpenDragProperty =
            DependencyProperty.Register(nameof(OpenDrag), typeof(bool), typeof(ZoomImage), new PropertyMetadata(true));

        /// <summary>
        /// 区域框颜色
        /// </summary>
        public Brush DragRectangleBorderBrush
        {
            get { return (Brush)GetValue(DragRectangleBorderBrushProperty); }
            set { SetValue(DragRectangleBorderBrushProperty, value); }
        }
        public static readonly DependencyProperty DragRectangleBorderBrushProperty =
            DependencyProperty.Register(nameof(DragRectangleBorderBrush), typeof(Brush), typeof(ZoomImage), new PropertyMetadata(new SolidColorBrush(Colors.Red), DragRectangleBorderBrushPropertyChanged));
        public static void DragRectangleBorderBrushPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ZoomImage control)
            {
                if (control._dragRectangle != null)
                {
                    control._dragRectangle.Stroke = control.DragRectangleBorderBrush;
                }
            }
        }

        public void RefreshMagnifyingCanvas()
        {
            this.MagnifyingCenterPoint = new Point() { X = this.MagnifyingWidth / 2, Y = this.MagnifyingHeight / 2 };
            this.MagnifyingRadiusX = this.MagnifyingCenterPoint.X;
            this.MagnifyingRadiusY = this.MagnifyingCenterPoint.Y;
        }

        #region 放大镜跟随

        private void Thumb_MouseEnter(object sender, MouseEventArgs e)
        {
            this.magnifyingCanvas.Visibility = OpenMagnifying ? Visibility.Visible : Visibility.Collapsed;
        }

        private void Thumb_MouseMove(object sender, MouseEventArgs e)
        {
            if (!OpenMagnifying)
                return;

            if (this.magnifyingCanvas.Visibility == Visibility.Collapsed)
                this.magnifyingCanvas.Visibility = Visibility.Visible;

            Point p = e.GetPosition(this.layerImage);

            double x = p.X / this.layerImage.ActualWidth;
            double y = p.Y / this.layerImage.ActualHeight;

            if (magnifyingImage.Source is BitmapImage bitmapImage)
            {
                double imgLeft = -x * bitmapImage.PixelWidth + this.MagnifyingWidth / 2;
                double imgTop = -y * bitmapImage.PixelHeight + this.MagnifyingHeight / 2;

                this.magnifyingImage.Margin = new Thickness(imgLeft, imgTop, 0, 0);
            }

            //放大镜左侧间距
            double contentLeft = p.X - this.MagnifyingWidth / 2;
            switch (this.MagnifyingHorizontalAlignment)
            {
                case HorizontalAlignment.Left:
                    contentLeft = p.X - this.MagnifyingWidth;
                    break;
                case HorizontalAlignment.Right:
                    contentLeft = p.X;
                    break;
                default:
                    break;
            }

            contentLeft = contentLeft + this.MagnifyingThickness.Left;
            if (contentLeft < 0)
            {
                contentLeft = p.X - this.MagnifyingThickness.Left;
            }
            else if (contentLeft + this.MagnifyingWidth > this.layerImage.ActualWidth)
            {
                contentLeft = p.X - this.MagnifyingWidth + this.MagnifyingThickness.Left;
            }

            //放大镜右侧间距
            double contentTop = p.Y - this.MagnifyingHeight / 2;
            switch (this.MagnifyingVerticalAlignment)
            {
                case VerticalAlignment.Top:
                    contentTop = p.Y - this.MagnifyingHeight;
                    break;
                case VerticalAlignment.Bottom:
                    contentTop = p.Y;
                    break;
                default:
                    break;
            }

            contentTop = contentTop + this.MagnifyingThickness.Top;
            if (contentTop < 0)
            {
                contentTop = p.Y - this.MagnifyingThickness.Top;
            }
            else if (contentTop + this.MagnifyingHeight > this.layerImage.ActualHeight)
            {
                contentTop = p.Y - this.MagnifyingHeight + this.MagnifyingThickness.Top;
            }

            this.magnifyingContent.Margin = new Thickness() { Left = contentLeft, Top = contentTop, Right = 0, Bottom = 0 };
        }

        private void Thumb_MouseLeave(object sender, MouseEventArgs e)
        {
            this.magnifyingCanvas.Visibility = Visibility.Collapsed;
        }

        #endregion

        #region 矩形框选取操作

        private Point _dragStartedPoint = new Point();
        private Rectangle _dragRectangle = null;

        private void Thumb_DragStarted(object sender, System.Windows.Controls.Primitives.DragStartedEventArgs e)
        {
            _dragStartedPoint = new Point(e.HorizontalOffset, e.VerticalOffset);
            this.magnifyingCanvas.Visibility = Visibility.Collapsed;
        }

        private void Thumb_DragDelta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        {
            if (!OpenDrag)
                return;

            Point offset = new Point(e.HorizontalChange, e.VerticalChange);

            Rect rect = CalculateEffectiveRect(_dragStartedPoint, new Point(_dragStartedPoint.X + offset.X, _dragStartedPoint.Y + offset.Y));

            if (_dragRectangle == null)
            {
                _dragRectangle = new Rectangle() { Stroke = this.DragRectangleBorderBrush, StrokeThickness = 2, IsHitTestVisible = false };
                this.Canvas.Children.Add(_dragRectangle);
            }
            _dragRectangle.Width = rect.Width;
            _dragRectangle.Height = rect.Height;
            Canvas.SetLeft(_dragRectangle, rect.X);
            Canvas.SetTop(_dragRectangle, rect.Y);
        }

        private void Thumb_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            if (!OpenDrag)
                return;

            Rect rect = CalculatePercentEffectiveRect(_dragStartedPoint, new Point(_dragStartedPoint.X + e.HorizontalChange, _dragStartedPoint.Y + e.VerticalChange));
            DragCompleted?.Invoke(this, rect);
        }

        /// <summary>
        /// 计算有效框选区域
        /// </summary>
        /// <param name="startPoint"></param>
        /// <param name="endPoint"></param>
        /// <returns></returns>
        public Rect CalculateEffectiveRect(Point startPoint, Point endPoint)
        {
            Rect rect = new Rect(startPoint, endPoint);
            double x1 = Math.Min(rect.Left, this.layerImage.ActualWidth - 1);
            x1 = Math.Max(x1, 0);

            double y1 = Math.Min(rect.Top, this.layerImage.ActualHeight - 1);
            y1 = Math.Max(y1, 0);

            double x2 = Math.Min(rect.Right, this.layerImage.ActualWidth - 1);
            x2 = Math.Max(x2, 0);

            double y2 = Math.Min(rect.Bottom, this.layerImage.ActualHeight - 1);
            y2 = Math.Max(y2, 0);

            return new Rect(new Point(x1, y1), new Point(x2, y2));
        }

        /// <summary>
        /// 计算有效框选区域
        /// </summary>
        /// <param name="startPoint"></param>
        /// <param name="endPoint"></param>
        /// <returns></returns>
        public Rect CalculatePercentEffectiveRect(Point startPoint, Point endPoint)
        {
            Rect rect = new Rect(startPoint, endPoint);
            double x1 = Math.Min(rect.Left, this.layerImage.ActualWidth - 1);
            x1 = Math.Max(x1, 0);

            double y1 = Math.Min(rect.Top, this.layerImage.ActualHeight - 1);
            y1 = Math.Max(y1, 0);

            double x2 = Math.Min(rect.Right, this.layerImage.ActualWidth - 1);
            x2 = Math.Max(x2, 0);

            double y2 = Math.Min(rect.Bottom, this.layerImage.ActualHeight - 1);
            y2 = Math.Max(y2, 0);

            return new Rect(new Point(Math.Round(x1 / this.layerImage.ActualWidth, 4), Math.Round(y1 / this.layerImage.ActualHeight, 4)), new Point(Math.Round(x2 / this.layerImage.ActualWidth, 4), Math.Round(y2 / this.layerImage.ActualHeight, 4)));
        }

        public void ClearRectangle()
        {
            if (this._dragRectangle != null && this.Canvas.Children.Contains(this._dragRectangle))
            {
                this.Canvas.Children.Remove(this._dragRectangle);
                this._dragRectangle = null;
            }
        }


        #endregion

        private async void ProgressControl_RefreshButtonClick(ImageStateControl obj)
        {
            await DecodeUriSourceAsync();
        }
    }
}
