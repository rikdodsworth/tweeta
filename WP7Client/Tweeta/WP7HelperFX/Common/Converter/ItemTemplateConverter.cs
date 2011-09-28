using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows;

namespace WP7HelperFX.Common.Converter
{
    public class ItemTemplateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var ContentTemplate = DataTemplateHelper.LoadFromDictionary(Application.Current, value.ToString());

            return ContentTemplate;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
