using System;

using GalaSoft.MvvmLight;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Tweeta.Common;
using System.Collections;
using TweetSharp;
using System.Collections.Generic;
using System.Windows.Controls;
using GalaSoft.MvvmLight.Messaging;
using System.Xml.Serialization;
using GalaSoft.MvvmLight.Threading;
using System.Threading;
using System.Diagnostics;
using WP7HelperFX;
using Tweeta.Common.Logging;
using Tweeta.Common.Twitter;

namespace Tweeta.ViewModel
{
    public class PanoramaPageViewModel : ViewModelBase
    {
        private bool isBusy;
        public bool IsBusy
        {
            get { return isBusy; }
            set
            {
                isBusy = value;
                RaisePropertyChanged("IsBusy");

                GlobalLoading.Instance.IsLoading = isBusy;
            }
        }

        int busyCount;
        public void AddBusy()
        {
            lock (busyLock)
            {
                busyCount++;
                DispatcherHelper.CheckBeginInvokeOnUI(delegate
                {
                    IsBusy = true;
                });
            }
        }
        object busyLock = new object();
        public void RemoveBusy()
        {
            lock (busyLock)
            {
                busyCount--;
                if (busyCount == 0)
                {
                    DispatcherHelper.CheckBeginInvokeOnUI(delegate
                    {
                        IsBusy = false;
                    });
                }
            }
        }
        public PanoramaPageViewModel()
        {
            if (IsInDesignMode)
            {
                AppTitle = "Tweeta";



            }
        }
        private string appTitle;
        public string AppTitle
        {
            get { return appTitle; }
            set
            {
                appTitle = value;
                RaisePropertyChanged("AppTitle");
            }
        }
        private ObservableCollection<PanoramaItemViewModel> panels = new ObservableCollection<PanoramaItemViewModel>();
        public ObservableCollection<PanoramaItemViewModel> Panels
        {
            get { return panels; }
        }

       

        public const string MESSAGE_REFRESH_TIMELINE = "REFRESH_TIMELINE";
        private void GotMessage(NotificationMessage msg)
        {
            if (false) //msg.Notification == MainAppPage.MESSAGE_BUSY_ON)
            {
                //ViewModelLocator.Instance.PanoramaPage.AddBusy();
            }
            else if (false) //msg.Notification == MainAppPage.MESSAGE_BUSY_OFF)
            {
                //ViewModelLocator.Instance.PanoramaPage.RemoveBusy();
            }
            else if (msg.Notification == MESSAGE_REFRESH_TIMELINE)
            {
                foreach (var item in this.panels)
                {
                    if (item.Header.Title == "timeline")
                    {
                        item.Refresh();
                    }
                }
            }
        }
       
       
    }
    
    public class GenericListItem : ViewModelBase
    {

    }
    
    [XmlInclude(typeof(TweetViewModel))]
    [XmlInclude(typeof(TrendingItemViewModel))]
    [XmlInclude(typeof(CoolWallItemViewModel))]
    public class PanoramaItemViewModel : ViewModelBase
    {
        public PanoramaItemViewModel()
        {
            header = new PanoramaItemHeaderViewModel(this);
            items = new ObservableCollection<GenericListItem>();
            this.MaxDisplayCount = 30;
            this.MaxCacheCount = 30;
        }
        private PanoramaItemHeaderViewModel header;
        public PanoramaItemHeaderViewModel Header
        {
            get { return header; }
        }

        private Orientation orientation = Orientation.Vertical;
        public Orientation Orientation
        {
            get { return orientation; }
            set
            {
                orientation = value;
                RaisePropertyChanged("Orientation");
            }
        }

        public int ItemsCount{get{return items.Count;}}
        private string panelTemplate;
        public string PanelTemplate
        {
            get { return panelTemplate; }
            set
            {
                panelTemplate = value;
                RaisePropertyChanged("PanelTemplate");
            }

        }

        private string emptyText;
        public string EmptyText
        {
            get { return emptyText; }
            set
            {
                emptyText = value;
                RaisePropertyChanged("EmptyText");
            }

        }
        
        private string itemTemplate;
        public string ItemTemplate 
        {
            get { return itemTemplate; }
            set
            {
                itemTemplate = value;
                RaisePropertyChanged("ItemTemplate");
            }

        }
        private ObservableCollection<GenericListItem> items;
        public ObservableCollection<GenericListItem> Items
        {
            get { return items; }
        }

        public bool ClearListBeforeAdd { get; set; }
        public string CacheName { get; set; }
        [XmlIgnore]
        public Action<Action<IEnumerable<ITwitterModel>>, long> GetDataMethod;

