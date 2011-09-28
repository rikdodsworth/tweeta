using Tweeta.Resources.Strings;
using WP7HelperFX.MVVM;

namespace Tweeta.ViewModel
{
    public class TweetaBaseVM : BaseViewModel
    {
        public StringResources AppResources { get { return appResources; } }

        internal static StringResources appResources = new StringResources();
    }
}
