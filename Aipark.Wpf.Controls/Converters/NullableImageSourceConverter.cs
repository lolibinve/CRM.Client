using System;
using System.Windows.Data;

namespace Aipark.Wpf.Converters
{
    public class NullableImageSourceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
            {
                return "pack://application:,,,/Aipark.Wpf.Controls;component/Resources/Images/empty.png";
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