        private void InternalGetItems(Action callback)
        {
            if (this.GetDataMethod == null)
                return;

            Log.Info("Internal Get Items " + this.header.Title + " - " + Thread.CurrentThread.ManagedThreadId);
            if (TypeOfData == PanelType.Trends)
            {
                this.GetDataMethod((a) =>
                {
                    Log.Info("Trends Response" + this.header.Title + " - " + Thread.CurrentThread.ManagedThreadId);
                    GalaSoft.MvvmLight.Threading.DispatcherHelper.CheckBeginInvokeOnUI(delegate
                    {
                        if (ClearListBeforeAdd)
                            this.Items.Clear();

                        foreach (var item in a)
                        {
                            AddItem(TrendingItemViewModel.FromTrend(item));
                        }
                        CacheThis();

                        callback();

                        if (IsSearch)
                        {
                            if (TwitterInterface.HasSearches)
                                this.EmptyText = "No tweets";
                            else
                            {
                                this.EmptyText = "You do not have search terms saved";
                            }
                        }
                        else
                        {
                            this.EmptyText = "No tweets";
                        }
                        //Messenger.Default.Send<NotificationMessage>(new NotificationMessage(MainAppPage.MESSAGE_BUSY_OFF));

                    });
                },
                    -1);

            }
            else
            {
                long id = -1;
                if (this.items.Count > 0)
                {
                    var tweet = this.items[0] as TweetViewModel;
                    id = tweet.ID;
                }
                
                this.GetDataMethod((a) =>
                {
                    Log.Info("Tweets Response" + this.header.Title + " - " + Thread.CurrentThread.ManagedThreadId);
                    GalaSoft.MvvmLight.Threading.DispatcherHelper.CheckBeginInvokeOnUI(delegate
                    {
                        if (ClearListBeforeAdd)
                            this.Items.Clear();

                        int index = 0;
                        bool addToTop = this.items.Count > 0;

                        foreach (var item in a)
                        {
                            if (item == null)
                                continue;

                            TweetViewModel newItem = null;
                            if (item is TwitterStatus)
                            {
                                newItem = TweetViewModel.FromTwitterStatus(item as TwitterStatus);
                            }
                            else
                            {
                                newItem = TweetViewModel.FromTwitterStatus(item as TwitterSearchStatus);
                                if (newItem.Message.StartsWith("search:"))
                                {
                                    int idx= newItem.Message.IndexOf("search:") + 7;
                                    int endindex = newItem.Message.IndexOf("}", idx);

                                    string search = newItem.Message.Substring(idx, endindex - idx);
                                    string tweet = newItem.Message.Substring(endindex+1);
                                    newItem.Message = tweet;
                                    newItem.SearchTag = search;
                                }
                                
                            }
                            

                            if (addToTop)
                                InsertItem(index, newItem);
                            else
                                AddItem(newItem);
                            index++;
                        }

                        CacheThis();

                        callback();
                        this.EmptyText = "No items";

                        //Messenger.Default.Send<NotificationMessage>(new NotificationMessage(MainAppPage.MESSAGE_BUSY_OFF));
                    });
                }, id);
            }

        }
        internal void GetItems(Action callback)
        {
            if (this.GetDataMethod == null) return;

            Log.Info("Get Items " + this.Header.Title + " - " + Thread.CurrentThread.ManagedThreadId);
            
            //Messenger.Default.Send<NotificationMessage>(new NotificationMessage(MainAppPage.MESSAGE_BUSY_ON));

            GalaSoft.MvvmLight.Threading.DispatcherHelper.CheckBeginInvokeOnUI(delegate
            {
                this.EmptyText = "Loading...";
            });

            LoadCache(InternalGetItems, callback);
        }
        bool hasLoadedCache;
        private void LoadCache(Action<Action> callback, Action otherCallback)
        {
            if (hasLoadedCache)
            {
                callback(otherCallback);
                return;
            }
            hasLoadedCache = true;
            string fname = this.header.Title.Replace(" ", "") + ".xml";
            if (!string.IsNullOrEmpty(this.CacheName))
                fname = CacheName;
            SerializationManager.DeSerializeData<PanoramaItemViewModel>(fname, delegate(PanoramaItemViewModel data)
            {
                Log.Info("Load Cache Response " + this.Header.Title + " - " + Thread.CurrentThread.ManagedThreadId);
                if (data != null && data.items.Count > 0)
                {
                    GalaSoft.MvvmLight.Threading.DispatcherHelper.CheckBeginInvokeOnUI(delegate
                    {
                        foreach (var item in data.items)
                        {
                            AddItem(item);
                        }
                    });
                }
                callback(otherCallback);
            });
        }
        public int MaxCacheCount { get; set; }
        public int MaxDisplayCount { get; set; }

