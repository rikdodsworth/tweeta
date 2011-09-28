using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using TweetSharp;

namespace Tweeta.ViewModel
{
    public class TweetViewModel : GenericListItem 
    {
        public int UserId { get; set; }
        private string username;
        public string Username
        {
            get { return username; }
            set 
            { 
                username = value;
                base.RaisePropertyChanged("Username");
            }
        }

        private string message;
        public string Message
        {
            get { return message; }
            set
            {
                message = value;
                base.RaisePropertyChanged("Message");
            }
        }

        private string realName = string.Empty;
        public string RealName
        {
            get { return realName; }
            set
            {
                realName = value;
                base.RaisePropertyChanged("RealName");
            }
        }
        private bool favourite = false;
        public bool Favourite
        {
            get { return favourite; }
            set
            {
                favourite = value;
                base.RaisePropertyChanged("Favourite");
            }
        }

        
        private List<string> urls = new List<string>();
        public List<string> Urls
        {
            get { return urls; }
        }
        /*
        private void CheckForImages()
        {
            this.ImageUrls.Clear();
            var regex = new Regex(@"((https?://)?([-\w]+\.[-\w\.]+)+\w(:\d+)?(/([-\w/_\.]*(\?\S+)?)?)*)");
            var matches = regex.Matches(this.message);
            foreach (var item in matches)
            {
                Match m = item as Match;
                var str = m.Value.ToLower();
                this.urls.Add(str);
            }

            if (urls.Count > 0)
            {
                
                GalaSoft.MvvmLight.Threading.DispatcherHelper.UIDispatcher.BeginInvoke(delegate
                {
                    foreach (var item in urls)
                    {

                        ImageUrlParserHelper.Parse(item, delegate(string newUrl)
                        {
                            GalaSoft.MvvmLight.Threading.DispatcherHelper.UIDispatcher.BeginInvoke(
                                delegate
                                {
                                    this.ImageUrls.Add(newUrl);
                                    //ViewModelLocator.Instance.ActiveContent.UpdateUrl(newUrl, this);
                                });
                        });

                        //ViewModelLocator.Instance.ActiveContent.AddUrl(item, this);
                    }
                });

                //TweetAnalyzer.Analyze(this);
            }
        }*/
        private string profilePhotoURL;
        public string ProfilePhotoURL
        {
            get { return profilePhotoURL; }
            set
            {
                profilePhotoURL = value;
                base.RaisePropertyChanged("ProfilePhotoURL");
            }
        }
        public long ID { get; set; }
        private string appName;
        public string AppName
        {
            get { return appName; }
            set
            {
                appName = value;
                RaisePropertyChanged("AppName");
            }
        }
        private string appPicURL;
        public string AppPicURL
        {
            get { return appPicURL; }
            set
            {
                appPicURL = value;
                RaisePropertyChanged("AppPicURL");
            }
        }

        private string searchResult;
        public string SearchTag
        {
            get { return searchResult; }
            set
            {
                searchResult = value;
                RaisePropertyChanged("SearchTag");
            }
        }
        
        private DateTime date;
        public DateTime DateTime
        {
            get { return date; }
            set
            {
                date = value;
                base.RaisePropertyChanged("DateTime");
            }
        }

        internal static TweetViewModel FromTwitterStatus(TweetSharp.TwitterStatus item)
        {
            int id = item.User.Id;
            /*
            if (item.Entities != null && item.Entities.HashTags.Count > 0)
            {
                ViewModelLocator.Instance.SearchResults.AddItem(item);
            }
            */
            string source = item.Source;
            if (source.Contains("<"))
            {
                source = source.Substring(source.IndexOf(">") + 1);
                source = source.Substring(0, source.IndexOf("<"));
            }
            
                
            var vm = new TweetViewModel()
            {
                Username = item.User.ScreenName,
                Message = item.Text,
                ProfilePhotoURL = item.User.ProfileImageUrl,
                ID = item.Id,
                AppName = source,
                AppPicURL = item.Author.ProfileImageUrl,
                RealName = item.User.Name,
                DateTime = item.CreatedDate,
                UserId = item.User.Id
            };
            
            return vm;
        }
        private static string ConvertToWhen(DateTime dateTime)
        {
            var diff = DateTime.UtcNow - dateTime;
            int seconds = (int)Math.Abs(diff.TotalSeconds);
            int minutes = (int)Math.Abs(diff.TotalMinutes);
            int hours = (int)Math.Abs(diff.TotalHours);

            if (seconds < 60)
                return "Just now";
            else if (hours < 1)
                return string.Format("{0} minutes ago", minutes);
            else if (hours < 24)
                return string.Format("{0} hours ago", hours);
            else
            {
                if (Math.Abs(diff.TotalDays) < 2)
                {
                    return string.Format("yesterday");
                }
                else
                    return string.Format("{0} days ago", (int)(diff.TotalDays));
            }
        }

        private ObservableCollection<string> imageUrls = new ObservableCollection<string>();
        public ObservableCollection<string> ImageUrls
        {
            get { return imageUrls; }
            set { imageUrls = value; }
        }

        internal void Clicked()
        {
            GalaSoft.MvvmLight.Messaging.Messenger.Default.Send<GalaSoft.MvvmLight.Messaging.NotificationMessage>(
                new GalaSoft.MvvmLight.Messaging.NotificationMessage(this, MESSAGE_CLICKED_TWEET));
            
        }

        public const string MESSAGE_CLICKED_TWEET = "ClickedTweet";
        public const string MESSAGE_CLICKED_PROFILE = "ClickedProfile";

        internal void ProfileClicked()
        {
           GalaSoft.MvvmLight.Messaging.Messenger.Default.Send<GalaSoft.MvvmLight.Messaging.NotificationMessage>(
              new GalaSoft.MvvmLight.Messaging.NotificationMessage(this, MESSAGE_CLICKED_PROFILE));   
            
        }
    }
}
