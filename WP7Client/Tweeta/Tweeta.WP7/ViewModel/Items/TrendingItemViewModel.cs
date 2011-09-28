using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using TweetSharp;

namespace Tweeta.ViewModel
{
    public class TrendingItemViewModel : GenericListItem
    {
        private string title;
        public string Title
        {
            get { return title; }
            set
            {
                title = value;
                RaisePropertyChanged("Title");
            }
        }

        internal static TrendingItemViewModel FromTrend(TweetSharp.ITwitterModel item)
        {
            var trend = item as TwitterTrend;
            return new TrendingItemViewModel()
            {
                Title = trend.Name
            };
        }

        public const string MESSAGE_CLICKED_TREND = "ClickedTrend";
        internal void Clicked()
        {
            GalaSoft.MvvmLight.Messaging.Messenger.Default.Send<GalaSoft.MvvmLight.Messaging.NotificationMessage>(
                new GalaSoft.MvvmLight.Messaging.NotificationMessage(this, MESSAGE_CLICKED_TREND));

        }
    }
}
