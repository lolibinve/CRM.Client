using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Aipark.Wpf.Mvvm;

namespace Aipark.Wpf.Controls
{
    /// <summary>
    /// 目标信息
    /// </summary>
    public class ShapeInfo : BindableBase
    {
        public ShapeInfo()
        {
            UniqueId = Guid.NewGuid().ToString();
            Tag = "";
            Index = 0;
            Visibility = Visibility.Visible;
            IsHitTestVisible = true;
            IsReadOnly = true;
            ScaleVisibility = Visibility.Visible;
            ThumbVisibility = Visibility.Visible;
            ThumbBrush = new SolidColorBrush(Colors.Blue);
            LineBrush = new SolidColorBrush(Colors.Chartreuse);
            Vertexs = new ObservableCollection<Vertex>();
            VertexsReady = true;
            IsChecked = false;
        }

        public string UniqueId { get; set; }

        private string tag;
        /// <summary>
        /// 标签
        /// </summary>
        public string Tag
        {
            get { return tag; }
            set
            {
                this.SetProperty(ref tag, value);
                RefreshTooltip();
            }
        }

        private int index;
        /// <summary>
        /// 索引
        /// </summary>
        public int Index
        {
            get { return index; }
            set
            {
                this.SetProperty(ref index, value);
                RefreshTooltip();
            }
        }

        private string toolTip;
        /// <summary>
        /// 提示信息
        /// </summary>
        public string Tooltip
        {
            get { return toolTip; }
            set
            {
                this.SetProperty(ref toolTip, value);
            }
        }

        private ToolTipType tooltipType;
        /// <summary>
        /// 提示信息类型
        /// </summary>
        public ToolTipType TooltipType
        {
            get { return tooltipType; }
            set
            {
                this.SetProperty(ref tooltipType, value);
                RefreshTooltip();
            }
        }

        private bool isHitTestVisible;
        /// <summary>
        /// 
        /// </summary>
        public bool IsHitTestVisible
        {
            get { return isHitTestVisible; }
            set
            {
                this.SetProperty(ref isHitTestVisible, value);
            }
        }

        private bool isReadOnly;
        /// <summary>
        /// 信息不可编辑
        /// </summary>
        public bool IsReadOnly
        {
            get { return isReadOnly; }
            set
            {
                this.SetProperty(ref isReadOnly, value);
            }
        }

        private Visibility visibility;
        /// <summary>
        /// 可见性
        /// </summary>
        public Visibility Visibility
        {
            get { return visibility; }
            set
            {
                this.SetProperty(ref visibility, value);
            }
        }

        private Visibility thumbVisibility;
        /// <summary>
        /// Thumb可见性
        /// </summary>
        public Visibility ThumbVisibility
        {
            get { return thumbVisibility; }
            set
            {
                this.SetProperty(ref thumbVisibility, value);
            }
        }

        private Visibility scaleVisibility;
        /// <summary>
        /// 刻度显示状态
        /// </summary>
        public Visibility ScaleVisibility
        {
            get { return scaleVisibility; }
            set
            {
                this.SetProperty(ref scaleVisibility, value);
            }
        }

        private Brush lineBrush;
        /// <summary>
        /// 边缘线颜色
        /// </summary>
        public Brush LineBrush
        {
            get { return lineBrush; }
            set
            {
                this.SetProperty(ref lineBrush, value);
            }
        }

        private Brush thumbBrush;
        /// <summary>
        /// Thumb控件颜色
        /// </summary>
        public Brush ThumbBrush
        {
            get { return thumbBrush; }
            set
            {
                this.SetProperty(ref thumbBrush, value);
            }
        }


        private bool isChecked;
        /// <summary>
        /// 选中(高亮显示)
        /// </summary>
        public bool IsChecked
        {
            get { return isChecked; }
            set
            {
                this.SetProperty(ref isChecked, value);
            }
        }

        private ItemCollection contextMenuItems;
        /// <summary>
        /// 右键菜单项
        /// </summary>
        public ItemCollection ContextMenuItems
        {
            get { return contextMenuItems; }
            set
            {
                this.SetProperty(ref contextMenuItems, value);
            }
        }


        private ObservableCollection<Vertex> vertexs;
        /// <summary>
        /// 目标端点集合
        /// </summary>
        public ObservableCollection<Vertex> Vertexs
        {
            get { return vertexs; }
            set
            {
                this.SetProperty(ref vertexs, value);
            }
        }

        private bool vertexsReady;
        /// <summary>
        /// 多边形目标(点位完成闭合)
        /// </summary>
        public bool VertexsReady
        {
            get { return vertexsReady; }
            set
            {
                this.SetProperty(ref vertexsReady, value);
            }
        }

        /// <summary>
        /// 刷新Tooltip
        /// </summary>
        private void RefreshTooltip()
        {
            switch (this.TooltipType)
            {
                case ToolTipType.None:
                    this.Tooltip = "";
                    break;
                case ToolTipType.Index:
                    this.Tooltip = Index.ToString();
                    break;
                case ToolTipType.Tag:
                    this.Tooltip = Tag.ToString();
                    break;
                case ToolTipType.Index_Tag:
                    this.Tooltip = $"{Index}-{Tag}";
                    break;
            }
        }
    }


    /// <summary>
    /// 图片点位信息
    /// </summary>
    public class Vertex
    {
        /// <summary>
        /// 点位名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public Vertex(double x, double y)
        {
            X = x;
            Y = y;
        }

        /// <summary>
        /// 初始化归一化的坐标信息
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="parentWidth"></param>
        /// <param name="parentHeight"></param>
        public Vertex(double x, double y, double parentWidth, double parentHeight)
        {
            X = parentWidth > 0 ? x / parentWidth : x;
            Y = parentHeight > 0 ? y / parentHeight : y;
        }

        public double X { get; set; }

        public double Y { get; set; }

        /// <summary>
        /// 附加信息
        /// 序列号 Serial Number sn:1,2,3...
        /// 可见性 Visibility    vis:1,0
        /// </summary>
        public Dictionary<string, string> Properties { get; set; }
    }


    /// <summary>
    /// Tooltip模式
    /// </summary>
    public enum ToolTipType
    {
        /// <summary>
        /// 无
        /// </summary>
        [Description("无")]
        None = 0,
        /// <summary>
        /// 索引
        /// </summary>
        [Description("索引")]
        Index = 1,
        /// <summary>
        /// 标签
        /// </summary>
        [Description("标签")]
        Tag = 2,
        /// <summary>
        /// [索引]-标签
        /// </summary>
        [Description("索引-标签")]
        Index_Tag = 3
    }


    /// <summary>
    /// 图片特征目标的形状
    /// </summary>
    public enum TargetShape
    {
        [Description("矩形")]
        Rectangle = 0,

        [Description("多边形")]
        Polygon = 1,

        [Description("多边形(带顶点描述)")]
        PolygonExt = 2,
    }
}
