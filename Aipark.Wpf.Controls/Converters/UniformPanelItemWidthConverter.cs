using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Aipark.Wpf.Converters
{
    /// <summary>
    /// 等分排列的容器项可用宽度转换器
    /// </summary>
    public class UniformPanelItemWidthConverter : IMultiValueConverter
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="values">容器ActualWidth,子项Margin,</param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            int length = values.Length;

            if (length == 0)
            {
                return "";
            }
            else if (length == 1)
            {
                return values[0];
            }
            else
            {
                try
                {
                    var v0 = values[0] as IEnumerable<object>; var ss = v0.ToArray();
                    var t = v0.GetType();

                    if (values[0] is IEnumerable<object> enumerable && int.TryParse(values[1].ToString(), out int index) && index > -1)
                    {
                        var array = enumerable.ToArray();
                        if (array != null && array.Length > index)
                        {
                            object obj = enumerable.ToArray()[index];
                            if (length == 2)
                            {
                                return obj.ToString();
                            }
                            else
                            {
                                object property = obj.GetType().GetProperty(values[2].ToString()).GetValue(obj);
                                return property.ToString();
                            }
                        }
                    }
                    return "";
                }
                catch
                {
                    return "";
                }
            }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
