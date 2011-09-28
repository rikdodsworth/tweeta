using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace WP7HelperFX.Common.Converter
{
    public class UriToImageBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string url = value.ToString();
            UriKind kind = url.StartsWith("htt") ? UriKind.Absolute : UriKind.Relative;
        
            ImageBrush brush = new ImageBrush
                { 
                    Stretch = Stretch.Fill, 
                    ImageSource = new BitmapImage(new Uri(url, kind)) 
                };

            return brush;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
