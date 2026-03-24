using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.IO;

namespace Aipark.Wpf.Extensions
{
    public static partial class BitmapExtensions
    {
        public static Bitmap FromStream(Stream stream)
        {
            stream.Seek(0, SeekOrigin.Begin);
            return new Bitmap(stream);
        }

        /// <summary>
        /// 按约定规则和另一张图片以水平或竖直的方式拼接
        /// </summary>
        /// <param name="bitmap"></param>
        /// <param name="otherBitmap"></param>
        /// <param name="alignment"></param>
        /// <returns></returns>
        public static Bitmap Stitching(this Bitmap bitmap, Bitmap otherBitmap, Orientation alignment)
        {
            return Stitching(new List<Image>() { bitmap, otherBitmap }, alignment);
        }

        public static Bitmap Stitching(List<Image> imageList, Orientation alignment)
        {
            //图片列表
            if (imageList.Count <= 0)
                return null;

            int width = 0;
            int height = 0;

            //确定新图大小
            foreach (var image in imageList)
            {
                if (alignment == Orientation.Horizontal)
                {
                    width += image.Width;
                    if (height < image.Height)
                    {
                        height = image.Height;
                    }
                }
                else
                {
                    if (width < image.Width)
                    {
                        width = image.Width;
                    }
                    height += image.Height;
                }
            }

            //构造最终的图片白板
            Bitmap bitmap = new Bitmap(width, height);
            using (Graphics graph = Graphics.FromImage(bitmap))
            {
                //初始化这个大图
                graph.DrawImage(bitmap, 0, 0);
                //初始化当前宽
                int left = 0;
                int top = 0;

                foreach (var image in imageList)
                {
                    if (alignment == Orientation.Horizontal)
                    {
                        //区域绘制
                        graph.DrawImage(image, left, 0);
                        left += image.Width;
                    }
                    else
                    {
                        //区域绘制
                        graph.DrawImage(image, 0, top);
                        top += image.Height;
                    }
                }
            }

            return bitmap;
        }


        /// <summary>
        /// 计算原始框基于水平(竖直)方向比例缩放后的目标框
        /// </summary>
        /// <param name="rectangle">原始矩形框</param>
        /// <param name="imgSize">图像尺寸</param>
        /// <param name="orientation">基线方向</param>
        /// <param name="aspectRatio">宽高比</param>
        /// <returns></returns>
        public static Rectangle AdjustRectangleByRatioAndBoundary(Rectangle rectangle, Size imgSize, Orientation orientation = Orientation.Horizontal, [Range(0, 10)] double aspectRatio = 0)
        {
            //边界值检查
            int x1 = Math.Max(rectangle.Left, 0);
            int y1 = Math.Max(rectangle.Top, 0);
            int x2 = Math.Min(rectangle.Right, imgSize.Width);
            int y2 = Math.Min(rectangle.Bottom, imgSize.Height);

            int rectangleWidth = x2 - x1;
            int rectangleHeight = y2 - y1;

            int x_center = (x1 + x2) / 2;
            int y_center = (y1 + y2) / 2;

            //不指定基于某方向的比例调整
            if (aspectRatio > 0)
            {
                int calculateWidth = 0;
                int calculateHeight = 0;

                if (orientation == Orientation.Horizontal)
                {
                    calculateHeight = (int)(rectangleWidth / aspectRatio);
                    if (calculateHeight < rectangleHeight)
                    {
                        calculateWidth = (int)(rectangleHeight * aspectRatio);
                        calculateHeight = 0;
                    }
                }
                else
                {
                    calculateWidth = (int)(rectangleHeight * aspectRatio);
                    if (calculateWidth < rectangleWidth)
                    {
                        calculateWidth = 0;
                        calculateHeight = (int)(rectangleWidth / aspectRatio);
                    }
                }

                //计算水平方向边界
                if (calculateWidth > 0)
                {
                    if (x_center - calculateWidth / 2 < 0)
                    {
                        //左边出界
                        x1 = 0;
                        x2 = Math.Min(calculateWidth, imgSize.Width);
                    }
                    else if (x_center + calculateWidth / 2 > imgSize.Width)
                    {
                        //右边出界
                        x1 = Math.Max(imgSize.Width - calculateWidth, 0);
                        x2 = imgSize.Width;
                    }
                    else
                    {
                        x1 = x_center - calculateWidth / 2;
                        x2 = x_center + calculateWidth / 2;
                    }
                }
                //计算竖直方向边界
                if (calculateHeight > 0)
                {
                    if (y_center - calculateHeight / 2 < 0)
                    {
                        //上边出界
                        y1 = 0;
                        y2 = Math.Min(calculateHeight, imgSize.Height);
                    }
                    else if (y_center + calculateHeight / 2 > imgSize.Height)
                    {
                        //下边出界
                        y1 = Math.Max(imgSize.Height - calculateHeight, 0);
                        y2 = imgSize.Height;
                    }
                    else
                    {
                        y1 = y_center - calculateHeight / 2;
                        y2 = y_center + calculateHeight / 2;
                    }
                }

                //刷新目标框大小
                rectangleWidth = x2 - x1;
                rectangleHeight = y2 - y1;
            }

            return new Rectangle(x1, y1, rectangleWidth, rectangleHeight);
        }

