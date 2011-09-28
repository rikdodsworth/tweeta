//#define LOCALE_TEST;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Navigation;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Notification;
using Microsoft.Phone.Shell;
using Tweeta.Common;
using Tweeta.Services;
using Tweeta.ViewModel;
using WP7HelperFX;
using System.Threading;
using Tweeta.Resources.Strings;

namespace Tweeta
{
    public partial class App : Application
    {
        public static Settings AppSettings = new Settings();
        
        public PhoneApplicationFrame RootFrame { get; private set; }
        
        public bool IsTomstoneRestore = false;
        private bool didTombstone = false;

        public App()
        {
            //Tweeta.Data.TweetaDataContext.CreateDB();
            
#if(LOCALE_TEST)
            System.Globalization.CultureInfo newCulture = new System.Globalization.CultureInfo("es-ES");
            Thread.CurrentThread.CurrentCulture = newCulture;
            
            StringResources.Culture = Thread.CurrentThread.CurrentCulture;
#endif


            AppServices.AddService<TweetaAppService>();
            AppServices.AddService<DataCacheService>();
            AppServices.AddService<Services.Data.TimelineDataService>();

            InitializeComponent();

            InitializePhoneApplication();

            AppServices.GetService<TweetaAppService>().Init();

            GalaSoft.MvvmLight.Messaging.Messenger.Default.Register<GalaSoft.MvvmLight.Messaging.NotificationMessage>(this, AppMessagePump);
        }

        private void AppMessagePump(NotificationMessage msg)
        {
            
            if (msg.Notification == TweetViewModel.MESSAGE_CLICKED_TWEET)
            {
                var vm = msg.Sender as TweetViewModel;
                ViewModelLocator.Instance.SelectedTweet = vm;
                NavigateToPage("Views/TweetView.xaml");
            }
            else if (msg.Notification == TrendingItemViewModel.MESSAGE_CLICKED_TREND)
            {
                var vm = msg.Sender as TrendingItemViewModel;
                
                NavigateToPage("/Views/Search.xaml?searchTerm=" + Uri.EscapeDataString(vm.Title));
            }
            else if (msg.Notification == TweetViewModel.MESSAGE_CLICKED_PROFILE)
            {
                var vm = msg.Sender as TweetViewModel;
                if (vm.UserId > 0)
                {
                    if (ViewModelLocator.Instance.SelectedUser == null)
                        ViewModelLocator.Instance.SelectedUser = new TwitterUserViewModel();
                    else
                        ViewModelLocator.Instance.SelectedUser.Reset();
                    
                    ViewModelLocator.Instance.SelectedUser.ID = vm.UserId;
                    
                    NavigateToPage("Views/ProfileView.xaml");
                }
            }
        }

        public void NavigateToPage(string path)
        {
            var page = ((PhoneApplicationFrame)RootFrame).Content as PhoneApplicationPage;
            page.NavigationService.Navigate(new Uri("/" + path, UriKind.Relative));
        }

        private void Application_Launching(object sender, LaunchingEventArgs e)
        {
            IsTomstoneRestore = false;
        }

        private void Application_Activated(object sender, ActivatedEventArgs e)
        {
            if (e.IsApplicationInstancePreserved)
                return;

            if (!didTombstone)
                IsTomstoneRestore = true;

            didTombstone = false;
        }

        private void Application_Deactivated(object sender, DeactivatedEventArgs e)
        {
            didTombstone = true;
        }

        private void Application_Closing(object sender, ClosingEventArgs e)
        {
            
        }

        private void RootFrame_NavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            if (System.Diagnostics.Debugger.IsAttached)
            {
                // A navigation has failed; break into the debugger
                System.Diagnostics.Debugger.Break();
            }
        }

        #region Phone application initialization

        // Avoid double-initialization
        private bool phoneApplicationInitialized = false;

        // Do not add any additional code to this method
        private void InitializePhoneApplication()
        {
            if (phoneApplicationInitialized)
                return;

            // Create the frame but don't set it as RootVisual yet; this allows the splash
            // screen to remain active until the application is ready to render.
            RootFrame = new TransitionFrame();
            GlobalLoading.Instance.Initialize(RootFrame);
            RootFrame.Navigated += CompleteInitializePhoneApplication;
            RootFrame.Navigating += new NavigatingCancelEventHandler(RootFrame_Navigating);
            // Handle navigation failures
            RootFrame.NavigationFailed += RootFrame_NavigationFailed;


            // Ensure we don't initialize again
            phoneApplicationInitialized = true;
        }

