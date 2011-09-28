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
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Tweeta.ViewModel
{
    public class CoolWallItemViewModel : GenericListItem
    {
        public CoolWallItemViewModel()
        {
        }

        private CoolWallType type = CoolWallType.Profile;
        public CoolWallType Type
        {
            get { return type; }
            set
            {
                type = value;
                RaisePropertyChanged("Type");
            }
        }

        private string profilePicURL = string.Empty;
        public string ProfilePicURL 
        {
            get { return profilePicURL; }
            set
            {
                profilePicURL = value;
                RaisePropertyChanged("ProfilePicURL");
            }
        }

        private string name = string.Empty;
        public string Name
        {
            get { return name; }
            set
            {
                name= value;
                RaisePropertyChanged("Name");
            }
        }

        private long profileID = -1;
        public long ProfileID
        {
            get { return profileID; }
            set
            {
                profileID = value;
                RaisePropertyChanged("ProfileID");
            }
        }

        private long tweetID = -1;
        public long TweetID
        {
            get { return tweetID; }
            set
            {
                tweetID = value;
                RaisePropertyChanged("TweetID");
            }
        }

        private string tweetMessage = string.Empty;
        public string TweetMessage
        {
            get { return tweetMessage; }
            set
            {
                tweetMessage = value;
                RaisePropertyChanged("TweetMessage");
            }
        }


        private string imageURL = string.Empty;
        public string ImageURL
        {
            get { return imageURL; }
            set
            {
                imageURL = value;
                RaisePropertyChanged("ImageURL");
            }
        }

        public CoolWallItemViewModel This
        {
            get { return this; }
        }
    }
    public enum CoolWallType
    {
        URL,
        Image,
        Profile,
        Video
    }
}
