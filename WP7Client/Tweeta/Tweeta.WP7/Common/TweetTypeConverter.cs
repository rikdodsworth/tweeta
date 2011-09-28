using System;
using System.Globalization;
using System.Windows.Data;
using Tweeta.ViewModel;
using Tweeta.CustomControl;


namespace Tweeta.Common.Converter
{
    public class TweetTypeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            CoolWallItemViewModel model = value as CoolWallItemViewModel;
            if (model == null)
                return new TweetType();
            
            return new TweetType()
                {
                    Message = model.TweetMessage,
                    Username = model.Name,
                    ProfilePicUrl = model.ProfilePicURL,
                    PicUrl  = model.ImageURL
                };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    public class TweetType
    {
        public TweetType()
        {
            Message = string.Empty;
        }
        public string Message { get; set; }
        public string Username { get; set; }
        public string ProfilePicUrl { get; set; }
        public string PicUrl { get; set; }

    }
}
