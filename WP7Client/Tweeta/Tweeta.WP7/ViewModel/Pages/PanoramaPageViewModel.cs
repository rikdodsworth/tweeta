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
using System.Windows.Media.Imaging;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Threading;

namespace Tweeta.ViewModel.Pages
{
    public class PanoramaPageViewModel : TweetaBaseVM
    {
        public PanoramaPageViewModel()
        {

            if (IsDesignMode)
                this.BackgroundUrl = new ImageBrush() { ImageSource = new BitmapImage(new
                    Uri(Helpers.DummyDataHelper.GetDummyBg())) };
            else
            {
                if (!string.IsNullOrEmpty(App.AppSettings.AccountBGUrl))
                {
                    DispatcherHelper.CheckBeginInvokeOnUI(delegate
                    {
                        this.BackgroundUrl = new ImageBrush() { ImageSource = new BitmapImage(new
                            Uri(App.AppSettings.AccountBGUrl)) };
                    });
                }
                else
                {
                    GalaSoft.MvvmLight.Messaging.Messenger.Default.Register<NotificationMessage>(this, Callback);
                }
                
             
            }
        }
        private void Callback(NotificationMessage msg)
        {
            if (msg.Notification == "GOTUSERDETAILS")
            {
                if (!string.IsNullOrEmpty(App.AppSettings.AccountBGUrl))
                {
                    DispatcherHelper.CheckBeginInvokeOnUI(delegate
                    {
                        this.BackgroundUrl = new ImageBrush() { ImageSource = new BitmapImage(
                            new Uri(App.AppSettings.AccountBGUrl)) };
                    });
                }
            }
        }
        private ImageBrush backgroundUrl;
        public ImageBrush BackgroundUrl
        {
            get
            {
                return backgroundUrl;
            }
            set
            {
                if (backgroundUrl != value)
                {
                    backgroundUrl = value;
                    NotifyPropertyChanged("BackgroundUrl");
                }
            }
        }
    }
}
