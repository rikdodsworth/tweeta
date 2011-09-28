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
using GalaSoft.MvvmLight.Messaging;
using Tweeta.ViewModel;
using TweetSharp;
using Tweeta.Common.Twitter;

namespace Tweeta
{
    public partial class TweetView : PhoneApplicationPage
    {
        public TweetView()
        {
            InitializeComponent();
            this.txtReply.KeyUp += new KeyEventHandler(txtReply_KeyUp);
        }

        void txtReply_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                this.Focus();
                txtReply.IsEnabled = false;
                prog.IsIndeterminate = true;
                TwitterInterface.PostTweet(txtReply.Text, delegate(TwitterStatus status)
                {

                    if (status == null)
                    {
                        Dispatcher.BeginInvoke(delegate
                        {
                            prog.IsIndeterminate = false;
                            MessageBox.Show("Tweet failed to send", "Tweet failed", MessageBoxButton.OK);
                        });
                    }
                    else
                    {
                        Dispatcher.BeginInvoke(delegate
                        {
                            ViewModelLocator.Instance.TweetViewPage.TweetReply =
                                TweetViewModel.FromTwitterStatus(status);

                            prog.IsIndeterminate = false;
                            txtReply.IsEnabled = true;
                            txtReply.Text = string.Empty;
                            Messenger.Default.Send<NotificationMessage>(new NotificationMessage(PanoramaPageViewModel.MESSAGE_REFRESH_TIMELINE));
                        });
                    }
                });
                //post tweet
            }
        }

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            var vm = this.DataContext as ViewModel.TweetViewViewModel;
            if (!txtReply.Text.Contains(vm.Tweet.Username))
            {
                txtReply.Text = "@" + vm.Tweet.Username + " " + txtReply.Text;
                txtReply.SelectionStart = txtReply.Text.Length;
            }
        }
    }
}
