using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using Tweeta.ViewModel;
using TweetSharp;
using Microsoft.Phone.Shell;
using Tweeta.Common.Twitter;

namespace Tweeta
{
    public partial class ProfileView : PhoneApplicationPage
    {
        public ProfileView()
        {
            InitializeComponent();
            this.Loaded += new RoutedEventHandler(ProfileView_Loaded);
            this.pivot.SelectionChanged += new SelectionChangedEventHandler(pivot_SelectionChanged);
        }

        void pivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(this.pivot.SelectedIndex == 1)
            {
                TwitterUserViewModel vm = this.DataContext as TwitterUserViewModel;
                vm.Refresh();
            }
        }
        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            
            TwitterUserViewModel vm = this.DataContext as TwitterUserViewModel;
            if (vm == null)
            {
                if (ViewModel.ViewModelLocator.Instance.SelectedUser == null)
                {
                    ViewModel.ViewModelLocator.Instance.SelectedUser = new TwitterUserViewModel();
                }

                this.DataContext = ViewModel.ViewModelLocator.Instance.SelectedUser;
                vm = this.DataContext as TwitterUserViewModel;
            }

            if (NavigationContext.QueryString.ContainsKey("UserID"))
            {
                int id = int.Parse(NavigationContext.QueryString["UserID"]);
                
                IEnumerable<ShellTile> tiles = ShellTile.ActiveTiles.Where(a => a.NavigationUri.ToString().Contains("")); //(x => x.NavigationUri.ToString().Contains("ProfileView.xaml?UserID="));
                foreach (var item in tiles)
                {
                    if (item.NavigationUri.ToString() == e.Uri.ToString())
                    {
                        vm.ID = id;
                    }
                }
            }
            if (!vm.IsValid)
            {
                vm.IsBusy = true;
                TwitterInterface.GetUser(vm.ID,
                    delegate(TwitterUser u)
                    {
                        vm.SetUser(u);
                    });
            }
            base.OnNavigatedTo(e);
        }
        void ProfileView_Loaded(object sender, RoutedEventArgs e)
        {
           
        }

        private void ApplicationBarIconButton_Click(object sender, EventArgs e)
        {
            TwitterUserViewModel vm = this.DataContext as TwitterUserViewModel;
            StandardTileData tile = new StandardTileData()
            {
                BackgroundImage = new Uri(vm.ProfileURL, UriKind.Absolute),
                Title = vm.Username,
                BackTitle = vm.Username,
                BackContent = vm.LastTweet
            };
            ShellTile.Create(new Uri("/Views/ProfileView.xaml?UserID=" + vm.ID.ToString(), UriKind.Relative), tile);
        }
    }
}