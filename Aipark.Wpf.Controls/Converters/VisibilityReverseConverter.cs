using System;
using System.Windows;
using System.Windows.Data;

namespace Aipark.Wpf.Converters
{
    /// <summary>
    /// 显示状态反转转换器
    /// </summary>
    public class VisibilityReverseConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value != null && value is Visibility visibility)
            {
                switch (visibility)
                {
                    case Visibility.Visible:
                        return Visibility.Collapsed;

                    default:
                        return Visibility.Visible;
                }
            }
            else
            {
                return Visibility.Visible;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value;
        }
    }
}
