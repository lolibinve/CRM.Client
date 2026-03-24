using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace Aipark.Wpf.Controls
{
    /// <summary>
    /// CubeShape.xaml 的交互逻辑
    /// </summary>
    public partial class CubeShape : BaseShape
    {
        private List<ScaleLine> lines = new List<ScaleLine>();

        public event Action<Vertex> VertexChanged;

        public CubeShape(ShapeInfo shapeInfo) : base(shapeInfo)
        {
            InitializeComponent();

            this.DataContext = this.ShapeInfo;
        }


        private void ObjectInfo_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals("ScaleVisibility"))
            {
                foreach (var line in lines)
                {
                    line.SetScaleVisibility(this.ShapeInfo.ScaleVisibility);
                }
            }
            else if (e.PropertyName.Equals("ThumbVisibility"))
            {
                foreach (var thumb in Thumbs)
                {
                    thumb.Visibility = this.ShapeInfo.ScaleVisibility;
                }
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

                this.ShapeInfo.PropertyChanged += ObjectInfo_PropertyChanged;
                this.ShapeInfo.Vertexs.CollectionChanged += Points_CollectionChanged;
            }
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
            Point point = new Point(vertex.X * this.parent.ActualWidth, vertex.Y * this.parent.ActualHeight);

            Thumb thumb = new Thumb();
            thumb.Visibility = this.ShapeInfo.ThumbVisibility;
            thumb.DragDelta += resizeThumb_DragDelta;
            thumb.Style = this.FindResource("CornerThumbStyle") as Style;
            thumb.Tag = vertex.Name;
            thumb.DataContext = vertex;

            //点显示的层级高于线
            Panel.SetZIndex(thumb, 99);
            Canvas.SetLeft(thumb, point.X - 7);
            Canvas.SetTop(thumb, point.Y - 7);
            this.rootCanvas.Children.Add(thumb);
            this.Thumbs.Add(thumb);

            switch (vertex.Name)
            {
                case "1":
                    CreatVertexLine(this.ShapeInfo.Vertexs.FirstOrDefault(x => x.Name.Equals("1")), this.ShapeInfo.Vertexs.FirstOrDefault(x => x.Name.Equals("2")), false);
                    CreatVertexLine(this.ShapeInfo.Vertexs.FirstOrDefault(x => x.Name.Equals("1")), this.ShapeInfo.Vertexs.FirstOrDefault(x => x.Name.Equals("4")));
                    CreatVertexLine(this.ShapeInfo.Vertexs.FirstOrDefault(x => x.Name.Equals("1")), this.ShapeInfo.Vertexs.FirstOrDefault(x => x.Name.Equals("5")));
                    break;
                case "2":
                    CreatVertexLine(this.ShapeInfo.Vertexs.FirstOrDefault(x => x.Name.Equals("2")), this.ShapeInfo.Vertexs.FirstOrDefault(x => x.Name.Equals("1")), false);
                    CreatVertexLine(this.ShapeInfo.Vertexs.FirstOrDefault(x => x.Name.Equals("2")), this.ShapeInfo.Vertexs.FirstOrDefault(x => x.Name.Equals("3")), false);
                    CreatVertexLine(this.ShapeInfo.Vertexs.FirstOrDefault(x => x.Name.Equals("2")), this.ShapeInfo.Vertexs.FirstOrDefault(x => x.Name.Equals("6")), false);
                    break;
                case "3":
                    CreatVertexLine(this.ShapeInfo.Vertexs.FirstOrDefault(x => x.Name.Equals("3")), this.ShapeInfo.Vertexs.FirstOrDefault(x => x.Name.Equals("2")), false);
                    CreatVertexLine(this.ShapeInfo.Vertexs.FirstOrDefault(x => x.Name.Equals("3")), this.ShapeInfo.Vertexs.FirstOrDefault(x => x.Name.Equals("4")));
                    CreatVertexLine(this.ShapeInfo.Vertexs.FirstOrDefault(x => x.Name.Equals("3")), this.ShapeInfo.Vertexs.FirstOrDefault(x => x.Name.Equals("7")));
                    break;
                case "4":
                    CreatVertexLine(this.ShapeInfo.Vertexs.FirstOrDefault(x => x.Name.Equals("4")), this.ShapeInfo.Vertexs.FirstOrDefault(x => x.Name.Equals("3")));
                    CreatVertexLine(this.ShapeInfo.Vertexs.FirstOrDefault(x => x.Name.Equals("4")), this.ShapeInfo.Vertexs.FirstOrDefault(x => x.Name.Equals("1")));
                    CreatVertexLine(this.ShapeInfo.Vertexs.FirstOrDefault(x => x.Name.Equals("4")), this.ShapeInfo.Vertexs.FirstOrDefault(x => x.Name.Equals("8")));
                    break;
                case "5":
                    CreatVertexLine(this.ShapeInfo.Vertexs.FirstOrDefault(x => x.Name.Equals("5")), this.ShapeInfo.Vertexs.FirstOrDefault(x => x.Name.Equals("1")));
                    CreatVertexLine(this.ShapeInfo.Vertexs.FirstOrDefault(x => x.Name.Equals("5")), this.ShapeInfo.Vertexs.FirstOrDefault(x => x.Name.Equals("6")));
                    CreatVertexLine(this.ShapeInfo.Vertexs.FirstOrDefault(x => x.Name.Equals("5")), this.ShapeInfo.Vertexs.FirstOrDefault(x => x.Name.Equals("8")));
                    break;
                case "6":
                    CreatVertexLine(this.ShapeInfo.Vertexs.FirstOrDefault(x => x.Name.Equals("6")), this.ShapeInfo.Vertexs.FirstOrDefault(x => x.Name.Equals("2")), false);
                    CreatVertexLine(this.ShapeInfo.Vertexs.FirstOrDefault(x => x.Name.Equals("6")), this.ShapeInfo.Vertexs.FirstOrDefault(x => x.Name.Equals("5")));
                    CreatVertexLine(this.ShapeInfo.Vertexs.FirstOrDefault(x => x.Name.Equals("6")), this.ShapeInfo.Vertexs.FirstOrDefault(x => x.Name.Equals("7")));
                    break;
                case "7":
                    CreatVertexLine(this.ShapeInfo.Vertexs.FirstOrDefault(x => x.Name.Equals("7")), this.ShapeInfo.Vertexs.FirstOrDefault(x => x.Name.Equals("3")));
                    CreatVertexLine(this.ShapeInfo.Vertexs.FirstOrDefault(x => x.Name.Equals("7")), this.ShapeInfo.Vertexs.FirstOrDefault(x => x.Name.Equals("6")));
                    CreatVertexLine(this.ShapeInfo.Vertexs.FirstOrDefault(x => x.Name.Equals("7")), this.ShapeInfo.Vertexs.FirstOrDefault(x => x.Name.Equals("8")));
                    break;
                case "8":
                    CreatVertexLine(this.ShapeInfo.Vertexs.FirstOrDefault(x => x.Name.Equals("8")), this.ShapeInfo.Vertexs.FirstOrDefault(x => x.Name.Equals("4")));
                    CreatVertexLine(this.ShapeInfo.Vertexs.FirstOrDefault(x => x.Name.Equals("8")), this.ShapeInfo.Vertexs.FirstOrDefault(x => x.Name.Equals("5")));
                    CreatVertexLine(this.ShapeInfo.Vertexs.FirstOrDefault(x => x.Name.Equals("8")), this.ShapeInfo.Vertexs.FirstOrDefault(x => x.Name.Equals("7")));
                    break;
            }
        }

        /// <summary>
        /// 创建两个点之间的连线
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <param name="isSolid"></param>
        private void CreatVertexLine(Vertex v1, Vertex v2, bool isSolid = true)
        {
            if (v1 != null && v2 != null)
            {
                foreach (FrameworkElement element in this.rootCanvas.Children)
                {
                    if (element is ScaleLine line && line.Vertices.Contains(v1) && line.Vertices.Contains(v2))
                    {
                        return; //连接线仅创建一次
                    }
                }

                ScaleLine connectLine = new ScaleLine(new List<Vertex>() { v1, v2 }, this.rootCanvas, this.ShapeInfo.ScaleVisibility, isSolid);
                Panel.SetZIndex(connectLine, 2);
                this.rootCanvas.Children.Add(connectLine);
                this.lines.Add(connectLine);
            }
        }


        /// <summary>
        /// 父容器尺寸改变事件
        /// </summary>
        private void Parent_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            this.Width = parent.ActualWidth;
            this.Height = parent.ActualHeight;

            //点位修正
            foreach (Thumb thumb in this.Thumbs)
            {
                var vertex = thumb.DataContext as Vertex;
                Point point = new Point(vertex.X * this.parent.ActualWidth, vertex.Y * this.parent.ActualHeight);
                Canvas.SetLeft(thumb, point.X - 7);
                Canvas.SetTop(thumb, point.Y - 7);
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
                Point point = new Point(vertex.X * this.parent.ActualWidth, vertex.Y * this.parent.ActualHeight);

                //水平方向不出右边界
                bool horAccept = true;
                if (e.HorizontalChange >= 0)
                {
                    if (point.X + e.HorizontalChange > this.parent.ActualWidth)
                        horAccept = false;
                }
                else
                {
                    if (point.X + e.HorizontalChange < 0)
                        horAccept = false;
                }

                //垂直方向不出下边界
                bool verAccept = true;
                if (e.VerticalChange >= 0)
                {
                    if (point.Y + e.VerticalChange > this.parent.ActualHeight)
                        verAccept = false;
                }
                else
                {
                    if (point.Y + e.VerticalChange < 0)
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
                    point = new Point(vertex.X * this.parent.ActualWidth, vertex.Y * this.parent.ActualHeight);

                    Canvas.SetLeft(thumb, point.X - 7);
                    Canvas.SetTop(thumb, point.Y - 7);

                    foreach (var line in this.lines)
                    {
                        if (line.Vertices.Contains(vertex))
                        {
                            line.Refresh();
                        }
                    }
                }

                VertexChanged?.Invoke(vertex);
            }
        }
    }
}
