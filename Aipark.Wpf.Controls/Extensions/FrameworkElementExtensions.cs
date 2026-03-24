using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace Aipark.Wpf.Extensions
{
    public static class FrameworkElementExtensions
    {
        /// <summary>
        /// 从<see cref="UIElementCollection"/>中移除指定类型的元素
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        /// <returns></returns>
        public static int RemoveAll<T>(this UIElementCollection collection) where T : UIElement
        {
            List<UIElement> list = new List<UIElement>();
            foreach (UIElement item in collection)
            {
                if (item is T _)
                    list.Add(item);
            }
            foreach (var item in list)
            {
                collection.Remove(item);
            }
            return list.Count;
        }

        /// <summary>
        /// 转换控件Tag信息至指定类型
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="control"></param>
        /// <returns></returns>
        public static T Tag<T>(this FrameworkElement control)
        {
            if (control.Tag != null && control.Tag is T t)
            {
                return t;
            }
            return default(T);
        }

        /// <summary>
        /// 返回<see cref="UIElementCollection"/>中第一个或默认元素
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        /// <returns></returns>
        public static T FirstOrDefault<T>(this UIElementCollection collection) where T : UIElement
        {
            foreach (UIElement item in collection)
            {
                if (item is T t)
                    return t;
            }
            return default(T);
        }

    }
}
