using System.Collections.Generic;
using System.IO.IsolatedStorage;

namespace Tweeta.Common
{
    public class Settings
    {
        public static string CONSUMER_SECRET = "{CONSUMERSECRET}";
        public static string CONSUMER_KEY = "{CONSUMERKEY}";

        public bool HasDetails 
        {
            get
            {
                return !string.IsNullOrEmpty(AccessToken) &&
                    !string.IsNullOrEmpty(AccessTokenSecret) &&
                    !string.IsNullOrEmpty(AccountName) &&
                    !string.IsNullOrEmpty(AccountID) ;
            }
        }

        public int PageCount { get; set; }
        
        public string AccessToken { get; set; }
        
        public string AccessTokenSecret { get; set; }
        
        public string AccountName { get; set; }
        
        public string AccountID { get; set; }
        
        public string AccountBGUrl { get; set; }

        public List<string> SearchTerms { get; set; }

        public bool HasAuthenticated { get; set; }

        public Settings()
        {
            SearchTerms = new List<string>();
        }

        public void Load()
        {
            var hasAuthenticated = LoadSetting<bool>("HasAuth", false);
            HasAuthenticated = hasAuthenticated;

            if (hasAuthenticated)
                LoadOtherSettings();
            else
                PageCount = 10;    
        }

        private void LoadOtherSettings()
        {
            AccessToken = IsolatedStorageSettings.ApplicationSettings["AccessToken"].ToString();
            AccessTokenSecret = IsolatedStorageSettings.ApplicationSettings["AccessTokenSecret"].ToString();
            AccountID = IsolatedStorageSettings.ApplicationSettings["AccountID"].ToString();
            AccountName = IsolatedStorageSettings.ApplicationSettings["AccountName"].ToString();
            AccountBGUrl = IsolatedStorageSettings.ApplicationSettings["AccountBGUrl"].ToString();
            SearchTerms = LoadSetting<List<string>>("SearchTerms", new List<string>());


            PageCount = (int)IsolatedStorageSettings.ApplicationSettings["PageCount"];
        }

        private T LoadSetting<T>(string name, T def)
        {
            if (IsolatedStorageSettings.ApplicationSettings.Contains(name))
                return (T)IsolatedStorageSettings.ApplicationSettings[name];

            return def;
        }

        public void Save()
        {
            if (string.IsNullOrEmpty(this.AccountName))
                return;

            IsolatedStorageSettings.ApplicationSettings["AccessToken"] = AccessToken;
            IsolatedStorageSettings.ApplicationSettings["AccessTokenSecret"] = AccessTokenSecret;
            IsolatedStorageSettings.ApplicationSettings["AccountID"] = AccountID;
            IsolatedStorageSettings.ApplicationSettings["AccountName"] = AccountName;
            IsolatedStorageSettings.ApplicationSettings["AccountBGUrl"] = AccountBGUrl;
            IsolatedStorageSettings.ApplicationSettings["PageCount"] = PageCount;
            IsolatedStorageSettings.ApplicationSettings["HasAuth"] = HasDetails;
            IsolatedStorageSettings.ApplicationSettings.Save();
        }
    }
}