        private void CacheThis()
        {
            string fileName = this.header.Title.Replace(" ", "") + ".xml";
            if (!string.IsNullOrEmpty(this.CacheName))
                fileName = CacheName;
            SerializationManager.SerializeData<PanoramaItemViewModel>(fileName, this);
        }

        private void InsertItem(int index, GenericListItem vm)
        {
            this.Items.Insert(index, vm);
            RaisePropertyChanged("ItemsCount");
        }
        bool Exists(GenericListItem item)
        {
            if (item is TweetViewModel)
            {
                foreach (var it in this.items)
                {
                    var i = it as TweetViewModel;
                    if (i.ID == ((TweetViewModel)item).ID)
                    {
                        Debug.WriteLine("");
                        return true;
                    }
                }
            }
            return false;
        }
        private void AddItem(GenericListItem vm)
        {
#if(DEBUG)
            
            if (Exists(vm))
            {
                Debug.WriteLine("");
                return;
            }
#endif
            if (this.Items.Count >= MaxDisplayCount)
                this.items.RemoveAt(this.items.Count - 1);

            this.Items.Add(vm);
            RaisePropertyChanged("ItemsCount");
        }

        public PanelType TypeOfData = PanelType.Tweets;

        public bool IsSearch { get; set; }
        internal void SetItems(ObservableCollection<GenericListItem> observableCollection)
        {
            this.items = observableCollection;
        }

        internal void Refresh()
        {
            GalaSoft.MvvmLight.Threading.DispatcherHelper.CheckBeginInvokeOnUI(delegate
            {
                Log.Info("Refresh");
                //this.Items.Clear();
                this.GetItems(delegate
                {

                });
            });
        }


        internal void UpdateCount()
        {
            GalaSoft.MvvmLight.Threading.DispatcherHelper.CheckBeginInvokeOnUI(delegate
            {
                RaisePropertyChanged("ItemsCount");
            });
        }
        bool initiallyLoaded;
        internal void SetSelected()
        {
            if (initiallyLoaded) return;
            initiallyLoaded = true;
            this.Refresh();
        }
    }
    public enum PanelType
    {
        Tweets,
        Trends
    }
    public class PanoramaItemHeaderViewModel : ViewModelBase
    {
        public PanoramaItemViewModel Parent;
        public PanoramaItemHeaderViewModel(PanoramaItemViewModel parent)
        {
            this.Parent = parent;
            buttons = new PanoButtonList(this.Parent);
        }
        private string title;
        public string Title
        {
            get { return title; }
            set
            {
                title = value;
                RaisePropertyChanged("Title");
            }
        }
        private PanoButtonList buttons;
        public PanoButtonList Buttons
        {
            get { return buttons; }
        }

    }

    public class PanoButtonList : ObservableCollection<PanoramaItemHeaderButtonViewModel>
    {
        PanoramaItemViewModel parent;
        public PanoButtonList(PanoramaItemViewModel vm)
        {
            this.parent = vm;
        }
        public void AddItem(PanoramaItemHeaderButtonViewModel item)
        {
            item.Parent = this.parent;
            base.Add(item);
        }
        public new void Add(PanoramaItemHeaderButtonViewModel item)
        {
            AddItem(item);
        }
        
    }
    public class PanoramaItemHeaderButtonViewModel : ViewModelBase
    {
        public PanoramaItemViewModel Parent;
        public PanoramaItemHeaderButtonViewModel()
        {
            
        }
        public string Key { get; set; }
        private string imageUri = string.Empty;
        public string ImageUri 
        {
            get { return imageUri; }
            set { imageUri = value; }
        }

        internal void Clicked()
        {
            if (Key == "refresh")
            {
                this.Parent.Refresh();
                //GalaSoft.MvvmLight.Messaging.Messenger.Default.Send<NotificationMessage>(
                  //  new NotificationMessage(
            }
            else if (Key == "add")
            {
                GalaSoft.MvvmLight.Messaging.Messenger.Default.Send<NotificationMessage>(
                  new NotificationMessage(MESSAGE_NEW_TWEET));
            }
        }
        public const string MESSAGE_NEW_TWEET = "MESSAGE_NEW_TWEET";
    }
}
