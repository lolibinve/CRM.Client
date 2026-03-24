using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace Aipark.Wpf.Controls
{
    /// <summary>
    /// 自定义形状的抽象基类
    /// </summary>
    public abstract class BaseShape : UserControl
    {
        /// <summary>
        /// 父容器
        /// </summary>
        public Canvas parent;

        /// <summary>
        /// 目标数据信息
        /// </summary>
        public ShapeInfo ShapeInfo { get; set; }

        /// <summary>
        /// 顶点集合
        /// </summary>
        public List<Thumb> Thumbs = new List<Thumb>();

        /// <summary>
        /// 连线集合
        /// </summary>
        public List<ScaleLine> Lines = new List<ScaleLine>();

        /// <summary>
        /// 右键菜单事件
        /// </summary>
        public event Action<BaseShape, string> MenuItemClick;
        /// <summary>
        /// 形状被选中事件
        /// </summary>
        public event Action<BaseShape, ShapeInfo> Selected;
        /// <summary>
        /// 多边形标定完成事件
        /// </summary>
        public event Action<BaseShape> PolygonCompleted;
        /// <summary>
        /// 多边形撤销点事件
        /// </summary>
        public event Action<BaseShape> PolygonVertexCanceled;

        /// <summary>
        /// 撤销多边形最后一点
        /// </summary>
        public virtual void CancelLastPoint() { }
        /// <summary>
        /// 测试多边形点位效果
        /// </summary>
        public virtual void TestLastPoint(Point point) { }
        ///// <summary>
        ///// 高亮显示
        ///// </summary>
        //public virtual void Highlight() { }

        protected virtual void OnMenuItemClick(BaseShape baseShape, string cmd)
        {
            MenuItemClick?.Invoke(baseShape, cmd);
        }
        protected virtual void OnChecked(BaseShape baseShape, ShapeInfo objectInfo)
        {
            Selected?.Invoke(baseShape, objectInfo);
        }
        protected virtual void OnPolygonCompleted(BaseShape baseShape)
        {
            PolygonCompleted?.Invoke(baseShape);
        }
        protected virtual void OnPolygonVertexCanceled(BaseShape baseShape)
        {
            PolygonVertexCanceled?.Invoke(baseShape);
        }

        public BaseShape() { }

        protected BaseShape(ShapeInfo objectInfo)
        {
            this.ShapeInfo = objectInfo;
        }
    }


    /// <summary>
    /// 标定对象创建工厂
    /// </summary>
    public static class BaseShapeFactory
    {
        public static BaseShape GetShape(TargetShape targetShape, ShapeInfo objectInfo)
        {
            switch (targetShape)
            {
                case TargetShape.Rectangle:
                    return new RectangleShape(objectInfo);
                case TargetShape.Polygon:
                    return new PolygonShape(objectInfo);
                case TargetShape.PolygonExt:
                    return new PolygonExtShape(objectInfo);
            }
            return null;
        }

        ///// <summary>
        ///// 获取图像顺时针旋转的角度
        ///// </summary>
        ///// <param name="json"></param>
        ///// <returns></returns>
        //public static int GetRotateAngle(string json)
        //{
        //    if (!string.IsNullOrWhiteSpace(json))
        //    {
        //        ImageInformation image = JsonHelper.DeserializeObject<ImageInformation>(json, "image");
        //        if (image != null)
        //        {
        //            return image.Angle;
        //        }
        //    }
        //    return 0;
        //}

        ///// <summary>
        ///// 生成标注目标形状
        ///// </summary>
        ///// <param name="annotation"></param>
        ///// <param name="targetShape"></param>
        ///// <param name="tooltipType"></param>
        ///// <param name="contentSize"></param>
        ///// <param name="showLineScale"></param>
        ///// <param name="isEditable"></param>
        ///// <returns></returns>
        //public static List<BaseShape> GetShapes(AnnotationInformation annotation, TargetShape targetShape, TooltipType tooltipType, Size contentSize, bool showLineScale, bool isEditable)
        //{
        //    List<BaseShape> shapes = new List<BaseShape>();

        //    if (annotation == null || annotation.Objects == null)
        //        return shapes;

        //    foreach (var obj in annotation.Objects)
        //    {
        //        ObjectInfo objectInfo = new ObjectInfo()
        //        {
        //            IsClosed = true,
        //            ShowScale = showLineScale,
        //            IsEditable = isEditable,
        //            Tag = obj.Name,
        //            TooltipType = tooltipType
        //        };

        //        foreach (var p in obj.Points)
        //        {
        //            Vertex vertex = new Vertex(p.X, p.Y, contentSize.Width, contentSize.Height);
        //            if (p.Properties != null && p.Properties.Count > 0)
        //            {
        //                vertex.Properties = new Dictionary<string, string>();
        //                foreach (var item in p.Properties)
        //                {
        //                    vertex.Properties.Add(item.Key, item.Value);
        //                }
        //            }
        //            objectInfo.Points.Add(vertex);
        //        }

        //        switch (targetShape)
        //        {
        //            case TargetShape.Rectangle:
        //                shapes.Add(new RectangleShape(objectInfo));
        //                break;
        //            case TargetShape.Polygon:
        //                shapes.Add(new PolygonShape(objectInfo));
        //                break;
        //            case TargetShape.PolygonExt:
        //                shapes.Add(new PolygonExtShape(objectInfo));
        //                break;
        //            default:
        //                break;
        //        }
        //    }

        //    return shapes;
        //}
    }
}
