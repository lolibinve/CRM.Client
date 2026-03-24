using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using T.Wpf.Controls.Extensions;

namespace Aipark.Wpf.Controls
{
    /// <summary>
    /// ScalableImage.xaml 的交互逻辑
    /// </summary>
    public partial class ImageView : UserControl
    {
        public BitmapImage BitmapImage => this.Image.Source as BitmapImage;

        /// <summary>
        /// 当前缩放比例
        /// </summary>
        private double ZoomScale = 1;
        /// <summary>
        /// 最小缩放比例，不小于Mini(原始尺寸,当前容器)*50%
        /// </summary>
        private double MinScale = 1;
        /// <summary>
        /// 最大缩放比例
        /// </summary>
        private double MaxScale = 1;

        /// <summary>
        /// Thumb拖动起点
        /// </summary>
        private Point DragStartedPoint = new Point(0, 0);
        /// <summary>
        /// 框选的矩形区
        /// </summary>
        private Rectangle AssistRectangle = null;
        /// <summary>
        /// 水平对齐线
        /// </summary>
        private Rectangle HorizontalLine = null;
        /// <summary>
        /// 竖直对齐线
        /// </summary>
        private Rectangle VerticallLine = null;
        /// <summary>
        /// 鼠标拖放连线
        /// </summary>
        private Line AssistLine = null;

        /// <summary>
        /// 外部调用Canvas尺寸改变事件
        /// </summary>
        public event Action<object, SizeChangedEventArgs> CanvasSizeChanged;
        /// <summary>
        /// 矩形框或点位选取完成事件
        /// </summary>
        public event Action<ImageView, Rect> SelectDragCompleted;
        /// <summary>
        /// 编辑模式下设置旋转角度完成
        /// </summary>
        public event Action<ImageView, int> SelectAngleCompleted;
        /// <summary>
        /// 底图缩放比例改变事件(之前比例,当前比例)
        /// </summary>
        public event Action<double, double> ZoomDeltaChanged;
        /// <summary>
        /// 编辑模式下鼠标移动事件
        /// </summary>
        public event Action<ImageView, Point> SelectMouseMove;

        public ImageView()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 允许底图填充不留白
        /// </summary>
        public bool AlwaysFill
        {
            get { return (bool)GetValue(AlwaysFillProperty); }
            set { SetValue(AlwaysFillProperty, value); }
        }
        public static readonly DependencyProperty AlwaysFillProperty =
            DependencyProperty.Register("AlwaysFill", typeof(bool), typeof(ImageView), new PropertyMetadata(false));

