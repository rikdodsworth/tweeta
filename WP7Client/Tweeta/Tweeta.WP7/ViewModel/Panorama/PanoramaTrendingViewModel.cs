using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using Newtonsoft.Json;
using Tweeta.Common;
using TweetSharp;
using WP7HelperFX;
using WP7HelperFX.MVVM;
using System.Threading;
using Tweeta.Common.Twitter;

namespace Tweeta.ViewModel
{
    public class PanoramaTrendingViewModel : TweetaItemsVM<TrendingItemViewModel>
    {
        private bool isLoading = false;
        private bool isLoadingFromWeb = false;
        //private Action jobCallback;
        
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

        public bool IsLoadingFromWeb
        {
            get { return isLoadingFromWeb; }
            set
            {
                isLoadingFromWeb = value;
                GalaSoft.MvvmLight.Threading.DispatcherHelper.UIDispatcher.BeginInvoke(
                    delegate
                    {
                        NotifyPropertyChanged("IsLoadingFromWeb");
                    });
            }
        }

        public ObservableCollection<TrendingItemViewModel> TrendingItems { get; set; }

        public PanoramaTrendingViewModel()
        {
            TrendingItems = new ObservableCollection<TrendingItemViewModel>();
            if (TweetaBaseVM.IsDesignMode)
            {

                for (int i = 0; i < 10; i++)
                {
                    TrendingItems.Add(Helpers.DummyDataHelper.CreateTrend());
                }
            }
            //else
            //{
                
            //   this.SetDataAction(DoRefresh);
            //}
        }

        private void RefreshInternal()
        {
            SetIsLoading(true);
            
            if (hasLoadedCache)
                InternalNewGetTimeline();
            else
                LoadFromCache();
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
            //HeavyDutyJobManager.AddJob(RefreshInternal);
        }

        bool hasLoadedCache;
        private void LoadFromCache()
        {
            hasLoadedCache = true;
            InternalNewGetTimeline();
        }

        private void InternalNewGetTimeline()
        {
            IsLoadingFromWeb = true;

            TwitterInterface.GetTrendingTopics(GetTrendsResponse);
        }
        
        private void DispatchAddingTrends(List<TrendingItemViewModel> list)
        {

            GalaSoft.MvvmLight.Threading.DispatcherHelper.UIDispatcher.BeginInvoke(
                    () =>
                    {
                        IsLoading = false;
                        
                        this.TrendingItems.Clear();

                        if (this.TrendingItems.Count > 0)
                        {
                            for (int i = list.Count-1; i >= 0; i--)
                            {
                                this.TrendingItems.Insert(0, list[i]);
                            }
                        }
                        else
                        {
                            foreach (var item in list)
                            {
                                this.TrendingItems.Add(item);
                            }
                        }

                        var lst = new List<TrendingItemViewModel>();
                        foreach (var item in TrendingItems)
                        {
                            lst.Add(item);
                        }
                        SerializeTrends(lst);
                    });
        }

        private void GetTrendsResponse(TwitterTrends trends, TwitterResponse response)
        {
            if (TwitterInterface.CheckStatusCode(response))
            {
                List<TrendingItemViewModel> localTrends = new List<TrendingItemViewModel>();
                foreach (var item in trends)
                {
                    localTrends.Add(TrendingItemViewModel.FromTrend(item));
                }

                DispatchAddingTrends(localTrends);
            }

            IsLoadingFromWeb = false;
        }
   
        string dataFilename = "trendingData.xml";
        private void SerializeTrends(List<TrendingItemViewModel> localTrends)
        {
            SerializationManager.SerializeData(dataFilename, localTrends);
        }


        internal void AddItem(TwitterStatus item)
        {
            string tagFound = string.Empty;
            foreach (var tag in item.Entities.HashTags)
            {
                if (App.AppSettings.SearchTerms.Contains(tag.Text))
                {
                    tagFound = tag.Text;
                    break;
                }
            }
            if (!string.IsNullOrEmpty(tagFound))
            {
                GalaSoft.MvvmLight.Threading.DispatcherHelper.UIDispatcher.BeginInvoke(
                    delegate
                    {
                        var it = TrendingItemViewModel.FromTrend(item);
                        this.TrendingItems.Add(it);
                    });
            }
        }
    }
}
