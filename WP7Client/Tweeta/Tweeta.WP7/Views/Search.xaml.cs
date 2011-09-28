using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using Tweeta.ViewModel;
using Microsoft.Phone.Shell;

namespace Tweeta.Views
{
    public partial class Search : PhoneApplicationPage
    {
        public Search()
        {
            InitializeComponent();
            this.DataContext = new SearchResultsViewModel();
        }
        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            this.afterSearch.Visibility = System.Windows.Visibility.Collapsed;
            this.beforeSearch.Visibility = System.Windows.Visibility.Visible;
            CheckAppBarState(e.Uri.ToString());
            if (NavigationContext.QueryString.ContainsKey("searchTerm"))
            {
                string term = NavigationContext.QueryString["searchTerm"].ToString();
                txtSearch.Text = term;
                DoSearch();
            }
            else
            {
                ((SearchResultsViewModel)this.DataContext).RefreshTerms();
            }
            base.OnNavigatedTo(e);
        }

        private void CheckAppBarState(string uri)
        {
            foreach (var item in ShellTile.ActiveTiles)
            {
               
                if (item.NavigationUri.ToString().Contains(uri))
                {
                    ApplicationBar.Buttons.RemoveAt(2);
                }
            }
        }
        private void TextBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (txtSearch.Text != string.Empty)
                {
                    this.Focus();
                    DoSearch();
                }
            }
        }

        private void DoSearch()
        {
            SearchResultsViewModel vm = this.DataContext as SearchResultsViewModel;
            vm.DoSearch(txtSearch.Text);

            beforeSearch.Visibility = System.Windows.Visibility.Collapsed;
            afterSearch.Visibility = System.Windows.Visibility.Visible;
            //gridSearch.Visibility = Visibility.Collapsed;
        }

        private void appBarBtnSearchClick(object sender, EventArgs e)
        {
            gridSearch.Visibility = Visibility.Visible;
            txtSearch.Focus();
        }

        private void appBarBtnSaveTileClicked(object sender, EventArgs e)
        {

             SearchResultsViewModel vm = this.DataContext as SearchResultsViewModel;
           
            foreach (var item in ShellTile.ActiveTiles)
            {

                if (item.NavigationUri.ToString().Contains(vm.SearchTerm))
                {
                    MessageBox.Show("You have already saved this search term as a tile", "Already saved", MessageBoxButton.OK);
                }
            }
 
            string term = vm.SearchTerm;
            if (!term.StartsWith("#"))
                term = "#" + term;

            StandardTileData tile = new StandardTileData()
            {
                Title = term,
                BackContent = vm.Items[0].Message,
                BackTitle = term,
            };

            if (vm.Items.Count > 0)
            {
                tile.BackgroundImage = new Uri(vm.Items[0].ProfilePhotoURL, UriKind.Absolute);
                tile.Title = vm.Items[0].Username;
            }
            ShellTile.Create(new Uri("/Views/Search.xaml?searchTerm=" + vm.SearchTerm, UriKind.Relative), tile);
        }
    }
}