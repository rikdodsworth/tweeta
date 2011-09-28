using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls.Primitives;
using Microsoft.Phone.Controls;
using System.Diagnostics;

namespace Tweeta.CustomControl
{
    public class LoadingPanoramaItem : PanoramaItem
    {
        public static readonly DependencyProperty LoadingTextProperty = DependencyProperty.Register(
            "LoadingText",          //Name
            typeof(string),       //Type
            typeof(LoadingPanoramaItem),     //Owner of the Property
            new PropertyMetadata("", new PropertyChangedCallback(OnLoadingTextPropertyChanged))

        );
        private static void OnLoadingTextPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) { }
        public LoadingPanoramaItem()
        {
            
        }
        public string LoadingText
        {
            get
            {
                return GetValue(LoadingTextProperty).ToString();
            }
            set
            {
                SetValue(LoadingTextProperty, value);
            }
        }



        public static readonly DependencyProperty LoadingVisibilityProperty = DependencyProperty.Register(
            "LoadingVisibility",          //Name
            typeof(Visibility),       //Type
            typeof(LoadingPanoramaItem),     //Owner of the Property
            new PropertyMetadata(Visibility.Collapsed, new PropertyChangedCallback(OnLoadingVisibilityPropertyChanged))

        );
        private static void OnLoadingVisibilityPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) 
        {
            ((LoadingPanoramaItem)d).LoadingVisibility = (Visibility)e.NewValue;
            Debug.WriteLine("");
        }

        public Visibility LoadingVisibility
        {
            get
            {
                return (Visibility)GetValue(LoadingVisibilityProperty);
            }
            set
            {
                SetValue(LoadingVisibilityProperty, value);
                if(panoLoadingText!=null) panoLoadingText.Visibility = value;
            }
        }

        public override void OnApplyTemplate()
        {
            var item = GetChildsRecursive(this, "txtPanoLoading");
            if (item != null)
            {
                panoLoadingText = (TextBlock)item;
                panoLoadingText.Visibility = LoadingVisibility;
            }
            base.OnApplyTemplate();
        }
        TextBlock panoLoadingText;
        DependencyObject GetChildsRecursive(DependencyObject root, string name)
        {
            if (root == null)
                return null;

            if (VisualTreeHelper.GetChildrenCount(root) == 0 && root is ContentControl)
            {
                return GetChildsRecursive(((ContentControl)root).Content as DependencyObject, name);
            }
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(root); i++)
            {
                var item = VisualTreeHelper.GetChild(root, i);
                FrameworkElement el = item as FrameworkElement;
                if (el.Name == name)
                {
                    return item;
                }
                var child = GetChildsRecursive(item, name);
                if (child != null) return child;
            }
            return null;
        }

    }
}
