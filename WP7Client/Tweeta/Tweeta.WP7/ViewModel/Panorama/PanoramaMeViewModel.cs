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
    public class PanoramaMeViewModel : TweetaItemsVM<TrendingItemViewModel>
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
        private TwitterUserViewModel userVM;


        public TwitterUserViewModel UserVM
        {
            get { return userVM; }
            set
            {
                userVM = value;
                NotifyPropertyChanged("UserVM");
            }
        }

        public PanoramaMeViewModel()
        {

            if (TweetaBaseVM.IsDesignMode)
            {
                UserVM = Helpers.DummyDataHelper.CreateUser();
            }
            else
            {
                
                this.UserVM = new TwitterUserViewModel();
                // this.SetDataAction(DoRefresh);
            }
        }

        private void RefreshInternal()
        {
            this.UserVM.Username = App.AppSettings.AccountName;
            //this.UserVM.ProfileURL = App.AppSettings.AccountBGUrl;

            SetIsLoading(true);
            
            if (hasLoadedCache)
                InternalGetNewMeData();
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
            InternalGetNewMeData();
        }

        private void InternalGetNewMeData()
        {
            
            if (!string.IsNullOrEmpty(App.AppSettings.AccountID))
            {
                IsLoadingFromWeb = true;
                TwitterInterface.GetUser(int.Parse(App.AppSettings.AccountID), GetMeResponse);
            }
        }
        
        private void GetMeResponse(TwitterUser user)
        {
            if (user != null)
            {
                
                GalaSoft.MvvmLight.Threading.DispatcherHelper.CheckBeginInvokeOnUI(
                    delegate                     
                    {
                        SetIsLoading(false);
                        IsLoadingFromWeb = false;
                        this.UserVM.SetUser(user);
                      
                    });
            }
        }
   
        string dataFilename = "trendingData.xml";
        private void SerializeTrends(List<TrendingItemViewModel> localTrends)
        {
            SerializationManager.SerializeData(dataFilename, localTrends);
        }


    }
}
