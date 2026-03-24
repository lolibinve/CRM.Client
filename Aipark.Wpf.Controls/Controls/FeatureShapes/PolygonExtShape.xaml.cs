using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

namespace Aipark.Wpf.Controls
{
    /// <summary>
    /// 顶点信息可编辑目标后台代码
    /// </summary>
    public partial class PolygonExtShape : BaseShape
    {
        public PolygonExtShape(ShapeInfo objectInfo) : base(objectInfo)
        {
            InitializeComponent();

            this.ShapeInfo.PropertyChanged += ObjectInfo_PropertyChanged;
            this.ShapeInfo.Vertexs.CollectionChanged += Points_CollectionChanged;
            this.DataContext = this.ShapeInfo;
        }

        private void ObjectInfo_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            //标签变化时更改颜色
            if (e.PropertyName.Equals("Name"))
            {
                AdaptPathStrokeByTag(this.ShapeInfo.Tag);
            }
            //选中变色置层级靠上
            else if (e.PropertyName.Equals("IsChecked"))
            {
                if (this.ShapeInfo.VertexsReady)
                {
                    if (this.ShapeInfo.IsChecked)
                    {
                        this.bgRectangle.Fill = this.bgRectangle.Fill = new SolidColorBrush(Colors.Blue);
                        Panel.SetZIndex(this, 3);
                    }
                    else
                    {
                        this.bgRectangle.Fill = this.bgRectangle.Fill = new SolidColorBrush(Colors.Transparent);
                        Panel.SetZIndex(this, 2);
                    }
                }
            }
            //显示点距值
            else if (e.PropertyName.Equals("ShowScale"))
            {
                foreach (var line in Lines)
                {
                    line.SetScaleVisibility(this.ShapeInfo.ScaleVisibility);
                }
            }
        }

        public void AdaptPathStrokeByTag(string tag)
        {
            SolidColorBrush stroke = new SolidColorBrush(Colors.Chartreuse);
            switch (tag)
            {
                case "occupancy":
                    stroke = new SolidColorBrush(Colors.Red);
                    break;
                case "shelter":
                    stroke = new SolidColorBrush(Colors.Blue);
                    break;
                case "empty":
                default:
                    break;
            }
            foreach (var line in Lines)
            {
                line.SetColor(stroke);
            }
        }


        private void BaseShape_Loaded(object sender, RoutedEventArgs e)
        {
            if (this.Parent is Canvas canvas)
            {
                this.parent = canvas;
                this.parent.SizeChanged += Parent_SizeChanged;
                for (int i = 0; i < this.ShapeInfo.Vertexs.Count; i++)
                {
                    GenerateThumbAndLine(i);
                }
                Parent_SizeChanged(null, null);
            }
            AdaptPathStrokeByTag(this.ShapeInfo.Tag);
        }

