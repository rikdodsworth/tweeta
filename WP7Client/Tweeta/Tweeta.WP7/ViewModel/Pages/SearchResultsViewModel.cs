using System.Collections.Generic;
using System.Collections.ObjectModel;
using GalaSoft.MvvmLight.Threading;
using Tweeta.Common.Twitter;
using TweetSharp;

namespace Tweeta.ViewModel
{
    public class SearchResultsViewModel : TweetaBaseVM
    {
        public SearchResultsViewModel()
        {
            if (TweetaBaseVM.IsDesignMode)
            {
                for (int i = 0; i < 10; i++)
                {
                    Items.Add(Helpers.DummyDataHelper.CreateTweet());
                }

                for (int i = 0; i < 3; i++)
                {
                    SearchTerms.Add(new ViewModel.SearchTerm() { Term = Helpers.DummyDataHelper.GetSearchTerm() });
                }
                searchTerm = "#searchTerm";
            }
            
        }
        
        private string searchTerm = string.Empty;
        public string SearchTerm
        {
            get { return searchTerm; }
            set
            {
                searchTerm = value;
                NotifyPropertyChanged("SearchTerm");
            }
        }

        private ObservableCollection<TweetViewModel> items = new ObservableCollection<TweetViewModel>();
        public ObservableCollection<TweetViewModel> Items
        {
            get { return items; }
        }

        private ObservableCollection<SearchTerm> searchTerms = new ObservableCollection<SearchTerm>();
        public ObservableCollection<SearchTerm> SearchTerms
        {
            get { return searchTerms; }
        }

        internal void DoSearch(string searchTerm)
        {
            this.IsBusy = true;
            this.searchTerm = searchTerm;
            this.Items.Clear();
            TwitterInterface.DoSearch(searchTerm, (a, b) =>    
                {
                    GalaSoft.MvvmLight.Threading.DispatcherHelper.CheckBeginInvokeOnUI(delegate
                    {
                        this.IsBusy = false;
                    });
                    if (TwitterInterface.CheckStatusCode(b))
                    {
                        GalaSoft.MvvmLight.Threading.DispatcherHelper.CheckBeginInvokeOnUI(delegate
                        {
                            foreach (var item in a.Statuses)
                            {
                                this.Items.Add(TweetViewModel.FromTwitterStatus(item));
                            }
                        });
                    }

                });
        }

        internal void RefreshTerms()
        {
            TwitterInterface.GetSavedSearch(delegate(IEnumerable<TwitterSavedSearch> srch)
            {
                if (srch != null)
                {
                    DispatcherHelper.CheckBeginInvokeOnUI(delegate
                    {
                        //List<string> searchTerms = new List<string>();
                        foreach (var item in srch)
                        {
                            this.searchTerms.Add(new SearchTerm()
                            {
                                Term = item.Name
                            });
                        }
                    });

                }
            });
        }
    }

    public class SearchTerm : TweetaBaseVM
    {
        private string term;

        public string Term
        {
            get
            {
                return term;
            }
            set
            {
                if (term != value)
                {
                    term = value;
                    NotifyPropertyChanged("Term");
                }
            }
        }
    }
}
