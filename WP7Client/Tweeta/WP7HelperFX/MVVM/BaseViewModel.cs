using System;
using System.ComponentModel;

namespace WP7HelperFX.MVVM
{
    public class BaseViewModel : NotifyPropertyChangedObject
    {
        public bool IsBusy
        {
            get { return isBusy; }
            set
            {
                isBusy = value;
                GlobalLoading.Instance.IsLoading = isBusy;
                NotifyPropertyChanged("IsBusy");
            }
        }

        public static bool IsDesignMode
        {
            get { return isInDesignMode; }
        }

        private static bool isInDesignMode;

        private bool isBusy;

        static BaseViewModel()
        {
            isInDesignMode = DesignerProperties.IsInDesignTool;
        }
    }
}
