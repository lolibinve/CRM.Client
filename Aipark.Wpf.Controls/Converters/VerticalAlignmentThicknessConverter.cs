using System;
using System.Windows;
using System.Windows.Data;

namespace Aipark.Wpf.Converters
{
    /// <summary>
    /// Thickness竖直分量转换器
    /// </summary>
    public class VerticalAlignmentThicknessConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is Thickness thickness)
            {
                return new Thickness(0, thickness.Top, 0, thickness.Bottom);
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value;
        }
    }
}
