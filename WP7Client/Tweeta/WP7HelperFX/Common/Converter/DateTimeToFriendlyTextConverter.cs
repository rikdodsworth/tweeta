using System;
using System.Globalization;
using System.Windows.Data;

namespace WP7HelperFX.Common.Converter
{
    public class DateTimeToFriendlyTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var diff = DateTime.UtcNow - (DateTime)value;
            int seconds = (int)Math.Abs(diff.TotalSeconds);
            int minutes = (int)Math.Abs(diff.TotalMinutes);
            int hours = (int)Math.Abs(diff.TotalHours);

            if (seconds < 60)
            {
                return "Just now";
            }
            else if (hours < 1)
            {
                return string.Format("{0} minutes ago", minutes);
            }
            else if (hours < 24)
            {
                return string.Format("{0} hours ago", hours);
            }
            else
            {
                if (Math.Abs(diff.TotalDays) < 2)
                    return string.Format("yesterday");
                else
                    return string.Format("{0} days ago", (int)(diff.TotalDays));
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
