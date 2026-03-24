using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace Aipark.Wpf.Converters
{
    /// <summary>
    /// 要求对象为绑定的第一个参数,属性为绑定的第二个参数
    /// </summary>
    public class DisplayMemberConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            try
            {
                if (values.Length < 1)
                    return "";

                if (values[0] == null)
                    return "";

                if (values.Length < 2 || string.IsNullOrWhiteSpace(values[1].ToString()))
                {
                    return values[0];
                }

                string propertyName = values[1].ToString();
                Type sourceType = values[0].GetType();
                if (sourceType.GetProperties().Any(x => x.Name.Equals(propertyName)))
                {
                    object obj = sourceType.GetProperty(propertyName).GetValue(values[0]);
                    return obj.ToString();
                }
                else
                {
                    return values[0];
                }
            }
            catch (Exception)
            {
                return values[0];
            }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
