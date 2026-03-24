using System;
using System.Windows;
using System.Windows.Data;

namespace Aipark.Wpf.Converters
{
    /// <summary>
    /// 水印文本间距转换器
    /// </summary>
    public class WatermarkPaddingConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is Thickness thickness)
            {
                return new Thickness(thickness.Left + 3, thickness.Top, thickness.Right + 3, thickness.Bottom);
            }
            else
            {
                return new Thickness(0);
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value;
        }
    }
}