        /// <summary>
        /// 图像可缩放
        /// </summary>
        public bool Zoomable
        {
            get { return (bool)GetValue(ZoomableProperty); }
            set { SetValue(ZoomableProperty, value); }
        }
        public static readonly DependencyProperty ZoomableProperty =
            DependencyProperty.Register("Zoomable", typeof(bool), typeof(ImageView), new PropertyMetadata(true, OnZoomablePropertyChanged));
        public static void OnZoomablePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ImageView view && e.NewValue is bool zoomable)
            {
                if (zoomable)
                {
                    view.RootGrid.MouseWheel += view.RootGrid_MouseWheel;
                    view.Thumb.IsHitTestVisible = true;
                }
                else
                {
                    view.RootGrid.MouseWheel -= view.RootGrid_MouseWheel;
                    view.Thumb.IsHitTestVisible = false;
                }
            }
        }

        public string UriSource
        {
            get { return (string)GetValue(UriSourceProperty); }
            set { SetValue(UriSourceProperty, value); }
        }
        public static readonly DependencyProperty UriSourceProperty =
            DependencyProperty.Register("UriSource", typeof(string), typeof(ImageView), new PropertyMetadata("!@#$%^&*", OnUriSourcePropertyChanged));
        public static async void OnUriSourcePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ImageView control)
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

                    if (memoryStream == null)
                    {
                        this.progressControl.State = ImageState.Failed;
                    }
                    else
                    {
                        memoryStream.Seek(0, SeekOrigin.Begin);

                        BitmapImage bitmapImage = new BitmapImage();
                        bitmapImage.BeginInit();
                        bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                        bitmapImage.StreamSource = memoryStream;
                        bitmapImage.EndInit();

                        this.Image.Source = bitmapImage;
                        this.UniformImageView();
                        this.progressControl.State = ImageState.Loaded;
                    }
                }
                catch (Exception ex)
                {
                    this.progressControl.State = ImageState.Failed;
                    this.Image.Source = null;
#if DEBUG
                    Console.WriteLine($"图像解析失败({nameof(ImageView)})：{uri.OriginalString},{ex.StackTrace}");
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
                this.Image.Source = null;
            }
        }

        /// <summary>
        /// 显示框选矩形框
        /// </summary>
        public bool ShowSelectRect
        {
            get { return (bool)GetValue(ShowSelectRectProperty); }
            set { SetValue(ShowSelectRectProperty, value); }
        }
        public static readonly DependencyProperty ShowSelectRectProperty =
            DependencyProperty.Register(nameof(ShowSelectRect), typeof(bool), typeof(ImageView), new PropertyMetadata(true, null));


        /// <summary>
        /// 控件交互模式
        /// </summary>
        public InteractionType Interaction
        {
            get { return (InteractionType)GetValue(InteractionProperty); }
            set { SetValue(InteractionProperty, value); }
        }
        public static readonly DependencyProperty InteractionProperty =
            DependencyProperty.Register(nameof(Interaction), typeof(InteractionType), typeof(ImageView), new PropertyMetadata(InteractionType.Browse, InteractionPropertyChanged));
        public static void InteractionPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ImageView view)
            {
                switch ((InteractionType)e.NewValue)
                {
                    case InteractionType.Browse:
                        view.Cursor = Cursors.Hand;
                        view.ClearAlignmentLines();
                        break;
                    case InteractionType.Rectangle:
                        view.Cursor = Cursors.Arrow;
                        view.CreateAlignmentLines();
                        break;
                    case InteractionType.Polygon:
                        view.Cursor = Cursors.Arrow;
                        view.ClearAlignmentLines();
                        break;
                    case InteractionType.Rotate:
                        view.Cursor = Cursors.Arrow;
                        view.ClearAlignmentLines();
                        break;
                }
            }
        }
        private void ClearAlignmentLines()
        {
            if (this.HorizontalLine != null && this.LayerGrid.Children.Contains(this.HorizontalLine))
            {
                this.LayerGrid.Children.Remove(this.HorizontalLine);
                this.HorizontalLine = null;
            }
            if (this.VerticallLine != null && this.LayerGrid.Children.Contains(this.VerticallLine))
            {
                this.LayerGrid.Children.Remove(this.VerticallLine);
                this.VerticallLine = null;
            }
        }
        private void CreateAlignmentLines()
        {
            this.HorizontalLine = new Rectangle()
            {
                Height = 1,
                Stroke = this.LineStroke,
                StrokeDashArray = new DoubleCollection() { 5, 10 },
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Top,
                IsHitTestVisible = false
            };
            this.LayerGrid.Children.Add(this.HorizontalLine);

            this.VerticallLine = new Rectangle()
            {
                Width = 1,
                Stroke = this.LineStroke,
                StrokeDashArray = new DoubleCollection() { 5, 10 },
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Stretch,
                IsHitTestVisible = false
            };
            this.LayerGrid.Children.Add(this.VerticallLine);
        }

        /// <summary>
        /// 辅助矩形框边框颜色
        /// </summary>
        public Brush RectangleStroke
        {
            get { return (Brush)GetValue(RectangleStrokeProperty); }
            set { SetValue(RectangleStrokeProperty, value); }
        }
        public static readonly DependencyProperty RectangleStrokeProperty =
            DependencyProperty.Register(nameof(RectangleStroke), typeof(Brush), typeof(ImageView), new PropertyMetadata(new SolidColorBrush(Colors.Blue), OnRectangleStrokePropertyChanged));
        public static void OnRectangleStrokePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ImageView view && view.AssistRectangle != null)
            {
                view.AssistRectangle.Stroke = (Brush)e.NewValue;
            }
        }

        /// <summary>
        /// 辅助线的颜色
        /// </summary>
        public Brush LineStroke
        {
            get { return (Brush)GetValue(LineStrokeProperty); }
            set { SetValue(LineStrokeProperty, value); }
        }
        public static readonly DependencyProperty LineStrokeProperty =
            DependencyProperty.Register(nameof(LineStroke), typeof(Brush), typeof(ImageView), new PropertyMetadata(new SolidColorBrush(Colors.Gold), OnLineStrokePropertyChanged));
        public static void OnLineStrokePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ImageView view && view.AssistLine != null)
            {
                view.AssistLine.Stroke = (Brush)e.NewValue;
            }
        }

        /// <summary>
        /// 自适应计算最佳显示比例
        /// </summary>
        public void UniformImageView()
        {
            if (this.BitmapImage == null)
                return;

            /* 按图片和容器大小的比例自适应显示
             *     图片尺寸大于容器：图片比例特别大的，最大缩放倍数适当调小；
             *                       图片比例普通的，最大缩放倍数适当调大；
             *     图片尺寸小于容器：图片极小时，长宽达不到容器的长宽的1/8，最大允许放大到容器的1/2显示；
             *                       图片较小时，长宽达不到容器的长宽的1/4，最大允许放大到容器的3/4显示；
             *                       图片普通时，可适当允许最大倍数略大；
             */

            double xZoom = RootGrid.ActualWidth / this.BitmapImage.PixelWidth;
            double yZoom = RootGrid.ActualHeight / this.BitmapImage.PixelHeight;
            ZoomScale = Math.Min(xZoom, yZoom);

            if (this.BitmapImage.PixelWidth >= RootGrid.ActualWidth || this.BitmapImage.PixelHeight >= RootGrid.ActualHeight)
            {
                //原图至少有一个维度大于容器
                if (ZoomScale < 0.5)
                {
                    //极大
                    MinScale = ZoomScale;
                    MaxScale = Math.Floor(1 / ZoomScale);
                }
                else
                {
                    MinScale = ZoomScale;
                    MaxScale = Math.Ceiling(2 / ZoomScale);
                }
            }
            else
            {
                //原图宽高两个维度均小于容器
                if (ZoomScale > 8)
                {
                    //极小
                    MinScale = 1;
                    MaxScale = Math.Floor(0.5 * ZoomScale);
                    ZoomScale = MaxScale;
                }
                else if (ZoomScale > 4)
                {
                    //较小
                    MinScale = 1;
                    MaxScale = Math.Floor(0.75 * ZoomScale);
                    ZoomScale = MaxScale;
                }
                else
                {
                    MinScale = 1;
                    MaxScale = Math.Ceiling(ZoomScale);
                    ZoomScale = 1;
                }
            }

            if (AlwaysFill == false)
            {
                LayerGrid.Width = this.BitmapImage.PixelWidth * ZoomScale;
                LayerGrid.Height = this.BitmapImage.PixelHeight * ZoomScale;

                double left = (RootGrid.ActualWidth - LayerGrid.Width) / 2;
                double top = (RootGrid.ActualHeight - LayerGrid.Height) / 2;
                LayerGrid.Margin = new Thickness(left, top, 0, 0);

                ZoomDeltaChanged?.Invoke(ZoomScale, ZoomScale);
            }
            else
            {
                LayerGrid.Width = this.RootGrid.ActualWidth;
                LayerGrid.Height = this.RootGrid.ActualHeight;
                LayerGrid.Margin = new Thickness(0, 0, 0, 0);

                ZoomScale = Math.Max(xZoom, yZoom);
            }
        }

        /// <summary>
        /// 基于鼠标中心点的缩放控制
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RootGrid_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (Interaction != InteractionType.Browse || this.BitmapImage == null)
                return;

            Point offsetRoot = e.GetPosition(RootGrid);

            double preDelta = ZoomScale;

            ZoomScale += e.Delta / 600.0;
            ZoomScale = Math.Max(ZoomScale, MinScale);
            ZoomScale = Math.Min(ZoomScale, MaxScale);

            //新旧比例系数
            var deltaScaleFactor = ZoomScale / preDelta;

            double left = offsetRoot.X - (offsetRoot.X - LayerGrid.Margin.Left) * deltaScaleFactor;
            double top = offsetRoot.Y - (offsetRoot.Y - LayerGrid.Margin.Top) * deltaScaleFactor;

            double targetWidth = this.BitmapImage.PixelWidth * ZoomScale;
            double targetHeight = this.BitmapImage.PixelHeight * ZoomScale;

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
            if (AlwaysFill == false)
            {
                LayerGrid.Width = targetWidth;
                LayerGrid.Height = targetHeight;
                LayerGrid.Margin = new Thickness(left, top, 0, 0);

                ZoomDeltaChanged?.Invoke(preDelta, ZoomScale);
            }
            else
            {
                if (targetWidth < this.RootGrid.ActualWidth)
                {
                    targetWidth = this.RootGrid.ActualWidth;
                    left = 0;
                }

                if (targetHeight < this.RootGrid.ActualHeight)
                {
                    targetHeight = this.RootGrid.ActualHeight;
                    top = 0;
                }

                LayerGrid.Width = targetWidth;
                LayerGrid.Height = targetHeight;
                LayerGrid.Margin = new Thickness(left, top, 0, 0);

                ZoomDeltaChanged?.Invoke(preDelta, ZoomScale);
            }

            e.Handled = true;
        }

        private void RootGrid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            //this.RootGrid.Clip = new RectangleGeometry(new Rect(this.BorderThickness.Left, this.BorderThickness.Top, this.RootGrid.ActualWidth, this.RootGrid.ActualHeight));
            UniformImageView();
        }

        private void Canvas_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            CanvasSizeChanged?.Invoke(sender, e);
        }

        /// <summary>
        /// 开始拖动
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DragThumb_DragStarted(object sender, DragStartedEventArgs e)
        {
            DragStartedPoint = new Point(e.HorizontalOffset, e.VerticalOffset);
        }

        /// <summary>
        /// 正在拖动
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DragThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            Point offset = new Point(e.HorizontalChange, e.VerticalChange);

            if (Interaction == InteractionType.Browse)
            {
                double left = LayerGrid.Margin.Left + offset.X;
                double top = LayerGrid.Margin.Top + offset.Y;
                //左
                if (LayerGrid.ActualWidth <= RootGrid.ActualWidth)
                {
                    left = (RootGrid.ActualWidth - LayerGrid.Width) / 2;
                }
                else
                {
                    left = Math.Min(left, 0);
                    left = Math.Max(RootGrid.ActualWidth - LayerGrid.ActualWidth, left);
                }
                //上
                if (LayerGrid.ActualHeight <= RootGrid.ActualHeight)
                {
                    top = (RootGrid.ActualHeight - LayerGrid.Height) / 2;
                }
                else
                {
                    top = Math.Min(top, 0);
                    top = Math.Max(RootGrid.ActualHeight - LayerGrid.ActualHeight, top);
                }
                //设置外边距
                LayerGrid.Margin = new Thickness(left, top, 0, 0);
            }
            else if (Interaction == InteractionType.Rectangle)
            {
                Rect rect = CalculateEffectiveRect(DragStartedPoint, new Point(DragStartedPoint.X + offset.X, DragStartedPoint.Y + offset.Y));
                if (ShowSelectRect)
                {
                    if (AssistRectangle == null)
                    {
                        AssistRectangle = new Rectangle() { Stroke = RectangleStroke, StrokeThickness = 2, IsHitTestVisible = false };
                        this.Canvas.Children.Add(AssistRectangle);
                    }
                    AssistRectangle.Width = rect.Width;
                    AssistRectangle.Height = rect.Height;
                    Canvas.SetLeft(AssistRectangle, rect.X);
                    Canvas.SetTop(AssistRectangle, rect.Y);
                }
            }
            else if (Interaction == InteractionType.Polygon)
            {
                ;
            }
            else if (Interaction == InteractionType.Rotate)
            {
                if (ShowSelectRect)
                {
                    if (AssistLine == null)
                    {
                        AssistLine = new Line() { Stroke = LineStroke, StrokeThickness = 2, IsHitTestVisible = false, X1 = DragStartedPoint.X, Y1 = DragStartedPoint.Y };
                    }
                    AssistLine.X2 = DragStartedPoint.X + offset.X;
                    AssistLine.Y2 = DragStartedPoint.Y + offset.Y;
                    //
                    if (!this.Canvas.Children.Contains(AssistLine))
                        this.Canvas.Children.Add(AssistLine);
                }
            }
        }

        /// <summary>
        /// 拖放结束
        /// </summary>
        private void DragThumb_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            if (Interaction == InteractionType.Rectangle || Interaction == InteractionType.Polygon)
            {
                if (AssistRectangle != null)
                {
                    this.Canvas.Children.Remove(AssistRectangle);
                    AssistRectangle = null;
                }
                Rect rect = CalculateEffectiveRect(DragStartedPoint, new Point(DragStartedPoint.X + e.HorizontalChange, DragStartedPoint.Y + e.VerticalChange));
                SelectDragCompleted?.Invoke(this, rect);
            }
            else if (Interaction == InteractionType.Rotate)
            {
                if (AssistLine != null)
                {
                    this.Canvas.Children.Remove(AssistLine);
                    AssistLine = null;
                }

                double hu = Math.Atan2(e.VerticalChange, e.HorizontalChange);
                double ag = hu * 180 / Math.PI;
                SelectAngleCompleted?.Invoke(this, (int)ag);
            }
            e.Handled = true;
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
            double x1 = Math.Min(rect.Left, LayerGrid.ActualWidth - 1);
            x1 = Math.Max(x1, 0);

            double y1 = Math.Min(rect.Top, LayerGrid.ActualHeight - 1);
            y1 = Math.Max(y1, 0);

            double x2 = Math.Min(rect.Right, LayerGrid.ActualWidth - 1);
            x2 = Math.Max(x2, 0);

            double y2 = Math.Min(rect.Bottom, LayerGrid.ActualHeight - 1);
            y2 = Math.Max(y2, 0);

            return new Rect(new Point(x1, y1), new Point(x2, y2));
        }


        /// <summary>
        /// 拖放中对齐线的显示
        /// </summary>
        private void DragThumb_MouseMove(object sender, MouseEventArgs e)
        {
            switch (Interaction)
            {
                case InteractionType.Rectangle:
                    if (e.LeftButton == MouseButtonState.Released)
                    {
                        HorizontalLine.Visibility = Visibility.Visible;
                        VerticallLine.Visibility = Visibility.Visible;

                        Point endPoint = e.GetPosition(LayerGrid);
                        HorizontalLine.Margin = new Thickness(0, endPoint.Y, 0, 0);
                        VerticallLine.Margin = new Thickness(endPoint.X, 0, 0, 0);
                    }
                    else
                    {
                        HorizontalLine.Visibility = Visibility.Collapsed;
                        VerticallLine.Visibility = Visibility.Collapsed;
                    }
                    break;
                case InteractionType.Polygon:
                    SelectMouseMove?.Invoke(this, e.GetPosition(LayerGrid));
                    break;
                default:
                    break;
            }
        }

        private async void ProgressControl_RefreshButtonClick(ImageStateControl obj)
        {
            await DecodeUriSourceAsync();
        }
    }
}
