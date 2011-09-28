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

namespace Tweeta.Views
{
    public partial class WebBrowser : PhoneApplicationPage
    {
        public WebBrowser()
        {
            InitializeComponent();
            this.webBrowser1.Loaded += new RoutedEventHandler(webBrowser1_Loaded);
        }

        void webBrowser1_Loaded(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(URL))
            {
                webBrowser1.Navigate(new Uri(URL));
            }
        }
        string URL = string.Empty;
        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            URL = NavigationContext.QueryString["URL"];
            base.OnNavigatedTo(e);
        }

    }
}