using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Tweeta.Common.Logging;
using TweetSharp;

namespace Tweeta.Common.Twitter
{
    public class TweetGetter
    {
        public static void GetTrendingTopics(Action<IEnumerable<ITwitterModel>> callback, long newestID)
        {
            TwitterInterface.GetTrendingTopics(
                (TwitterTrends trends, TwitterResponse response) =>
                {
                    Log.Info("Get Trending response " + Thread.CurrentThread.ManagedThreadId);
                    if (TwitterInterface.CheckStatusCode(response))
                    {
                        List<ITwitterModel> data = new List<ITwitterModel>();
                        
                        foreach (var item in trends)
                            data.Add(item);

                        callback(data);
                    }
                });
        }

        public static void GetUserTweets(Action<IEnumerable<ITwitterModel>> callback, int userid)
        {
            TwitterInterface.GetSpecifiedUserTimeline(
                (IEnumerable<TwitterStatus> statuses, TwitterResponse response) =>
                {
                    Log.Info("Get User timeline response " + Thread.CurrentThread.ManagedThreadId);
                    
                    if (TwitterInterface.CheckStatusCode(response))
                    {
                        List<ITwitterModel> data = new List<ITwitterModel>();
                        
                        foreach (TwitterStatus status in statuses)
                            data.Add(status);

                        callback(data);
                    }
                }, userid);
        }

        public static void GetUserTimeline(Action<IEnumerable<ITwitterModel>> callback, long newestID)
        {
            TwitterInterface.GetUserTimeline(
                (IEnumerable<TwitterStatus> statuses, TwitterResponse response) =>
                {
                    Log.Info("Get User timeline response " + Thread.CurrentThread.ManagedThreadId);

                    if (TwitterInterface.CheckStatusCode(response))
                    {
                        List<ITwitterModel> data = new List<ITwitterModel>();
                    
                        foreach (var item in statuses)
                            data.Add(item);

                        callback(data);
                    }
                }, newestID);
        }

        public static void GetCombinedSearchResults(Action<IEnumerable<ITwitterModel>> callback, long newestID)
        {
            TwitterInterface.GetSavedSearch(
                (IEnumerable<TwitterSavedSearch> srch) =>
                {
                    if (srch != null)
                    {
                        List<string> searchTerms = new List<string>();
                        
                        foreach (var item in srch)
                            searchTerms.Add(item.Name);

                        SearchAll(searchTerms, callback);
                    }
                    else
                    {
                        callback(null);
                    }
                });
            
        }
        
        public static void GetMentions(Action<IEnumerable<ITwitterModel>> callback, long newestID)
        {
            TwitterInterface.GetMentions(
                (IEnumerable<TwitterStatus> statuses, TwitterResponse response) =>
                    {
                        Log.Info("Get Mentions response " + Thread.CurrentThread.ManagedThreadId);
                        if (TwitterInterface.CheckStatusCode(response))
                        {
                            List<ITwitterModel> data = new List<ITwitterModel>();
                            
                            foreach (var item in statuses)
                                data.Add(item);

                            callback(data);
                        }
                    });
        }

        private static void SearchAll(List<string> searchTerms, Action<IEnumerable<ITwitterModel>> callback)
        {
            List<ITwitterModel> list = new List<ITwitterModel>();
            bool complete = false;
            foreach (var item in searchTerms)
            {
                try
                {
                    TwitterInterface.DoSearch(item,
                        (TwitterSearchResult result, TwitterResponse response) =>
                        {
                            if (TwitterInterface.CheckStatusCode(response))
                            {
                                foreach (var itm in result.Statuses)
                                    list.Add(itm);
                            }

                            complete = true;
                        }
                    );
                }
                catch (Exception err)
                {
                    Debug.WriteLine(err.ToString());
                }

                while (!complete)
                    Thread.Sleep(10);

                complete = false;
            }

            list.Sort(CompareTweet);
            callback(list);
        }

        private static int CompareTweet(ITwitterModel a, ITwitterModel b)
        {
            TwitterSearchStatus aS = a as TwitterSearchStatus;
            TwitterSearchStatus bS = b as TwitterSearchStatus;

            return aS.CreatedDate.CompareTo(bS.CreatedDate);
        }
    }
}
