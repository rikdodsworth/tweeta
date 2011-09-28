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
using Tweeta.Resources.Strings;
using WP7HelperFX;

namespace Tweeta.ViewModel
{
    public class TweetaItemsVM<T> : WP7HelperFX.MVVM.ItemsViewModel<T>
    {
        public StringResources AppResources { get { return TweetaBaseVM.appResources; } }

        private bool isLoading = false;
        public bool IsLoading
        {
            get { return isLoading; }
            set
            {
                isLoading = value;
                GlobalLoading.Instance.IsLoading = isLoading;

                base.NotifyPropertyChanged("IsLoading");
            }
        }


    }
}
