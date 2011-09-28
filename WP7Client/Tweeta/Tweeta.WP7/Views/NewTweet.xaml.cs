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
using System.Text;
using Hammock;
using Tweeta.Common;
using System.Diagnostics;
using Hammock.Authentication.OAuth;
using TweetSharp;
using GalaSoft.MvvmLight.Messaging;
using Tweeta.ViewModel;
using System.Windows.Media.Imaging;
using System.IO;
using System.Windows.Threading;
using Tweeta.Common.Twitter;

namespace Tweeta
{
    public partial class NewTweet : PhoneApplicationPage
    {
        DispatcherTimer timer;
        public NewTweet()
        {
            InitializeComponent();

            this.Loaded += new RoutedEventHandler(NewTweet_Loaded);
            this.txtTweetText.Loaded += new RoutedEventHandler(txtTweetText_Loaded);
            this.txtTweetText.TextChanged += new TextChangedEventHandler(txtTweetText_TextChanged);

            this.progress.IsIndeterminate = false;
            this.progress.Visibility = System.Windows.Visibility.Collapsed;
        }

        void txtTweetText_TextChanged(object sender, TextChangedEventArgs e)
        {
            txtCount.Text = (140 - txtTweetText.Text.Length).ToString();
        }
        
        void txtTweetText_Loaded(object sender, RoutedEventArgs e)
        {
            timer = new DispatcherTimer()
            {
                Interval = TimeSpan.FromMilliseconds(300)
            };
            timer.Tick+=new EventHandler(timer_Tick);
            timer.Start();
        }

        void timer_Tick(object sender, EventArgs e)
        {
            timer.Stop();
            timer = null;
            Dispatcher.BeginInvoke(() =>
            txtTweetText.Focus());
        }

        void NewTweet_Loaded(object sender, RoutedEventArgs e)
        {
            
        }

        private void ApplicationBarIconButton_Click(object sender, EventArgs e)
        {
            //TODO Twit pic integration
        }

        bool queueTweetPost;
        private void SubmitTweet(object sender, EventArgs e)
        {
            DoTweet();
        }

        private void DoTweet()
        {
            ((Microsoft.Phone.Shell.ApplicationBarIconButton)this.ApplicationBar.Buttons[0]).IsEnabled = false;
            ((Microsoft.Phone.Shell.ApplicationBarIconButton)this.ApplicationBar.Buttons[1]).IsEnabled = false;
            progress.IsIndeterminate = true;
            progress.Visibility = System.Windows.Visibility.Visible;

            TwitterInterface.PostTweet(txtTweetText.Text,
                delegate(TwitterStatus status)
                {
                    if (status != null)
                    {
                        Dispatcher.BeginInvoke(
                            delegate
                            {
                                progress.IsIndeterminate = false;
                                
                                GalaSoft.MvvmLight.Messaging.Messenger.Default.Send<NotificationMessage>(new NotificationMessage(PanoramaPageViewModel.MESSAGE_REFRESH_TIMELINE));
                                //ViewModel.ViewModelLocator.Instance.Timeline.Refresh();
                                NavigationService.GoBack();
                            });
                    }
                    else
                    {
                        GalaSoft.MvvmLight.Threading.DispatcherHelper.CheckBeginInvokeOnUI(delegate
                        {
                            MessageBox.Show("Failed");
                        });
                    }
                });
        }
    }
}