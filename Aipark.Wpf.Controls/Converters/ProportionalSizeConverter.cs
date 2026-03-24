using System;
using System.Windows.Data;

namespace Aipark.Wpf.Converters
{
    /// <summary>
    /// 比例尺寸转换器
    /// </summary>
    public class ProportionalSizeConverter : IValueConverter
    {
        /// <summary>
        /// 比例
        /// </summary>
        public double Scale { get; set; }
        /// <summary>
        /// 修正量
        /// </summary>
        public double Offset { get; set; }

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is double size)
            {
                return size * Scale + Offset;
            }
            else
            {
                return value;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value;
        }
    }
}
