
using Tweeta.ViewModel;
using System.Collections.Generic;
using System;
using WP7HelperFX;
using TweetSharp;
using Tweeta.Common;
using GalaSoft.MvvmLight.Threading;
using Tweeta.Common.Twitter;
namespace Tweeta.Services.Data
{
    public interface ITimelineDataService : IAppService
    {
        void GetData(Action<bool, List<TweetViewModel>> callback, long? sinceID );
    }

    public class TimelineDataService : ITimelineDataService
    {
        Action<bool, List<TweetViewModel>> Callback;
        public void GetData(Action<bool, List<TweetViewModel>> callback, long? sinceID)
        {
            Callback = callback;
            if (sinceID.HasValue)
            {
                TwitterInterface.GetUserTimeline(GetTweetsResponse, sinceID.Value);
            }
            else
            {
                TwitterInterface.GetUserTimeline(GetTweetsResponse);
            }

        }
        private void GetTweetsResponse(IEnumerable<TwitterStatus> tweets, TwitterResponse response)
        {
            if (TwitterInterface.CheckStatusCode(response))
            {
                List<TweetViewModel> localTweets = new List<TweetViewModel>();
                foreach (var item in tweets)
                {
                    localTweets.Add(TweetViewModel.FromTwitterStatus(item));
                }
                DispatcherHelper.UIDispatcher.BeginInvoke(
                    () =>
                    {
                        Callback(true, localTweets);
                    });
            }
            else
            {
                Callback(false, null);
            }


        }
    }
}
