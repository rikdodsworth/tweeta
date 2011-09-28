using System.Collections.Generic;
using System.Collections.ObjectModel;
using GalaSoft.MvvmLight.Threading;
using Tweeta.Common;
using TweetSharp;

namespace Tweeta.ViewModel
{
    public class TwitterUserViewModel : TweetaBaseVM
    {
        TwitterUser user;
        public TwitterUserViewModel()
        {
          //  if (TweetaBaseVM.IsDesignMode)
            {
                for (int i = 0; i < 10; i++)
                {
                    this.items.Add(Helpers.DummyDataHelper.CreateTweet());
                }
                for (int i = 0; i < 10; i++)
                {
                    this.picitems.Add(new PicVM() { PicURL = Helpers.DummyDataHelper.GetPicUrl() });
                    this.IsValid = true;
                }
            }

        }

        public void SetUser(TwitterUser us)
        {
            user = us;
            
            username = us.ScreenName;
            IsValid = true;
            Realname = us.Name;
            ProfileURL = us.ProfileImageUrl;
            if(username.Contains("parlando"))
                BackgroundUrl = "Images/tweeta-bg.jpg";
            else
                BackgroundUrl = us.ProfileBackgroundImageUrl;
            Description = us.Description;
            FavouritesCount = us.FavouritesCount;
            FollowersCount = us.FollowersCount;
            FriendsCount = us.FriendsCount;
            StatusesCount = us.StatusesCount;
            Url = us.Url;
            ID = us.Id;


            if (us.Status != null)
            {
                lasttweet = us.Status.Text;
            }

            GalaSoft.MvvmLight.Threading.DispatcherHelper.UIDispatcher.BeginInvoke(
                delegate
                {
                    IsBusy = false;
                        
                    NotifyPropertyChanged("Username");
                    NotifyPropertyChanged("ProfileURL");
                    NotifyPropertyChanged("BackgroundUrl");
                    NotifyPropertyChanged("Description");
                    NotifyPropertyChanged("FavouritesCount");
                    NotifyPropertyChanged("FollowersCount");
                    NotifyPropertyChanged("FriendsCount");
                    NotifyPropertyChanged("StatusesCount");
                    NotifyPropertyChanged("Url");
                    NotifyPropertyChanged("LastTweet");
                    NotifyPropertyChanged("IsValid");
                });
        }
        
        public bool IsValid { get; set; }
        
        public int ID { get; set; }
        
        public string Username
        {
            get
            {
                return username;
            }
            set
            {
                username = value;
            }
        }
        
        string username;

        public string Realname { get; set; }
        
        public string ProfileURL{get;set;}
        
        public string BackgroundUrl{get;set;}
        
        public string Description{get;set;}
        
        public string Url{get;set;}

        public int FavouritesCount{get;set;}
        
        public int FollowersCount{get;set;}
        
        public int FriendsCount{get;set;}

        public int StatusesCount{get;set;}

        private string lasttweet;
        
        public string LastTweet
        {
            get { return lasttweet; }
            set
            {
                lasttweet = value;
                NotifyPropertyChanged("LastTweet");
            }
        }

        public void Refresh()
        {
           // this.IsBusy = true;
            
          //  TweetGetter.GetUserTweets(HandleResponse, this.ID);
        }
        
        private void HandleResponse(IEnumerable<ITwitterModel> data)
        {
            if (data!=null)
            {
                DispatcherHelper.CheckBeginInvokeOnUI(delegate
                {
                    this.IsBusy = false;
                    foreach (var item in data)
                    {
                        this.items.Add(TweetViewModel.FromTwitterStatus(item as TwitterStatus));
                    }
                });
            }
        }

        private ObservableCollection<TweetViewModel> items = new ObservableCollection<TweetViewModel>();
        public ObservableCollection<TweetViewModel> Items
        {
            get { return items; }
        }

        internal static TwitterUserViewModel FromTwitterUser(TwitterUser user)
        {
            var x = new TwitterUserViewModel();
            x.SetUser(user);
            return x;
            
        }


        private ObservableCollection<PicVM> picitems = new ObservableCollection<PicVM>();
        public ObservableCollection<PicVM> PicItems
        {
            get
            {
                return picitems;
            }
        }

        internal void Reset()
        {
            this.picitems.Clear();
            this.BackgroundUrl = string.Empty;
            this.Description = string.Empty;
            this.FavouritesCount = 0;
            this.FollowersCount = 0;
            this.FriendsCount = 0;
            this.ID = 0;
            this.IsValid = false;
            this.Items.Clear();
            this.LastTweet = string.Empty;
            this.ProfileURL = string.Empty;
            this.Realname = string.Empty;
            this.StatusesCount = 0;
            this.Url = string.Empty;
            this.Username = string.Empty;
            
        }
    }

    public class PicVM : TweetaBaseVM
    {
        public string PicURL { get; set; }
    }
}
