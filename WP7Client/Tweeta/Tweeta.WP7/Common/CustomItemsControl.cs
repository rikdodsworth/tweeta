using System.Windows;
using System.Windows.Controls;

namespace Tweeta.Common
{
    public class CustomItemsControl : ItemsControl
    {
        public static readonly DependencyProperty HasItemsProperty = DependencyProperty.Register(
            "HasItems",
            typeof(bool),
            typeof(CustomItemsControl),
            new PropertyMetadata(false, new PropertyChangedCallback(OnHasItemsPropertyChanged)));

        private static void OnHasItemsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) { }

        public bool HasItems
        {
            get 
            { 
                if(this.Items == null)
                    return false;

                return this.Items.Count > 0;
            }
        }
        
    }
}
