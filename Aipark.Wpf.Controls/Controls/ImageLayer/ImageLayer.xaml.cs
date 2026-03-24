using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Aipark.Wpf.Controls
{
    /// <summary>
    /// ImageLayer.xaml 的交互逻辑
    /// </summary>
    public partial class ImageLayer : UserControl
    {
        /// <summary>
        /// 底图原始大小
        /// </summary>
        public Size ImageSize { get; private set; } = new Size(0, 0);
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
        private Point startPoint = new Point(0, 0);
        /// <summary>
        /// 框选的矩形区
        /// </summary>
        private Rectangle selectRectangle = null;
        /// <summary>
        /// 鼠标拖放连线
        /// </summary>
        private Line directionLine = null;


        public ImageLayer()
        {
            InitializeComponent();

            Cursor = Cursors.Hand;
        }

        //private void UserControl_Loaded(object sender, RoutedEventArgs e)
        //{
        //    LoadLayer();
        //}

        /// <summary>
        /// 加载默认图层
        /// </summary>
        public void LoadLayer()
        {
            LayerImage.Source = new BitmapImage(new Uri("pack://application:,,,/Aipark.Wpf.Controls;component/Resources/Images/alice_blue.jpg", UriKind.Absolute));
        }

        /// <summary>
        /// 从字节数组加载图片
        /// </summary>
        /// <param name="bytes"></param>
        public void LoadLayer(byte[] bytes)
        {
            Size size = Size.Empty;
            LoadLayer(bytes, out size);
        }

        /// <summary>
        /// 从字节数组加载图片
        /// </summary>
        /// <param name="bytes"></param>
        public void LoadLayer(byte[] bytes, out Size size)
        {
            using (var ms = new MemoryStream(bytes))
            {
                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.StreamSource = ms;
                bitmap.EndInit();
                bitmap.Freeze();

                size = new Size(bitmap.PixelWidth, bitmap.PixelHeight);
                LoadLayer(bitmap as BitmapImage);
            }
        }

        /// <summary>
        /// 加载底图(Uniform缩小或最佳比例放大)
        /// </summary>
        /// <param name="bmp"></param>
        public void LoadLayer(BitmapImage bmp)
        {
            LayerImage.Source = bmp;
            ImageSize = new Size(bmp.PixelWidth, bmp.PixelHeight);

            CalculateUniformZoomScale();
        }

        /// <summary>
        /// 计算最佳显示效果的自适应比例
        /// </summary>
        private void CalculateUniformZoomScale()
        {
            if (ImageSize.Width == 0 || ImageSize.Height == 0)
                return;

            /* 按图片和容器大小的比例自适应显示
             *     图片尺寸大于容器：图片比例特别大的，最大缩放倍数适当调小；
             *                       图片比例普通的，最大缩放倍数适当调大；
             *     图片尺寸小于容器：图片极小时，长宽达不到容器的长宽的1/8，最大允许放大到容器的1/2显示；
             *                       图片较小时，长宽达不到容器的长宽的1/4，最大允许放大到容器的3/4显示；
             *                       图片普通时，可适当允许最大倍数略大；
             */

            double xZoom = RootGrid.ActualWidth / ImageSize.Width;
            double yZoom = RootGrid.ActualHeight / ImageSize.Height;
            ZoomScale = Math.Min(xZoom, yZoom);

            if (ImageSize.Width >= RootGrid.ActualWidth || ImageSize.Height >= RootGrid.ActualHeight)
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

            LayerImage.Width = ImageSize.Width * ZoomScale;
            LayerImage.Height = ImageSize.Height * ZoomScale;

            double left = (RootGrid.ActualWidth - LayerImage.Width) / 2;
            double top = (RootGrid.ActualHeight - LayerImage.Height) / 2;
            ContentGrid.Margin = new Thickness(left, top, 0, 0);

            PlayZoomScaleStory();
            ZoomDeltaChanged?.Invoke(ZoomScale, ZoomScale);
        }

        /// <summary>
        /// 播放缩放比例显示动画
        /// </summary>
        private void PlayZoomScaleStory()
        {
            if (ShowZoomScale)
            {
                msgTextBlock.Text = Math.Round(ZoomScale * 100) + "%";
                Storyboard storyboard = FindResource("MsgShowStory") as Storyboard;
                storyboard.Begin();
            }
        }

        #region 地图缩放与移动

        /// <summary>
        /// 基于鼠标中心点的缩放控制
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ContentGrid_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (Interaction != InteractionType.Browse)
                return;

            Point offsetRoot = e.GetPosition(RootGrid);

            double preDelta = ZoomScale;

            ZoomScale += e.Delta / 600.0;
            ZoomScale = Math.Max(ZoomScale, MinScale);
            ZoomScale = Math.Min(ZoomScale, MaxScale);

            //新旧比例系数
            var deltaScaleFactor = ZoomScale / preDelta;

            double left = offsetRoot.X - (offsetRoot.X - ContentGrid.Margin.Left) * deltaScaleFactor;
            double top = offsetRoot.Y - (offsetRoot.Y - ContentGrid.Margin.Top) * deltaScaleFactor;

            double targetWidth = ImageSize.Width * ZoomScale;
            double targetHeight = ImageSize.Height * ZoomScale;

            //左
            if (targetWidth <= RootGrid.ActualWidth)
            {
                left = (RootGrid.ActualWidth - targetWidth) / 2;
            }
            else
            {
                //
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
                //
                top = Math.Min(top, 0);
                top = Math.Max(RootGrid.ActualHeight - targetHeight, top);
            }

            //设置大小
            LayerImage.Width = targetWidth;
            LayerImage.Height = targetHeight;
            //设置外边距
            ContentGrid.Margin = new Thickness(left, top, 0, 0);

            PlayZoomScaleStory();
            ZoomDeltaChanged?.Invoke(preDelta, ZoomScale);

            e.Handled = true;
        }

        #endregion


        private void RootGrid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            CalculateUniformZoomScale();
        }

        private void PaintCanvas_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            CanvasSizeChanged?.Invoke(sender, e);
        }


        #region 外部事件和方法调用

        /// <summary>
        /// 外部调用Canvas
        /// </summary>
        /// <returns></returns>
        public Canvas Canvas => this.PaintCanvas;
        /// <summary>
        /// 外部调用Canvas尺寸改变事件
        /// </summary>
        public event Action<object, SizeChangedEventArgs> CanvasSizeChanged;
        /// <summary>
        /// 矩形框或点位选取完成事件
        /// </summary>
        public event Action<ImageLayer, Rect> SelectDragCompleted;
        /// <summary>
        /// 编辑模式下设置旋转角度完成
        /// </summary>
        public event Action<ImageLayer, int> SelectAngleCompleted;
        /// <summary>
        /// 底图缩放比例改变事件(之前比例,当前比例)
        /// </summary>
        public event Action<double, double> ZoomDeltaChanged;
        /// <summary>
        /// 编辑模式下鼠标移动事件
        /// </summary>
        public event Action<ImageLayer, Point> SelectMouseMove;

        /// <summary>
        /// 重置底图显示
        /// </summary>
        public void ResetScale()
        {
            CalculateUniformZoomScale();
        }

        #endregion



        #region 附加属性

        /// <summary>
        /// 显示缩放比例
        /// </summary>
        public bool ShowZoomScale
        {
            get { return (bool)GetValue(ShowZoomScaleProperty); }
            set { SetValue(ShowZoomScaleProperty, value); }
        }
        public static readonly DependencyProperty ShowZoomScaleProperty =
            DependencyProperty.Register("ShowZoomScale", typeof(bool), typeof(ImageLayer), new PropertyMetadata(true, null));


        /// <summary>
        /// 显示框选矩形框
        /// </summary>
        public bool ShowSelectRect
        {
            get { return (bool)GetValue(ShowSelectRectProperty); }
            set { SetValue(ShowSelectRectProperty, value); }
        }
        public static readonly DependencyProperty ShowSelectRectProperty =
            DependencyProperty.Register("ShowSelectRect", typeof(bool), typeof(ImageLayer), new PropertyMetadata(true, null));


        /// <summary>
        /// 控件交互模式
        /// </summary>
        public InteractionType Interaction
        {
            get { return (InteractionType)GetValue(InteractionProperty); }
            set { SetValue(InteractionProperty, value); }
        }
        public static readonly DependencyProperty InteractionProperty =
            DependencyProperty.Register("Interaction", typeof(InteractionType), typeof(ImageLayer), new PropertyMetadata(InteractionType.Browse, InteractionPropertyChangedCallback));
        public static void InteractionPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ImageLayer layer = d as ImageLayer;

            switch ((InteractionType)e.NewValue)
            {
                case InteractionType.Browse:
                    layer.Cursor = Cursors.Hand;
                    layer.alignmentGrid.Visibility = Visibility.Collapsed;
                    break;
                case InteractionType.Rectangle:
                    layer.Cursor = Cursors.Arrow;
                    layer.alignmentGrid.Visibility = Visibility.Visible;
                    //设置对齐线
                    Point mousePoint = Mouse.GetPosition(layer.ContentGrid);
                    double top = Math.Min(mousePoint.Y, layer.ContentGrid.ActualHeight);
                    top = Math.Max(top, 0);
                    layer.horizonLine.Margin = new Thickness(0, top, 0, 0);
                    double left = Math.Min(mousePoint.X, layer.ContentGrid.ActualWidth);
                    left = Math.Max(left, 0);
                    layer.verticalLine.Margin = new Thickness(left, 0, 0, 0);
                    break;
                case InteractionType.Polygon:
                    layer.Cursor = Cursors.Arrow;
                    layer.alignmentGrid.Visibility = Visibility.Collapsed;
                    break;
                case InteractionType.Rotate:
                    layer.Cursor = Cursors.Arrow;
                    layer.alignmentGrid.Visibility = Visibility.Collapsed;
                    break;
            }
        }

        /// <summary>
        /// 框选矩形框边框颜色
        /// </summary>
        public SolidColorBrush SelectRectStroke
        {
            get { return (SolidColorBrush)GetValue(SelectRectStrokeProperty); }
            set { SetValue(SelectRectStrokeProperty, value); }
        }
        public static readonly DependencyProperty SelectRectStrokeProperty =
            DependencyProperty.Register("SelectRectStroke", typeof(SolidColorBrush), typeof(ImageLayer), new PropertyMetadata(new SolidColorBrush(Colors.Blue), SelectRectStrokePropertyChangedCallback));
        public static void SelectRectStrokePropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ImageLayer layer = d as ImageLayer;
            if (layer.selectRectangle != null)
                layer.selectRectangle.Stroke = (SolidColorBrush)e.NewValue;
        }

        #endregion



        /// <summary>
        /// 开始拖动
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DragThumb_DragStarted(object sender, DragStartedEventArgs e)
        {
            startPoint = new Point(e.HorizontalOffset, e.VerticalOffset);
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
                double left = ContentGrid.Margin.Left + offset.X;
                double top = ContentGrid.Margin.Top + offset.Y;
                //左
                if (LayerImage.ActualWidth <= RootGrid.ActualWidth)
                {
                    left = (RootGrid.ActualWidth - LayerImage.Width) / 2;
                }
                else
                {
                    left = Math.Min(left, 0);
                    left = Math.Max(RootGrid.ActualWidth - LayerImage.ActualWidth, left);
                }
                //上
                if (LayerImage.ActualHeight <= RootGrid.ActualHeight)
                {
                    top = (RootGrid.ActualHeight - LayerImage.Height) / 2;
                }
                else
                {
                    top = Math.Min(top, 0);
                    top = Math.Max(RootGrid.ActualHeight - LayerImage.ActualHeight, top);
                }
                //设置外边距
                ContentGrid.Margin = new Thickness(left, top, 0, 0);

                if (ShowZoomScale)
                {
                    msgTextBlock.Text = Math.Round(ZoomScale * 100) + "%";
                    Storyboard storyboard = FindResource("MsgShowStory") as Storyboard;
                    storyboard.Begin();
                }
            }
            else if (Interaction == InteractionType.Rectangle)
            {
                Rect rect = CalculateEffectiveRect(startPoint, new Point(startPoint.X + offset.X, startPoint.Y + offset.Y));
                if (ShowSelectRect)
                {
                    if (selectRectangle == null)
                    {
                        selectRectangle = new Rectangle() { Stroke = SelectRectStroke, StrokeThickness = 2, IsHitTestVisible = false };
                        PaintCanvas.Children.Add(selectRectangle);
                    }
                    selectRectangle.Width = rect.Width;
                    selectRectangle.Height = rect.Height;
                    Canvas.SetLeft(selectRectangle, rect.X);
                    Canvas.SetTop(selectRectangle, rect.Y);
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
                    if (directionLine == null)
                    {
                        directionLine = new Line() { Stroke = SelectRectStroke, StrokeThickness = 2, IsHitTestVisible = false, X1 = startPoint.X, Y1 = startPoint.Y };
                    }
                    directionLine.X2 = startPoint.X + offset.X;
                    directionLine.Y2 = startPoint.Y + offset.Y;
                    //
                    if (!PaintCanvas.Children.Contains(directionLine))
                        PaintCanvas.Children.Add(directionLine);
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
                if (selectRectangle != null)
                {
                    PaintCanvas.Children.Remove(selectRectangle);
                    selectRectangle = null;
                }
                Rect rect = CalculateEffectiveRect(startPoint, new Point(startPoint.X + e.HorizontalChange, startPoint.Y + e.VerticalChange));
                SelectDragCompleted?.Invoke(this, rect);
            }
            else if (Interaction == InteractionType.Rotate)
            {
                if (directionLine != null)
                {
                    PaintCanvas.Children.Remove(directionLine);
                    directionLine = null;
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
            double x1 = Math.Min(rect.Left, ContentGrid.ActualWidth - 1);
            x1 = Math.Max(x1, 0);

            double y1 = Math.Min(rect.Top, ContentGrid.ActualHeight - 1);
            y1 = Math.Max(y1, 0);

            double x2 = Math.Min(rect.Right, ContentGrid.ActualWidth - 1);
            x2 = Math.Max(x2, 0);

            double y2 = Math.Min(rect.Bottom, ContentGrid.ActualHeight - 1);
            y2 = Math.Max(y2, 0);

            return new Rect(new Point(x1, y1), new Point(x2, y2));
        }


        /// <summary>
        /// 拖放中对齐线的显示
        /// </summary>
        private void DragThumb_MouseMove(object sender, MouseEventArgs e)
        {
            if (Interaction == InteractionType.Rectangle && e.LeftButton == MouseButtonState.Released)
            {
                Point endPoint = e.GetPosition(ContentGrid);
                horizonLine.Margin = new Thickness(0, endPoint.Y, 0, 0);
                verticalLine.Margin = new Thickness(endPoint.X, 0, 0, 0);
                alignmentGrid.Visibility = Visibility.Visible;
            }
            else
            {
                alignmentGrid.Visibility = Visibility.Collapsed;
            }

            if (Interaction == InteractionType.Polygon)
            {
                SelectMouseMove?.Invoke(this, e.GetPosition(ContentGrid));
            }
        }
    }
}
