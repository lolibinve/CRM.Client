using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Aipark.Wpf.Controls
{
    /// <summary>
    /// ScaleLine.xaml 的交互逻辑
    /// </summary>
    public partial class ScaleLine : UserControl
    {
        /// <summary>
        /// 顶点信息
        /// </summary>
        public List<Vertex> Vertices = null;

        private Canvas canvas = null;
        private bool isSolid = true;

        public ScaleLine(List<Vertex> vertices, Canvas canvas, Visibility scaleVisibility, bool isSolid = true)
        {
            InitializeComponent();

            this.Vertices = vertices;
            this.canvas = canvas;
            this.lengthTbx.Visibility = scaleVisibility;
            this.isSolid = isSolid;
            this.canvas.SizeChanged += Canvas_SizeChanged;
        }

        private void Canvas_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Refresh();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            Refresh();
        }

        /// <summary>
        /// 更新
        /// </summary>
        public void Refresh()
        {
            double angle = GetRotateAngle();
            this.rotateTransform.Angle = angle;
            double length = GetLineLength();
            this.line.X2 = length;
            this.Width = length;
            this.lengthTbx.Text = Math.Ceiling(length).ToString();
            if ((angle > 90 && angle < 180) || (angle > -180 && angle < -90))
            {
                this.lengthRotateTransform.Angle = 180;
            }
            else
            {
                this.lengthRotateTransform.Angle = 0;
            }
            if (Vertices.Any(x => x.Properties != null && x.Properties.ContainsKey("vis") && x.Properties["vis"].Equals("0")))
            {
                this.line.StrokeDashArray = new DoubleCollection() { 4, 4 };
            }
            else
            {
                this.line.StrokeDashArray = this.isSolid ? null : new DoubleCollection() { 4, 2 };
            }
            Canvas.SetLeft(this, Vertices[0].X * canvas.ActualWidth);
            Canvas.SetTop(this, Vertices[0].Y * canvas.ActualHeight);
        }

        /// <summary>
        /// 设置刻度的可见性
        /// </summary>
        /// <param name="visibility"></param>
        public void SetScaleVisibility(Visibility visibility)
        {
            this.lengthTbx.Visibility = visibility;
        }

        /// <summary>
        /// 显示刻度值
        /// </summary>
        public void ShowScale()
        {
            this.lengthTbx.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// 隐藏刻度值
        /// </summary>
        public void HideScale()
        {
            this.lengthTbx.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// 设置线条颜色
        /// </summary>
        /// <param name="solidColorBrush"></param>
        public void SetColor(SolidColorBrush solidColorBrush)
        {
            this.line.Stroke = solidColorBrush;
        }

        private double GetRotateAngle()
        {
            /*
             * 根据以起点为原点，终点落在的象限计算与水平轴逆时针旋转至X轴正方向的角度
             */
            double x = (Vertices[1].X - Vertices[0].X) * canvas.ActualWidth;
            double y = (Vertices[1].Y - Vertices[0].Y) * canvas.ActualHeight;

            double hu = Math.Atan2(y, x);
            double ag = hu * 180 / Math.PI;

            return ag;
        }

        public double GetLineLength()
        {
            double x = Vertices[1].X - Vertices[0].X;
            double y = Vertices[1].Y - Vertices[0].Y;

            double length = Math.Sqrt(Math.Pow(x * canvas.ActualWidth, 2) + Math.Pow(y * canvas.ActualHeight, 2));
            return length;
        }


    }
}
