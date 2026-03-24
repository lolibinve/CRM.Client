using System;
using System.Collections.Generic;
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

namespace Aipark.Wpf.Controls.SuperImage
{
    /// <summary>
    /// PartialZoomControl.xaml 的交互逻辑
    /// </summary>
    public partial class PartialZoomControl : UserControl
    {
        public PartialZoomControl()
        {
            InitializeComponent();

            SetRadius();
        }

        /// <summary>
        /// 
        /// </summary>
        public BitmapImage BitmapImage
        {
            get { return (BitmapImage)GetValue(BitmapImageProperty); }
            set { SetValue(BitmapImageProperty, value); }
        }
        public static readonly DependencyProperty BitmapImageProperty =
            DependencyProperty.Register(nameof(BitmapImage), typeof(BitmapImage), typeof(PartialZoomControl), new PropertyMetadata(null, OnBitmapImagePropertyChanged));
        public static void OnBitmapImagePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is PartialZoomControl control)
            {
                control.RefreshImage();
            }
        }

        private void RefreshImage()
        {
            this.zoomImage.Source = this.BitmapImage;
            if (this.BitmapImage != null)
            {
                this.zoomImage.Width = this.BitmapImage.PixelWidth * this.ZoomFactor;
                this.zoomImage.Height = this.BitmapImage.PixelHeight * this.ZoomFactor;
            }
        }

        /// <summary>
        /// 放大系数
        /// </summary>
        public double ZoomFactor
        {
            get { return (double)GetValue(ZoomFactorProperty); }
            set { SetValue(ZoomFactorProperty, value); }
        }
        public static readonly DependencyProperty ZoomFactorProperty =
            DependencyProperty.Register(nameof(ZoomFactor), typeof(double), typeof(PartialZoomControl), new PropertyMetadata(1.0, OnZoomFactorPropertyChanged));
        public static void OnZoomFactorPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is PartialZoomControl control)
            {
                control.RefreshImage();
                control.RefreshZoomCenter();
            }
        }

        /// <summary>
        /// 归一化中心点坐标
        /// </summary>
        public Point NormalizedCenter
        {
            get { return (Point)GetValue(NormalizedCenterProperty); }
            set { SetValue(NormalizedCenterProperty, value); }
        }
        public static readonly DependencyProperty NormalizedCenterProperty =
            DependencyProperty.Register(nameof(NormalizedCenter), typeof(Point), typeof(PartialZoomControl), new PropertyMetadata(new Point(0.0, 0.0), OnNormalizedCenterPropertyChanged));
        public static void OnNormalizedCenterPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is PartialZoomControl control)
            {
                control.RefreshZoomCenter();
            }
        }

        private void RefreshZoomCenter()
        {
            if (this.zoomImage.Source == null)
                return;

            double left = -this.zoomImage.Width * NormalizedCenter.X + this.Radius;
            double top = -this.zoomImage.Height * NormalizedCenter.Y + this.Radius;
            this.zoomImage.Margin = new Thickness(left, top, 0, 0);
        }

        /// <summary>
        /// 放大区域半径
        /// </summary>
        public double Radius
        {
            get { return (double)GetValue(RadiusProperty); }
            set { SetValue(RadiusProperty, value); }
        }
        public static readonly DependencyProperty RadiusProperty =
            DependencyProperty.Register(nameof(Radius), typeof(double), typeof(PartialZoomControl), new PropertyMetadata(100.0, OnRadiusPropertyChanged));
        public static void OnRadiusPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is PartialZoomControl control)
            {
                control.SetRadius();
            }
        }

        private void SetRadius()
        {
            this.Width = this.Radius * 2;
            this.Height = this.Radius * 2;
            this.borderEllipse.Width = this.Radius * 2;
            this.borderEllipse.Height = this.Radius * 2;

            this.clipEllipse.Center = new Point() { X = this.Radius, Y = this.Radius };
            this.clipEllipse.RadiusX = this.Radius;
            this.clipEllipse.RadiusY = this.Radius;
        }

    }

    public enum PartialPlacement
    {
        MousePoint = 0,


    }

}
