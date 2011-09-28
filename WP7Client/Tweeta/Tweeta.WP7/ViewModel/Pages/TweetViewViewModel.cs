using System;
using GalaSoft.MvvmLight;

namespace Tweeta.ViewModel
{
    public class TweetViewViewModel:ViewModelBase
    {
        public TweetViewViewModel()
        {
            tweetreply = new TweetViewModel()
            {
                AppName = string.Empty,
                AppPicURL = string.Empty,
                DateTime = DateTime.MinValue,
                Favourite = false,
                Message = string.Empty,
                ProfilePhotoURL = string.Empty,
                RealName = string.Empty,
                SearchTag = string.Empty,
                UserId = 0,
                Username = string.Empty,
            };
        }

        private TweetViewModel tweet;
        public TweetViewModel Tweet
        {
            get { return tweet; }
            set { tweet = value;
            RaisePropertyChanged("Tweet");
            }
        }

        private TweetViewModel tweetreply;
        public TweetViewModel TweetReply
        {
            get { return tweetreply; }
            set
            {

                tweetreply.ID = value.ID;
                tweetreply.AppName = value.AppName;
                tweetreply.AppPicURL = value.AppPicURL;
                tweetreply.DateTime= value.DateTime;
                tweetreply.Favourite= value.Favourite;
                
                tweetreply.ImageUrls= value.ImageUrls;
                tweetreply.Message= value.Message;
                tweetreply.ProfilePhotoURL= value.ProfilePhotoURL;
                tweetreply.RealName= value.RealName;
                tweetreply.SearchTag= value.SearchTag;
                tweetreply.Urls.AddRange(value.Urls);
                tweetreply.UserId = value.UserId;
                tweetreply.Username= value.Username;
                
                RaisePropertyChanged("TweetReply");
                RaisePropertyChanged("HasReply");
            }
        }
        
        public bool HasReply
        {
            get
            {
                return tweetreply != null && tweetreply.ID > 0;
            }
        }
    }
}
