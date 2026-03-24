using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Markup;

namespace Aipark.Wpf.Controls
{
    public class FontSizeExtension : MarkupExtension
    {
        [TypeConverter(typeof(FontSizeConverter))]
        public double Size { get; set; }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return Size;
        }
    }
}
