using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using WP7HelperFX.MVVM;

namespace WP7HelperFX
{
    public class GlobalLoading : NotifyPropertyChangedObject
    {
        public static GlobalLoading Instance
        {
            get
            {
                if (instance == null)
                    instance = new GlobalLoading();

                return instance;
            }
        }
        
        public bool IsDataManagerLoading { get; set; }

        public bool ActualIsLoading
        {
            get { return IsLoading || IsDataManagerLoading; }
        }

        public bool IsLoading
        {
            get { return loadingCount > 0; }

            set
            {
                bool loading = IsLoading;
            
                if (value)
                    ++loadingCount;
                else
                    --loadingCount;

                NotifyValueChanged();
            }
        }

        private void NotifyValueChanged()
        {
            mangoIndicator.IsIndeterminate = loadingCount > 0 || IsDataManagerLoading;

            // for now, just make sure it's always visible.
            if (mangoIndicator.IsVisible == false)
                mangoIndicator.IsVisible = true;

            if (panoBar != null)
                panoBar.IsIndeterminate = loadingCount > 0 || IsDataManagerLoading;
        }

        private static GlobalLoading instance;

        private ProgressIndicator mangoIndicator;
        private PerformanceProgressBar panoBar;
        private int loadingCount;

        private GlobalLoading() { } 

        public void Initialize(PhoneApplicationFrame frame)
        {
            mangoIndicator = new ProgressIndicator();

            frame.Navigated += OnRootFrameNavigated;
            frame.Navigating += new NavigatingCancelEventHandler(Frame_Navigating);
        }

        private void Frame_Navigating(object sender, NavigatingCancelEventArgs e)
        {
            if (panoBar != null)
            {
                ((Grid)panoBar.Parent).Children.Remove(panoBar);
                panoBar = null;
            }
        }
        
        private void OnRootFrameNavigated(object sender, NavigationEventArgs e)
        {
            // Use in Mango to share a single progress indicator instance.
            PhoneApplicationPage page = e.Content as PhoneApplicationPage;

            if (page != null)
            {
                page.SetValue(SystemTray.ProgressIndicatorProperty, mangoIndicator);
                
                var root = FindChildByType<Panorama>(e.Content as DependencyObject);

                if (root != null)
                {
                    panoBar= new PerformanceProgressBar();
                    panoBar.IsIndeterminate = false;
                    panoBar.VerticalAlignment = VerticalAlignment.Top;
                    panoBar.HorizontalAlignment = HorizontalAlignment.Stretch;

                    FrameworkElement el = ((Panorama)root).Parent as FrameworkElement;
                    if (el != null)
                    {
                        ((Grid)el).Children.Add(panoBar);
                    }
                }
            }
        }

        private DependencyObject FindChildByType<T>(DependencyObject root)
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(root); i++)
            {
                DependencyObject item = VisualTreeHelper.GetChild(root, i);
                
                FrameworkElement element = item as FrameworkElement;

                if (element.GetType() == typeof(T))
                    return item;

                DependencyObject child = FindChildByType<T>(item);
                
                if (child != null) 
                    return child;
            }
            return null;
        }

        private void OnDataManagerPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if ("IsLoading" == e.PropertyName)
            {
                // if AgFx: IsDataManagerLoading = DataManager.Current.IsLoading;
                NotifyValueChanged();
            }
        }
    }
}