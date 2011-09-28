using System;
using System.Globalization;
using System.Windows.Data;

namespace WP7HelperFX.Common.Converter
{
    public class DateTimeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var str = ((DateTime)value).ToString(parameter.ToString());
            return str;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

    }
}
