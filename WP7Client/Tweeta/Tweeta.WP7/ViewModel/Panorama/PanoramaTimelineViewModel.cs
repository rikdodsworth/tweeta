using System;
using System.Collections.Generic;
using Tweeta.Common;
using Tweeta.Services;
using WP7HelperFX;
using WP7HelperFX.MVVM;
using System.Threading;

namespace Tweeta.ViewModel
{
    public class PanoramaTimelineViewModel : TweetaItemsVM<TweetViewModel>
    {
        string dataFilename = "timelineData.xml";
        
        public PanoramaTimelineViewModel()
        {
            if (BaseViewModel.IsDesignMode)
            {
                for (int i = 0; i < 10; i++)
                {
                    Items.Add(Helpers.DummyDataHelper.CreateTweet());
                }
            }
            //else
            //{
            //    this.SetDataAction(DoRefresh);
            //}
        }

        private void RefreshInternal()
        {   
            
            SetIsLoading(true);
            long? since = null;
            if(this.Items.Count > 0)
            {
                since = this.Items[0].ID;
            }

            AppServices.GetService<Services.Data.ITimelineDataService>().GetData(GetTimelineCallback, since);
        }

        private void GetTimelineCallback(bool success, List<TweetViewModel> data)
        {
            IsLoading = false;
            if (success)
            {
                if (this.Items.Count > 0)
                {
                    for (int i = data.Count - 1; i >= 0; i--)
                    {
                        this.Items.Insert(0, data[i]);
                    }
                }
                else
                {
                    foreach (var item in data)
                    {
                        this.Items.Add(item);
                    }
                }

                var lst = new List<TweetViewModel>();
                foreach (var item in Items)
                {
                    
                    lst.Add(item);
                }

                WP7HelperFX.AppServices.GetService<DataCacheService>().SaveToCache(dataFilename, lst);
            }

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