        /// <summary>
        /// 只处理点数增加事件
        /// </summary>
        private void Points_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                GenerateThumbAndLine(this.ShapeInfo.Vertexs.Count - 1);
            }
        }

        /// <summary>
        /// 生成形状
        /// </summary>
        /// <param name="index"></param>
        /// <param name="total"></param>
        /// <param name="isNew">最后一点为人工新增</param>
        private void GenerateThumbAndLine(int index)
        {
            Vertex vertex = this.ShapeInfo.Vertexs[index];

            vertex.Properties = vertex.Properties ?? new Dictionary<string, string>();
            if (!vertex.Properties.ContainsKey("sn"))
            {
                if (index == 0)
                {
                    vertex.Properties.Add("sn", (index + 1).ToString());
                }
                else
                {
                    vertex.Properties.Add("sn", (int.Parse(this.ShapeInfo.Vertexs[index - 1].Properties["sn"]) + 1).ToString());
                }
            }
            if (!vertex.Properties.ContainsKey("vis"))
            {
                vertex.Properties.Add("vis", "1");
            }
            if (!vertex.Properties.ContainsKey("rel"))
            {
                vertex.Properties.Add("rel", "1");
            }

            Thumb thumb = new Thumb();
            thumb.DragDelta += resizeThumb_DragDelta;
            thumb.Style = vertex.Properties["rel"].Equals("1") ? this.FindResource("CornerThumbStyle") as Style : this.FindResource("VirtualCornerThumbStyle") as Style;
            thumb.Tag = vertex.Properties["sn"];
            thumb.DataContext = vertex;
            //首点可执行闭合操作，其它点为编辑操作，闭合后首点也执行编辑操作
            thumb.RenderTransformOrigin = new Point(0.5, 0.5);
            thumb.RenderTransform = new ScaleTransform(1, 1);
            if (index == 0 && !this.ShapeInfo.VertexsReady)
            {
                thumb.MouseEnter += Thumb_MouseEnter;
                thumb.MouseLeave += Thumb_MouseLeave;
            }
            thumb.MouseDoubleClick += Thumb_MouseDoubleClick;

            //点显示的层级高于线
            Panel.SetZIndex(thumb, 99);
            Canvas.SetLeft(thumb, vertex.X * this.parent.ActualWidth - 7);
            Canvas.SetTop(thumb, vertex.Y * this.parent.ActualHeight - 7);
            this.rootCanvas.Children.Add(thumb);
            this.Thumbs.Add(thumb);

            //使用点位默认值
            //if (!this.objectInfo.IsClosed)
            //{
            //    MessageBoxResult result = VertexPropertyEditor.Show(this.objectInfo, vertex);
            //    if (result == MessageBoxResult.OK)
            //    {
            //        thumb.Tag = vertex.Properties["sn"];
            //        lines.Where(x => x.Vertices.Contains(vertex)).ToList().ForEach(l => l.Refresh());

            //        if (vertex.Properties["rel"].Equals("0"))
            //        {
            //            thumb.Style = this.FindResource("VirtualCornerThumbStyle") as Style;
            //        }
            //    }
            //}

            if (index > 0)
            {
                //从第二点开始连线
                ScaleLine connectLine = new ScaleLine(new List<Vertex>() { this.ShapeInfo.Vertexs[index - 1], vertex }, this.rootCanvas, this.ShapeInfo.ScaleVisibility);
                Panel.SetZIndex(connectLine, 2);
                this.rootCanvas.Children.Add(connectLine);
                this.Lines.Add(connectLine);

                //最后一点首尾相接
                if (index == this.ShapeInfo.Vertexs.Count - 1 && this.ShapeInfo.VertexsReady)
                {
                    ScaleLine closeLine = new ScaleLine(new List<Vertex>() { vertex, this.ShapeInfo.Vertexs[0] }, this.rootCanvas, this.ShapeInfo.ScaleVisibility);
                    Panel.SetZIndex(closeLine, 2);
                    this.rootCanvas.Children.Add(closeLine);
                    this.Lines.Add(closeLine);
                }
            }

            if (index == this.ShapeInfo.Vertexs.Count - 1 && this.ShapeInfo.VertexsReady)
            {
                RefreshBackgroundRectangle();
            }
        }


        /// <summary>
        /// 刷新背景矩形裁剪
        /// </summary>
        private void RefreshBackgroundRectangle()
        {
            if (this.ShapeInfo.VertexsReady)
            {
                PathGeometry geometry = new PathGeometry();
                PathFigure pathFigure = new PathFigure()
                {
                    StartPoint = new Point() { X = this.ShapeInfo.Vertexs[0].X * this.parent.ActualWidth, Y = this.ShapeInfo.Vertexs[0].Y * this.parent.ActualHeight },
                    IsClosed = true
                };
                for (int i = 1; i < this.ShapeInfo.Vertexs.Count; i++)
                {
                    pathFigure.Segments.Add(new LineSegment() { Point = new Point() { X = this.ShapeInfo.Vertexs[i].X * this.parent.ActualWidth, Y = this.ShapeInfo.Vertexs[i].Y * this.parent.ActualHeight } });
                }
                geometry.Figures.Add(pathFigure);
                this.bgRectangle.Clip = geometry;
                this.bgRectangle.IsHitTestVisible = true;
            }
        }

        /// <summary>
        /// 父容器尺寸改变事件
        /// </summary>
        private void Parent_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            this.Width = this.parent.ActualWidth;
            this.Height = this.parent.ActualHeight;

            //点位修正
            foreach (Thumb thumb in this.Thumbs)
            {
                Vertex point = thumb.DataContext as Vertex;
                Canvas.SetLeft(thumb, point.X * this.parent.ActualWidth - 7);
                Canvas.SetTop(thumb, point.Y * this.parent.ActualHeight - 7);
            }
            //线自动修正
            //背景层修正
            RefreshBackgroundRectangle();
        }

        private void Thumb_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Thumb thumb = sender as Thumb;
            if (thumb.DataContext is Vertex vertex)
            {
                int index = this.ShapeInfo.Vertexs.IndexOf(vertex);
                if (index == 0 && !this.ShapeInfo.VertexsReady)
                {
                    ScaleLine line = new ScaleLine(new List<Vertex>() { this.ShapeInfo.Vertexs[this.ShapeInfo.Vertexs.Count - 1], vertex }, this.rootCanvas, this.ShapeInfo.ScaleVisibility);
                    Panel.SetZIndex(line, 2);
                    this.rootCanvas.Children.Add(line);
                    this.Lines.Add(line);

                    thumb.MouseEnter -= Thumb_MouseEnter;
                    this.ShapeInfo.VertexsReady = true;
                    this.suggestLine.Visibility = Visibility.Collapsed;

                    RefreshBackgroundRectangle();

                    base.OnPolygonCompleted(this);
                }
                else
                {
                    if (!this.ShapeInfo.IsReadOnly)
                    {
                        MessageBoxResult result = VertexPropertyEditor.Show(this.ShapeInfo, vertex);
                        if (result == MessageBoxResult.OK)
                        {
                            thumb.Tag = vertex.Properties["sn"];
                            Lines.Where(x => x.Vertices.Contains(vertex)).ToList().ForEach(l => l.Refresh());
                            thumb.Style = vertex.Properties["rel"].Equals("0") ? this.FindResource("VirtualCornerThumbStyle") as Style : this.FindResource("CornerThumbStyle") as Style;
                        }
                    }
                }
            }
        }
        private void Thumb_MouseLeave(object sender, MouseEventArgs e)
        {
            if (sender is Thumb thumb)
            {
                ScaleTransform transform = thumb.RenderTransform as ScaleTransform;
                transform.ScaleX = 1;
                transform.ScaleY = 1;
                if (this.ShapeInfo.VertexsReady)
                {
                    thumb.MouseLeave -= Thumb_MouseLeave;
                }
            }
        }
        private void Thumb_MouseEnter(object sender, MouseEventArgs e)
        {
            if (sender is Thumb thumb)
            {
                ScaleTransform transform = thumb.RenderTransform as ScaleTransform;
                transform.ScaleX = 1.5;
                transform.ScaleY = 1.5;
            }
        }

        /// <summary>
        /// 标定中撤销一个点
        /// </summary>
        public override void CancelLastPoint()
        {
            if (!this.ShapeInfo.VertexsReady)
            {
                int count = this.ShapeInfo.Vertexs.Count;
                Vertex last = this.ShapeInfo.Vertexs[count - 1];

                Thumb thumb = this.Thumbs.FirstOrDefault(x => x.DataContext is Vertex v && v == last);
                if (thumb != null)
                {
                    this.rootCanvas.Children.Remove(thumb);
                }

                ScaleLine line = Lines.FirstOrDefault(x => x.Vertices.IndexOf(last) == 1);
                if (line != null)
                {
                    this.rootCanvas.Children.Remove(line);
                }

                if (count > 1)
                {
                    //重置虚连线
                    Vertex secondlast = this.ShapeInfo.Vertexs[count - 2];
                    this.suggestLine.X1 = secondlast.X * this.parent.ActualWidth;
                    this.suggestLine.Y1 = secondlast.Y * this.parent.ActualHeight;
                }
                else
                {
                    this.suggestLine.Visibility = Visibility.Collapsed;
                }

                this.ShapeInfo.Vertexs.Remove(last);
            }
            base.OnPolygonVertexCanceled(this);
        }

        /// <summary>
        /// 多边形最后点位建议位置效果
        /// </summary>
        public override void TestLastPoint(Point point)
        {
            if (!this.ShapeInfo.VertexsReady && this.ShapeInfo.Vertexs.Count > 0)
            {
                this.suggestLine.Visibility = Visibility.Visible;
                this.suggestLine.X1 = this.ShapeInfo.Vertexs[this.ShapeInfo.Vertexs.Count - 1].X * this.parent.ActualWidth;
                this.suggestLine.Y1 = this.ShapeInfo.Vertexs[this.ShapeInfo.Vertexs.Count - 1].Y * this.parent.ActualHeight;
                this.suggestLine.X2 = point.X;
                this.suggestLine.Y2 = point.Y;
            }
            else
            {
                this.suggestLine.Visibility = Visibility.Collapsed;
            }
        }

        /// <summary>
        /// 拖拽调整顶点位置
        /// </summary>
        private void resizeThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            if (this.ShapeInfo.IsReadOnly)
                return;

            Thumb thumb = sender as Thumb;
            if (thumb.DataContext is Vertex vertex)
            {
                //水平方向不出右边界
                bool horAccept = true;
                if (e.HorizontalChange >= 0)
                {
                    if (vertex.X * this.parent.ActualWidth + e.HorizontalChange > this.parent.ActualWidth)
                        horAccept = false;
                }
                else
                {
                    if (vertex.X * this.parent.ActualWidth + e.HorizontalChange < 0)
                        horAccept = false;
                }

                //垂直方向不出下边界
                bool verAccept = true;
                if (e.VerticalChange >= 0)
                {
                    if (vertex.Y * this.parent.ActualHeight + e.VerticalChange > this.parent.ActualHeight)
                        verAccept = false;
                }
                else
                {
                    if (vertex.Y * this.parent.ActualHeight + e.VerticalChange < 0)
                        verAccept = false;
                }

                if (horAccept)
                {
                    vertex.X += e.HorizontalChange / this.parent.ActualWidth;
                }

                if (verAccept)
                {
                    vertex.Y += e.VerticalChange / this.parent.ActualHeight;
                }

                //刷新调整后的大小
                if (horAccept || verAccept)
                {
                    Canvas.SetLeft(thumb, vertex.X * this.parent.ActualWidth - 7);
                    Canvas.SetTop(thumb, vertex.Y * this.parent.ActualHeight - 7);

                    foreach (var line in this.Lines)
                    {
                        if (line.Vertices.Contains(vertex))
                        {
                            line.Refresh();
                        }
                    }

                    RefreshBackgroundRectangle();
                }
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

        private void BgRectangle_MouseEnter(object sender, MouseEventArgs e)
        {
            this.bgRectangle.Fill = new SolidColorBrush(Colors.Blue);
        }
        private void BgRectangle_MouseLeave(object sender, MouseEventArgs e)
        {
            this.bgRectangle.Fill = new SolidColorBrush(Colors.Transparent);
        }
        private void BgRectangle_PreviewMouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (this.ShapeInfo.IsReadOnly)
                e.Handled = true;
        }
        private void BgRectangle_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            base.OnChecked(this, this.ShapeInfo);
        }
    }
}
