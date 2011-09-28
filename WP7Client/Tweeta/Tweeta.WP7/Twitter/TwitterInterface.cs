using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Tweeta.Common;
using Tweeta.Common.Logging;
using TweetSharp;
using System.Net;

namespace Tweeta.Common.Twitter
{
    public class TwitterAuthWrapper
    {
        public Action<bool> mainCallback;
        public Action<string, Action> RequestAuth;
    }

    public class TwitterInterface
    {
        public const string MESSAGE_AUTHENTICATION_GOT_TOKEN = "GOT_TOKEN";

        public static bool HasSearches { get; set; }

        public static TwitterService Service { get { return service; } }

        private static TwitterAuthWrapper wrapper;
        private static OAuthRequestToken requestToken;
        private static OAuthAccessToken accessToken;
        private static TwitterService service;

        private static string username;
        private static string password;
        private static Action<bool> loginCallback;

        public static void GetUserTimeline(Action<IEnumerable<TwitterStatus>, TwitterResponse> callback, long sinceID)
        {
            Log.Info("GetUsertimeline " + Thread.CurrentThread.ManagedThreadId);

            if (sinceID > -1)
            {
                ValidateServiceSetup();

                service.ListTweetsOnHomeTimelineSince(sinceID, 1, App.AppSettings.PageCount, callback);
            }
            else
                GetUserTimeline(callback);
        }

        public static void GetSpecifiedUserTimeline(Action<IEnumerable<TwitterStatus>, TwitterResponse> callback, int userid)
        {
            Log.Info("GetUsertimeline " + Thread.CurrentThread.ManagedThreadId);
            ValidateServiceSetup();

            service.ListTweetsOnSpecifiedUserTimeline(userid, callback);
        }

        public static void DoSearch(string query, Action<TwitterSearchResult, TwitterResponse> callback)
        {
            Log.Info("DoSearch " + Thread.CurrentThread.ManagedThreadId);
            ValidateServiceSetup();
            service.Search(query, 1, App.AppSettings.PageCount, callback);
        }

        public static void GetTrendingTopics(Action<TwitterTrends, TwitterResponse> callback)
        {
            Log.Info("GetTrending" + Thread.CurrentThread.ManagedThreadId);
            ValidateServiceSetup();

            service.ListCurrentTrends(callback);
        }

        public static void GetUserTimeline(Action<IEnumerable<TwitterStatus>, TwitterResponse> callback)
        {
            Log.Info("GetUserTimeline" + Thread.CurrentThread.ManagedThreadId);
            ValidateServiceSetup();

            service.ListTweetsOnHomeTimeline(0, App.AppSettings.PageCount, callback);
        }

        public static void GetMentions(Action<IEnumerable<TwitterStatus>, TwitterResponse> callback)
        {
            Log.Info("GetMentions" + Thread.CurrentThread.ManagedThreadId);
            ValidateServiceSetup();

            service.ListTweetsMentioningMe(0, App.AppSettings.PageCount, callback);
        }

        public static void Authenticate(string username, string password, Action<bool> callback, Action<string, Action> auth)
        {
            ValidateServiceSetup();
            wrapper = new TwitterAuthWrapper();
            wrapper.mainCallback = callback;
            wrapper.RequestAuth = auth;
            service.GetRequestToken(HandleAuthenticationRequestToken);
        }

        public static void Authenticate(string uname, string pwd, Action<bool> callback)
        {
            username = uname;
            password = pwd;
            loginCallback = callback;
            ValidateServiceSetup();
            service.GetRequestToken(HandleAuthenticationRequestToken);
        }

        public static void GetAccessToken(string code, Action callback)
        {
            service.GetAccessToken(requestToken, code,
                (a, b) =>
                {
                    HandleAuthenticationAccessToken(a, b);
                    callback();
                });
        }

        public static void PostTweet(string message, Action<TwitterStatus> callback)
        {
            service.SendTweet(message, delegate(TwitterStatus st, TwitterResponse r)
            {
                if (TwitterInterface.CheckStatusCode(r))
                    callback(st);
                else
                    callback(null);
            });
        }

        public static void GetUser(int id, Action<TwitterUser> callback)
        {
            ValidateServiceSetup();
            service.GetUserProfileFor(id,
                delegate(TwitterUser user, TwitterResponse res)
                {
                    if (TwitterInterface.CheckStatusCode(res))
                    {
                        if (user.ProfileImageUrl.Contains("_normal"))
                            user.ProfileImageUrl = user.ProfileImageUrl.Replace("_normal", "");
                        // service.GetProfileImageFor(user.ScreenName, TwitterProfileImageSize.Bigger,
                        callback(user);
                    }
                });
        }

        public static void GetSavedSearch(Action<IEnumerable<TwitterSavedSearch>> callback)
        {
            try
            {
                service.ListSavedSearches(delegate(IEnumerable<TwitterSavedSearch> srch, TwitterResponse res)
                {
                    if (TwitterInterface.CheckStatusCode(res))
                    {
                        if (srch != null)
                        {
                            foreach (var item in srch)
                            {
                                HasSearches = true;
                                break;
                            }
                        }
                        callback(srch);
                    }
                    else
                        callback(null);
                });
            }
            catch (Exception err)
            {
                Debug.WriteLine(err.ToString());
            }
        }
        public static bool CheckStatusCode(TwitterResponse response)
        {
            return response.StatusCode == HttpStatusCode.OK;
        }
        private static void HandleAuthenticationRequestToken(OAuthRequestToken token, TwitterResponse response)
        {
            if (token == null)
            {
                loginCallback(false);
                return;
            }

            requestToken = token;

            service.GetAccessTokenWithXAuth(username, password, HandleAuthenticationAccessToken);
        }

        private static void ValidateServiceSetup()
        {
            if (service == null)
            {
                service = new TwitterService(Common.Settings.CONSUMER_KEY, Common.Settings.CONSUMER_SECRET);
                //TODO: force login if no settings
                if (App.AppSettings.HasDetails)
                {
                    service.AuthenticateWith(App.AppSettings.AccessToken, App.AppSettings.AccessTokenSecret);
                }
            }
        }

        private static void HandleAuthenticationAccessToken(OAuthAccessToken token, TwitterResponse response)
        {
            if (TwitterInterface.CheckStatusCode(response))
            {
                try
                {
                    accessToken = token;
                    App.AppSettings.AccessToken = accessToken.Token;
                    App.AppSettings.AccessTokenSecret = token.TokenSecret;
                    App.AppSettings.AccountID = token.UserId.ToString();
                    App.AppSettings.AccountName = token.ScreenName;

                    App.AppSettings.HasAuthenticated = true;
                    App.AppSettings.Save();

                    service.AuthenticateWith(App.AppSettings.AccessToken, App.AppSettings.AccessTokenSecret);

                    loginCallback(true);
                }
                catch (Exception er)
                {
                    loginCallback(false);
                }
            }
            else
                loginCallback(false);
        }
    }
}
