using System;
using System.Collections.Generic;
using Tweeta.Common;
using Tweeta.Services;
using WP7HelperFX;
using WP7HelperFX.MVVM;
using System.Threading;
using Tweeta.ViewModel.Items;
using GalaSoft.MvvmLight.Threading;
using Tweeta.Common.Twitter;

namespace Tweeta.ViewModel
{
    public class PanoramaRecentViewModel : TweetaItemsVM<AnimatedTileViewModel>
    {
        string dataFilename = "Recent.xml";

        public PanoramaRecentViewModel()
        {
            if (BaseViewModel.IsDesignMode)
            {
                for (int i = 0; i < 10; i++)
                {
                    Items.Add(Helpers.DummyDataHelper.CreateAnimatedTile());
                }
            }
        }

        private void RefreshInternal()
        {   
            SetIsLoading(true);
        }

        private void GetTweetsCallBack(List<AnimatedTileViewModel> vm)
        {
            DispatcherHelper.CheckBeginInvokeOnUI(delegate
            {
                foreach (var item in vm)
                {
                    this.Items.Add(item);
                }
                SetIsLoading(false);
            });
            
        }
        private void SetIsLoading(bool p)
        {
            GalaSoft.MvvmLight.Threading.DispatcherHelper.CheckBeginInvokeOnUI(() => 
            IsLoading = p
            );
        }

        DateTime lastRefreshed;
        public override void Refresh()
        {
            if (BaseViewModel.IsDesignMode)
                return;

            TimeSpan diff = DateTime.Now - lastRefreshed;
            if (diff.TotalMinutes < 1)
            {
                return;
            }

            lastRefreshed = DateTime.Now;

            ThreadPool.QueueUserWorkItem(a =>
                {
                    RefreshInternal();
                });
        }      
    }
}