        /// <summary>
        /// 计算矩形框在缩放尺寸上的相对位置
        /// </summary>
        /// <param name="rectangle"></param>
        /// <param name="baseSize"></param>
        /// <param name="destSize"></param>
        /// <returns></returns>
        public static Rectangle AdjustRectangleByImageSize(Rectangle rectangle, Size baseSize, Size destSize)
        {
            if (baseSize.IsEmpty || destSize.IsEmpty)
                return rectangle;

            double x_ratio = destSize.Width * 1.0 / baseSize.Width;
            double y_ratio = destSize.Height * 1.0 / baseSize.Height;

            int x = (int)Math.Ceiling(rectangle.Left * x_ratio);
            int y = (int)Math.Ceiling(rectangle.Top * x_ratio);

            int width = (int)Math.Floor(rectangle.Width * x_ratio);
            int height = (int)Math.Floor(rectangle.Height * x_ratio);

            return new Rectangle(x, y, width, height);
        }

        /// <summary>
        /// 矩形框在图片区域内的扩展(宽高扩展、长宽比调整和边界调整)
        /// </summary>
        /// <param name="size">图像尺寸</param>
        /// <param name="rect">矩形区域</param>
        /// <param name="offset_x">宽度扩展</param>
        /// <param name="offset_y">高度扩展</param>
        /// <param name="orientation">期望基准方向</param>
        /// <param name="aspectRatio">宽高比</param>
        public static Rectangle ExpandRectangle(Size size, Rectangle rect, int offset_x, int offset_y, Orientation? orientation = Orientation.Horizontal, double? aspectRatio = 0)
        {
            int x1 = Math.Max(rect.Left - offset_x, 0);
            int x2 = Math.Min(rect.Right + offset_x, size.Width);
            int y1 = Math.Max(rect.Top - offset_y, 0);
            int y2 = Math.Min(rect.Bottom + offset_y, size.Height);

            int rectangleWidth = x2 - x1;
            int rectangleHeight = y2 - y1;

            int x_center = (x1 + x2) / 2;
            int y_center = (y1 + y2) / 2;

            //不指定基于某方向的比例调整
            if (aspectRatio > 0)
            {
                int calculateWidth = 0;
                int calculateHeight = 0;

                if (orientation == Orientation.Horizontal)
                {
                    calculateHeight = (int)(rectangleWidth / aspectRatio);
                    if (calculateHeight < rectangleHeight)
                    {
                        calculateWidth = (int)(rectangleHeight * aspectRatio);
                        calculateHeight = 0;
                    }
                }
                else
                {
                    calculateWidth = (int)(rectangleHeight * aspectRatio);
                    if (calculateWidth < rectangleWidth)
                    {
                        calculateWidth = 0;
                        calculateHeight = (int)(rectangleWidth / aspectRatio);
                    }
                }

                //计算水平方向边界
                if (calculateWidth > 0)
                {
                    if (x_center - calculateWidth / 2 < 0)
                    {
                        //左边出界
                        x1 = 0;
                        x2 = Math.Min(calculateWidth, size.Width);
                    }
                    else if (x_center + calculateWidth / 2 > size.Width)
                    {
                        //右边出界
                        x1 = Math.Max(size.Width - calculateWidth, 0);
                        x2 = size.Width;
                    }
                    else
                    {
                        x1 = x_center - calculateWidth / 2;
                        x2 = x_center + calculateWidth / 2;
                    }
                }
                //计算竖直方向边界
                if (calculateHeight > 0)
                {
                    if (y_center - calculateHeight / 2 < 0)
                    {
                        //上边出界
                        y1 = 0;
                        y2 = Math.Min(calculateHeight, size.Height);
                    }
                    else if (y_center + calculateHeight / 2 > size.Height)
                    {
                        //下边出界
                        y1 = Math.Max(size.Height - calculateHeight, 0);
                        y2 = size.Height;
                    }
                    else
                    {
                        y1 = y_center - calculateHeight / 2;
                        y2 = y_center + calculateHeight / 2;
                    }
                }

                //刷新目标框大小
                rectangleWidth = x2 - x1;
                rectangleHeight = y2 - y1;
            }

            return new Rectangle(x1, y1, rectangleWidth, rectangleHeight);
        }

        /// <summary>
        /// 抠取指定的区域的图像
        /// </summary>
        /// <param name="source">原图</param>
        /// <param name="rectangle">抠图区域</param>
        /// <returns></returns>
        public static Bitmap ClipRectangleArea(this Image source, Rectangle rectangle)
        {
            Bitmap bitmap = new Bitmap(rectangle.Width, rectangle.Height);
            using (Graphics graphics = Graphics.FromImage(bitmap))
            {
                graphics.DrawImage(source, new Rectangle(0, 0, bitmap.Width, bitmap.Height), rectangle, GraphicsUnit.Pixel);
                graphics.Dispose();
            }
            return bitmap;
        }
    }
}
