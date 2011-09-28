using System;
using System.Globalization;
using System.Windows.Data;

namespace WP7HelperFX.Common.Converter
{
    public class TextCaseConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) 
                return string.Empty;

            if (parameter is string)
            {
                string param = (parameter as string).ToLower();

                if (param == "lower")
                    return value.ToString().ToLower();
                if (param == "upper")
                    return value.ToString().ToUpper();
            }

            return value.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

    }
}
