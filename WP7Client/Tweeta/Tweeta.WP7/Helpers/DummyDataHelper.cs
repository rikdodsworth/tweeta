
using System;
using Tweeta.ViewModel;
namespace Tweeta.Helpers
{
    public class DummyDataHelper
    {
        static string[] picUrls = new string[]
        {
            "/Images/TileRed.jpg",
            "/Images/TileGreen.jpg",
            "/Images/TileBlue.jpg",
        };

        static string[] names = new string[]
        {
            "twitter user 1",
            "twitter user 2",
            "twitter user 3",
            "twitter user 4",
            "twitter user 5",
            "twitter user 6",
            "twitter user 7",
        };

        static string[] tweets = new string[]
        {
            "This is a tweet",
            "This is a really really long tweet with more text than it probably should have",
            "What a great tweet",
            "Is this really a tweet?",
            "I'm up to something really interesting today...",
            "ZOMG I'M INZ UR TWEETZ!!!",
            "This #is a #crazy #tweet about #the kind of #things that #go on in #the world"
        };
        static string[] trends = new string[]
        {
            "#trender",
            "#trender1",
            "#trender2",
            "#trender3",
            "#trender4",
        };
        public static TweetViewModel CreateTweet()
        {
            TweetViewModel vm = new TweetViewModel();
            vm.Message = tweets[GetRandomItemFromList(tweets)];
            vm.ProfilePhotoURL= picUrls[GetRandomItemFromList(picUrls)];
            vm.Username = names[GetRandomItemFromList(names)];
            //vm.RealName = names[GetRandomItemFromList(names)];
            vm.Favourite = true;
            //vm.SearchTag = "#xna";


            return vm;
        }
        public static TwitterUserViewModel CreateUser()
        {
            TwitterUserViewModel vm = new TwitterUserViewModel();
            vm.Username = names[GetRandomItemFromList(names)];
            vm.ProfileURL = picUrls[GetRandomItemFromList(picUrls)];
            vm.LastTweet = tweets[GetRandomItemFromList(tweets)];
            vm.Description = "This is my description";
            vm.FollowersCount = 100;
            vm.FriendsCount = 1000;
            vm.StatusesCount = 30995;
            
            return vm;
        }
        static Random random = new Random();
        private static int GetRandomItemFromList(string[] strings)
        {
            return random.Next(0, strings.Length);
        }
        
        internal static TrendingItemViewModel CreateTrend()
        {
            return new TrendingItemViewModel() { Title = trends[GetRandomItemFromList(trends)] };   
        }

        internal static ViewModel.Items.AnimatedTileViewModel CreateAnimatedTile()
        {
            return new ViewModel.Items.AnimatedTileViewModel()
            {
                FrontTitle = names[GetRandomItemFromList(names)],
                FrontImage = picUrls[GetRandomItemFromList(picUrls)],
                FrontContent = tweets[GetRandomItemFromList(tweets)],
                BackTitle = names[GetRandomItemFromList(names)],
                BackImage = picUrls[GetRandomItemFromList(picUrls)],
                BackContent = tweets[GetRandomItemFromList(tweets)],
            };
        }

        internal static string GetDummyBg()
        {
            return "http://thisisatest.url";
        }

        internal static string GetPicUrl()
        {
            return picUrls[GetRandomItemFromList(picUrls)];
        }
        internal static string GetSearchTerm()
        {
            return trends[GetRandomItemFromList(trends)];
        }
    }
}
