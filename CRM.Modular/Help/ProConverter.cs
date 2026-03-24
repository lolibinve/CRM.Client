using CRM.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace CRM.Modular.Help
{
    [ValueConversion(typeof(string), typeof(BitmapSource))]
    public class Base64ImageConverter : IValueConverter
    {
        //源属性传给目标属性时，调用此方法ConvertBack
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null && value is string base64)
            {
               var result = Base64ToImageExtensions.Base64ToBitmapImage(base64);

                return result;
            }
            return null;
        }

        //目标属性传给源属性时，调用此方法ConvertBack
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }



    [ValueConversion(typeof(string), typeof(BitmapSource))]
    public class EnumStrConverter : IValueConverter
    {
        //源属性传给目标属性时，调用此方法ConvertBack
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null && value is OrderState state)
            {
                string result = Enum.GetName(typeof(OrderState), state);
                return result;
            }
            return null;
        }

        //目标属性传给源属性时，调用此方法ConvertBack
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }



    [ValueConversion(typeof(int), typeof(string))]
    public class PowerConverter : IValueConverter
    {
        //源属性传给目标属性时，调用此方法ConvertBack
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null && value is int state)
            {

                string result = state == 1 ? "管理员" : "业务员";
                return result;
            }
            return null;
        }

        //目标属性传给源属性时，调用此方法ConvertBack
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }



    [ValueConversion(typeof(string), typeof(string))]
    public class DvShowConverter : IValueConverter
    {
        //源属性传给目标属性时，调用此方法ConvertBack
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null && value is ExchangeData data )
            {
                string result = data.Dv;
                double intValue;
                if(data.IsDvPercent)
                {
                    intValue = double.Parse(data.Dv) * 100 ;
                    result = intValue + "%";
                }

                return result;
            }
            return null;
        }

        //目标属性传给源属性时，调用此方法ConvertBack
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
