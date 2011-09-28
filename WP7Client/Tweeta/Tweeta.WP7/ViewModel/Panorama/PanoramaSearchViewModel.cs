using System;
using System.Collections.Generic;
using GalaSoft.MvvmLight.Threading;
using Tweeta.Common;
using TweetSharp;
using Tweeta.Common.Twitter;
namespace Tweeta.ViewModel
{
    public class PanoramaSearchViewModel : TweetaItemsVM<TweetViewModel>
    {
        public PanoramaSearchViewModel()
        {

            if (TweetaBaseVM.IsDesignMode)
            {
                for (int i = 0; i < 10; i++)
                {
                    this.Items.Add(Helpers.DummyDataHelper.CreateTweet());
                }
            }
            //else
            //{
            //    this.Refresh();
            //    this.SetDataAction(DoRefresh);
            //}
        }

        public override void Refresh()
        {
            if (TweetaBaseVM.IsDesignMode)
                return;

            this.IsLoading = true;
            TweetGetter.GetCombinedSearchResults(delegate(IEnumerable<ITwitterModel> data)
            {
                if (data != null)
                {
                    DispatcherHelper.CheckBeginInvokeOnUI(delegate
                    {
                        this.IsLoading = false;
                        foreach (var item in data)
                        {
                            var newItem = TweetViewModel.FromTwitterStatus(item as TwitterSearchStatus);
                            
                            this.Items.Add(newItem);
                        }
                    });
                }
                else
                {
                    DispatcherHelper.CheckBeginInvokeOnUI(delegate
                    {
                        this.IsLoading = false;
                    });
                }
                
            },
            -1);
            
        }
    }
}
