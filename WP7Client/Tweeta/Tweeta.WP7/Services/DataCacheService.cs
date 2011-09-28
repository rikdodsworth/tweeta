using System.Collections.Generic;
using Tweeta.ViewModel;
using WP7HelperFX;

namespace Tweeta.Services
{
    public class DataCacheService : IAppService
    {

        public void SaveToCache(string filename, List<TweetViewModel> data)
        {
            SerializationManager.SerializeData(filename, data);
        }
    }
}
