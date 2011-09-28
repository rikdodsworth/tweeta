using System.Collections.ObjectModel;

namespace WP7HelperFX.MVVM
{
    public class ItemsViewModel<T> : BaseViewModel, IRefreshable
    {
        public ObservableCollection<T> Items
        {
            get { return items; }
        }

        private ObservableCollection<T> items = new ObservableCollection<T>();

        public virtual void Refresh()
        {
        }
    }
}
