using System.Windows;
using System.Windows.Media;

namespace Aipark.Wpf.Controls.Extensions
{
    public static class DependencyObjectExtensions
    {
        /// <summary>
        /// 获取第一个类型匹配的父级对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dependencyObject"></param>
        /// <returns></returns>
        public static T GetFirstParent<T>(this DependencyObject dependencyObject) where T : DependencyObject
        {
            DependencyObject dependency = dependencyObject;
            do
            {
                if (dependency is T instance)
                    return instance;
            } while ((dependency = VisualTreeHelper.GetParent(dependency)) != null);

            return null;
        }

    }
}
