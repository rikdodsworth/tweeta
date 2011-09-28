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
using GalaSoft.MvvmLight;
using Tweeta.ViewModel;
using GalaSoft.MvvmLight.Messaging;
using WP7HelperFX.MVVM;
using System.Windows.Media.Imaging;
using System.Diagnostics;
using Microsoft.Phone.Shell;

namespace Tweeta.Views
{
    public partial class MainPanoramaPage : PhoneApplicationPage
    {
        LoginPopup popup;
                
        public MainPanoramaPage()
        {
            InitializeComponent();
            if (!App.AppSettings.HasAuthenticated)
            {
                popup = new LoginPopup();
                
                popup.VerticalAlignment = System.Windows.VerticalAlignment.Top;
                popup.Width = popup.Height = 480;
                this.LayoutRoot.Children.Add(popup);

                Messenger.Default.Register<NotificationMessage>(this, LoginMessage);

            }
            panorama.SelectionChanged += new EventHandler<SelectionChangedEventArgs>(panoram_SelectionChanged);
            this.panorama.Loaded += new RoutedEventHandler(panoram_Loaded);
        }

        private void LoginMessage(NotificationMessage msg)
        {
            if (msg.Notification == LoginPopup.LOGIN_COMPLETE)
            {
                if (popup != null)
                {
                    RefreshActivePanel();
                    this.LayoutRoot.Children.Remove(popup);
                    
                    /*
                    if (!string.IsNullOrEmpty(App.AppSettings.AccountBGUrl))
                    {
                        var source = new BitmapImage(new Uri(App.AppSettings.AccountBGUrl, UriKind.Absolute));
                        var brush = new ImageBrush() { ImageSource = source };
                        brush.ImageOpened += new EventHandler<RoutedEventArgs>(brush_ImageOpened);
                        panoram.Background = brush;

                    }*/

                }
            }
        }

        void brush_ImageOpened(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("");
        }

        void panoram_Loaded(object sender, RoutedEventArgs e)
        {
            if (App.AppSettings.HasAuthenticated)
            {
                RefreshActivePanel();
            }
            else
            {
                Console.Write("");
            }
        }

        void NavigateToPage()
        {
            //NavigationService.Navigate(new Uri("/Views/NewTweet.xaml", UriKind.Relative));
        }
        IEnumerable<DependencyObject> GetChildsRecursive(DependencyObject root)
        {
            List<DependencyObject> elts = new List<DependencyObject>();
            // elts.Add(root);
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(root); i++)
                elts.AddRange(GetChildsRecursive(VisualTreeHelper.GetChild(root, i)));
            return elts;
        }

        IEnumerable<DependencyObject> GetAllChildren(DependencyObject root)
        {
            List<DependencyObject> items = new List<DependencyObject>();
            int childCount = VisualTreeHelper.GetChildrenCount(root);

            for (int i = 0; i < childCount; i++)
            {
                var newItem = VisualTreeHelper.GetChild(root, i);
                items.Add(newItem);

                items.AddRange(GetAllChildren(newItem));
            }
            return items;
        }
        void panoram_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            /*
            var list = GetAllChildren(this.panoram.ItemContainerGenerator.ContainerFromIndex(0));

            foreach (var item in list)
            {
                if (item is FrameworkElement)
                    Debug.WriteLine(item.GetType().Name + " - " + ((FrameworkElement)item).Name);
            }
              */

            RefreshActivePanel();
        }

        private void RefreshActivePanel()
        {
            ApplicationBar = (ApplicationBar)Resources["appBarFull"];

            if (panorama.SelectedIndex == 4)
            {
                ApplicationBar.Mode = ApplicationBarMode.Minimized;
            }
            else
            {
                ApplicationBar.Mode = ApplicationBarMode.Default;
            }

            IRefreshable model = ((PanoramaItem)this.panorama.Items[panorama.SelectedIndex]).DataContext as IRefreshable;

            if (model != null)
            {
                model.Refresh();
            }
            else
            {
                Console.Write("");
            }
        }

        private void SearchClicked(object sender, EventArgs e)
        {
            ((App)App.Current).NavigateToPage("/Views/Search.xaml");
        }

        private void NewTweetClicked(object sender, EventArgs e)
        {
            ((App)App.Current).NavigateToPage("/Views/NewTweet.xaml");
        }
    }
}