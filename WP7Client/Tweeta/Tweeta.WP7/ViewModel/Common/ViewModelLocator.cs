
using GalaSoft.MvvmLight;

namespace Tweeta.ViewModel
{
    public class ViewModelLocator
    {
        private static TweetViewViewModel tweetPageViewModel;   
        
        private static TwitterUserViewModel selectedUser;
        
        public static ViewModelLocator Instance;
        
        public ViewModelLocator()
        {
            Instance = this;
            tweetPageViewModel = new TweetViewViewModel();


            tweetPageViewModel.Tweet = new TweetViewModel();
            
            if (ViewModelBase.IsInDesignModeStatic)
            {
                tweetPageViewModel.Tweet = Helpers.DummyDataHelper.CreateTweet();
                TweetViewPage.TweetReply = tweetPageViewModel.Tweet;
                selectedUser = Helpers.DummyDataHelper.CreateUser();
            }
            else
            {
                selectedUser = Helpers.DummyDataHelper.CreateUser();
            }
        }

        
        public TweetViewViewModel TweetViewPage
        {
            get { return tweetPageViewModel; }
        }

        
        public TwitterUserViewModel SelectedUser
        {
            get { return selectedUser; }
            set { selectedUser = value; }
        }
        public TweetViewModel SelectedTweet
        {
            get { return this.TweetViewPage.Tweet; }
            set
            {
                this.TweetViewPage.Tweet = value;
            }
        }
        
    }
}