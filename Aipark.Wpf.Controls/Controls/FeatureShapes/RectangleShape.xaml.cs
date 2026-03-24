using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Aipark.Wpf.Controls
{
    /// <summary>
    /// RectangleShape.xaml 的交互逻辑
    /// </summary>
    public partial class RectangleShape : BaseShape
    {
        public RectangleShape(ShapeInfo objectInfo) : base(objectInfo)
        {
            InitializeComponent();

            this.ShapeInfo.PropertyChanged += ObjectInfo_PropertyChanged;
            this.DataContext = this.ShapeInfo;
        }

        private void BaseShape_Loaded(object sender, RoutedEventArgs e)
        {
            if (this.Parent is Canvas canvas)
            {
                this.parent = canvas;
                canvas.SizeChanged += Parent_SizeChanged;
                Parent_SizeChanged(null, null);
            }
        }

        private void BaseShape_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(this.ShapeInfo.Tooltip))
            {
                RedrawTooltip();
            }
        }

        private void ObjectInfo_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals("Tooltip"))
            {
                if (!string.IsNullOrWhiteSpace(this.ShapeInfo.Tooltip))
                {
                    tooltipGrid.Visibility = Visibility.Visible;
                    RedrawTooltip();
                }
                else
                {
                    tooltipGrid.Visibility = Visibility.Collapsed;
                }
            }
            //选中变色置层级靠上
            else if (e.PropertyName.Equals("IsChecked"))
            {
                if (this.ShapeInfo.IsChecked)
                {
                    backgroundRectangle.Visibility = Visibility.Visible;
                    Panel.SetZIndex(this, 3);
                }
                else
                {
                    backgroundRectangle.Visibility = Visibility.Collapsed;
                    Panel.SetZIndex(this, 2);
                }
            }
        }


        /// <summary>
        /// 重绘提示信息
        /// </summary>
        private void RedrawTooltip()
        {
            string signature = this.ShapeInfo.Tooltip;

            using (Graphics graphics = Graphics.FromHwnd(new WindowInteropHelper(Application.Current.MainWindow).Handle))
            {
                for (int i = 48; i > 8; i--)
                {
                    Font f = new Font("SimHei", i, System.Drawing.FontStyle.Regular);

                    float fontWidth = graphics.MeasureString(signature, f).Width;
                    float fontHeight = graphics.MeasureString(signature, f).Height;

                    if (fontWidth <= this.ActualWidth)
                    {
                        int voffset = i / 8 + 1;

                        this.tooltipGrid.Margin = new Thickness(0, -(i + voffset), 0, 0);
                        this.tooltipTextBlock.FontSize = i;
                        return;
                    }
                }
            }

            this.tooltipGrid.Margin = new Thickness(0, -10, 0, 0);
            this.tooltipTextBlock.FontSize = 8;
        }


        /// <summary>
        /// 父容器尺寸改变事件
        /// </summary>
        private void Parent_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (this.Parent is Canvas canvas)
            {
                this.Width = (this.ShapeInfo.Vertexs[1].X - this.ShapeInfo.Vertexs[0].X) * canvas.ActualWidth;
                this.Height = (this.ShapeInfo.Vertexs[1].Y - this.ShapeInfo.Vertexs[0].Y) * canvas.ActualHeight;
                Canvas.SetLeft(this, this.ShapeInfo.Vertexs[0].X * canvas.ActualWidth);
                Canvas.SetTop(this, this.ShapeInfo.Vertexs[0].Y * canvas.ActualHeight);
            }
        }

        /// <summary>
        /// 右键菜单点击事件
        /// </summary>
        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            string cmd = (sender as MenuItem).Header.ToString();
            base.OnMenuItemClick(this, cmd);
        }


        #region 拖动

        private void RootGrid_MouseEnter(object sender, MouseEventArgs e)
        {
            backgroundRectangle.Visibility = Visibility.Visible;
            ltThumb.Visibility = Visibility.Visible;
            rtThumb.Visibility = Visibility.Visible;
            rbThumb.Visibility = Visibility.Visible;
            lbThumb.Visibility = Visibility.Visible;
        }
        private void RootGrid_MouseLeave(object sender, MouseEventArgs e)
        {
            backgroundRectangle.Visibility = Visibility.Collapsed;
            ltThumb.Visibility = Visibility.Collapsed;
            rtThumb.Visibility = Visibility.Collapsed;
            rbThumb.Visibility = Visibility.Collapsed;
            lbThumb.Visibility = Visibility.Collapsed;
        }
        private void RootGrid_PreviewMouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (this.ShapeInfo.IsReadOnly)
                e.Handled = true;
        }

        /// <summary>
        /// 拖动以移动
        /// </summary>
        private void DragThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            if (this.ShapeInfo.IsReadOnly)
                return;

            //左
            double left = Canvas.GetLeft(this) + e.HorizontalChange;
            left = Math.Min(left, this.parent.ActualWidth - this.ActualWidth - 1);
            left = Math.Max(left, 0);

            //上
            double top = Canvas.GetTop(this) + e.VerticalChange;
            top = Math.Min(top, this.parent.ActualHeight - this.ActualHeight - 1);
            top = Math.Max(top, 0);

            Canvas.SetLeft(this, left);
            Canvas.SetTop(this, top);

            this.ShapeInfo.Vertexs[0] = new Vertex(Canvas.GetLeft(this), Canvas.GetTop(this), this.parent.ActualWidth, this.parent.ActualHeight);
            this.ShapeInfo.Vertexs[1] = new Vertex(Canvas.GetLeft(this) + this.Width, Canvas.GetTop(this) + this.Height, this.parent.ActualWidth, this.parent.ActualHeight);
        }
        /// <summary>
        /// 点击选中
        /// </summary>
        private void DragThumb_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            base.OnChecked(this, this.ShapeInfo);
        }

        #endregion


        /// <summary>
        /// 拖动以调整尺寸
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ResizeThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            if (this.ShapeInfo.IsReadOnly)
                return;

            double height, top, width, left;

            //计算竖直方向的变动
            switch ((sender as Thumb).VerticalAlignment)
            {
                case VerticalAlignment.Bottom:
                    height = this.Height + e.VerticalChange;
                    height = Math.Min(height, this.parent.ActualHeight - Canvas.GetTop(this) - 1);
                    height = Math.Max(height, this.MinHeight);
                    this.Height = height;
                    break;
                case VerticalAlignment.Top:
                    top = Canvas.GetTop(this) + e.VerticalChange;
                    height = this.Height - e.VerticalChange;

                    if (top < 0)
                    {
                        height = this.Height - e.VerticalChange + top;
                        top = 0;
                    }
                    if (height < this.MinHeight)
                    {
                        top = top + height - this.MinHeight;
                        height = this.MinHeight;
                    }
                    Canvas.SetTop(this, top);
                    this.Height = height;
                    break;
                default:
                    break;
            }

            //计算水平方向的变动
            switch ((sender as Thumb).HorizontalAlignment)
            {
                case HorizontalAlignment.Right:
                    width = this.Width + e.HorizontalChange;
                    width = Math.Min(width, this.parent.ActualWidth - Canvas.GetLeft(this) - 1);
                    width = Math.Max(width, this.MinWidth);
                    this.Width = width;
                    break;
                case HorizontalAlignment.Left:
                    left = Canvas.GetLeft(this) + e.HorizontalChange;
                    width = this.Width - e.HorizontalChange;

                    if (left < 0)
                    {
                        width = this.Width - e.HorizontalChange + left;
                        left = 0;
                    }
                    if (width < this.MinWidth)
                    {
                        left = left + width - this.MinWidth;
                        width = this.MinWidth;
                    }
                    Canvas.SetLeft(this, left);
                    this.Width = width;
                    break;
                default:
                    break;
            }

            this.ShapeInfo.Vertexs[0] = new Vertex(Canvas.GetLeft(this), Canvas.GetTop(this), this.parent.ActualWidth, this.parent.ActualHeight);
            this.ShapeInfo.Vertexs[1] = new Vertex(Canvas.GetLeft(this) + this.Width, Canvas.GetTop(this) + this.Height, this.parent.ActualWidth, this.parent.ActualHeight);
        }

    }
}
