using System;
using System.Windows;
using System.Windows.Data;

namespace Aipark.Wpf.Controls
{
    /// <summary>
    /// Thickness水平分量转换器
    /// </summary>
    public class HorizontalAlignmentThicknessConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is Thickness thickness)
            {
                return new Thickness(thickness.Left, 0, thickness.Right, 0);
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value;
        }
    }
}