        void RootFrame_Navigating(object sender, NavigatingCancelEventArgs e)
        {
            LastNavMode = e.NavigationMode;
        }
        public static NavigationMode LastNavMode;
        // Do not add any additional code to this method
        private void CompleteInitializePhoneApplication(object sender, NavigationEventArgs e)
        {
            // Set the root visual to allow the application to render
            if (RootVisual != RootFrame)
                RootVisual = RootFrame;

            // Remove this handler since it is no longer needed
            RootFrame.Navigated -= CompleteInitializePhoneApplication;

            //SetupPushNotifications();
        }
        static string deviceId;
        HttpNotificationChannel channel;
        private void SetupPushNotifications()
        {

            return;
            if (PhoneApplicationService.Current.State.ContainsKey("DeviceUrl"))
            {
                deviceUrl = PhoneApplicationService.Current.State["DeviceUrl"].ToString();
            }

            byte[] devId = (byte[])Microsoft.Phone.Info.DeviceExtendedProperties.GetValue("DeviceUniqueId");
            deviceId = Convert.ToBase64String(devId);

            //TweetaService.TweetaServiceClient client = new TweetaService.TweetaServiceClient();
            //client.RegisterDeviceAsync(

            try
            {
                channel = HttpNotificationChannel.Find("Tweeta");
                if (this.channel == null)
                {
                    this.channel = new HttpNotificationChannel("Tweeta", "");
                    this.channel.ChannelUriUpdated += new EventHandler<NotificationChannelUriEventArgs>(channel_ChannelUriUpdated);
                    this.channel.HttpNotificationReceived += new EventHandler<HttpNotificationEventArgs>(channel_HttpNotificationReceived);
                    this.channel.ErrorOccurred += new EventHandler<NotificationChannelErrorEventArgs>(channel_ErrorOccurred);
                    this.channel.ShellToastNotificationReceived += new EventHandler<NotificationEventArgs>(channel_ShellToastNotificationReceived);
                    this.channel.Open();
                }
                else
                {
                    deviceUrl = this.channel.ChannelUri.ToString();
                    this.channel.ChannelUriUpdated += new EventHandler<NotificationChannelUriEventArgs>(channel_ChannelUriUpdated);
                    this.channel.HttpNotificationReceived += new EventHandler<HttpNotificationEventArgs>(channel_HttpNotificationReceived);
                    this.channel.ErrorOccurred += new EventHandler<NotificationChannelErrorEventArgs>(channel_ErrorOccurred);
                    this.channel.ShellToastNotificationReceived += new EventHandler<NotificationEventArgs>(channel_ShellToastNotificationReceived);

                    UpdateServer();
                }

                if (!this.channel.IsShellTileBound)
                {
                    this.channel.BindToShellTile();
                }
                if (!this.channel.IsShellToastBound)
                {
                    this.channel.BindToShellToast();
                }
            }
            catch (Exception err)
            {
                Debug.WriteLine(err.ToString());
            }
        }

        private void UpdateServer()
        {
           // TweetaService.TweetaServiceClient client = new TweetaService.TweetaServiceClient();
          //  client.RegisterDeviceCompleted += new EventHandler<System.ComponentModel.AsyncCompletedEventArgs>(client_RegisterDeviceCompleted);
           // client.RegisterDeviceAsync(deviceId, deviceUrl, true, true);

        }

        void client_RegisterDeviceCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
          //  TweetaService.TweetaServiceClient client = new TweetaService.TweetaServiceClient();
           // if (AppSettings.HasAuthenticated && AppSettings.HasDetails)
            {
           //     client.RegisterAuthDetailsAsync(deviceId, AppSettings.AccessToken, AppSettings.AccessTokenSecret);
            }
        }

        static string deviceUrl;
        void channel_ShellToastNotificationReceived(object sender, NotificationEventArgs e)
        {
            
        }

        void channel_ErrorOccurred(object sender, NotificationChannelErrorEventArgs e)
        {
            Debug.WriteLine(e.ToString());
        }

        void channel_HttpNotificationReceived(object sender, HttpNotificationEventArgs e)
        {
            
        }

        void channel_ChannelUriUpdated(object sender, NotificationChannelUriEventArgs e)
        {
            deviceUrl = e.ChannelUri.ToString();
            PhoneApplicationService.Current.State["DeviceUrl"] = deviceUrl;
            UpdateServer();
        }

        #endregion


        private void TweetItemClicked(object sender, RoutedEventArgs e)
        {
            var vm = ((FrameworkElement)sender).DataContext as TweetViewModel;
            if (vm != null)
            {
                vm.Clicked();
            }
        }

        private void TweetProfileClicked(object sender, RoutedEventArgs e)
        {
            var vm = ((FrameworkElement)sender).DataContext as TweetViewModel;
            if (vm != null)
            {
                vm.ProfileClicked();
            }
        }

        private void TrendingItemClicked(object sender, RoutedEventArgs e)
        {
            var vm = ((FrameworkElement)sender).DataContext as TrendingItemViewModel;
            if (vm != null)
            {
                vm.Clicked();
            }
        }

    }
}