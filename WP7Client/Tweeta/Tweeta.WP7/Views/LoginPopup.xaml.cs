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
using GalaSoft.MvvmLight.Messaging;
using TweetSharp;
using Tweeta.Common.Twitter;

namespace Tweeta.Views
{
    public partial class LoginPopup : UserControl
    {
        public LoginPopup()
        {
            InitializeComponent();
        }
        public const string LOGIN_COMPLETE = "LOGIN_COMPLETE";
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtUsername.Text))
            {
                MessageBox.Show("Please enter a username", "Username required", MessageBoxButton.OK);
                return;
            }
            if (string.IsNullOrEmpty(txtPassword.Password))
            {
                MessageBox.Show("Please enter a password", "Password required", MessageBoxButton.OK);
                return;
            }
            btnLogin.IsEnabled = false;

            TwitterInterface.Authenticate(txtUsername.Text, txtPassword.Password, delegate(bool success)
            {

                if (success)
                {
                    //Retrieve the info for the user
                    TwitterInterface.GetUser(int.Parse(App.AppSettings.AccountID), delegate(TwitterUser usr)
                    {
                        if (!usr.ScreenName.Contains("parlando"))
                            App.AppSettings.AccountBGUrl = usr.ProfileBackgroundImageUrl;
                        else
                        {
                            App.AppSettings.AccountBGUrl = "Images/tweeta-bg.jpg";
                        }
                        App.AppSettings.Save();
                        GalaSoft.MvvmLight.Messaging.Messenger.Default.Send<NotificationMessage>(new NotificationMessage("GOTUSERDETAILS"));
                        Dispatcher.BeginInvoke(() =>
                        {
                            Messenger.Default.Send<NotificationMessage>(new NotificationMessage(LOGIN_COMPLETE));
                        });
                    });
		

                }
                else
                {
                    Dispatcher.BeginInvoke(() =>
                    {
                        btnLogin.IsEnabled = true;
                        MessageBox.Show("Your username or password is incorrect", "Invalid credentials", MessageBoxButton.OK);
                        txtPassword.Focus();
                    });

                }
            });

        }
    }
}
