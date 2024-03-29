﻿using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.ApplicationModel.Resources.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using System.Collections.Specialized;

// The data model defined by this file serves as a representative example of a strongly-typed
// model that supports notification when members are added, removed, or modified.  The property
// names chosen coincide with data bindings in the standard item templates.
//
// Applications may use this model as a starting point and build on it, or discard it entirely and
// replace it with something appropriate to their needs.

namespace EcommFashion.Data
{
    /// <summary>
    /// Base class for <see cref="WomenDataItem"/> and <see cref="WomenDataGroup"/> that
    /// defines properties common to both.
    /// </summary>
    [Windows.Foundation.Metadata.WebHostHidden]
    public abstract class WomenDataCommon : EcommFashion.Common.BindableBase
    {
        private static Uri _baseUri = new Uri("ms-appx:///");

        public WomenDataCommon(String uniqueId, String title, String subtitle, String imagePath, String description)
        {
            this._uniqueId = uniqueId;
            this._title = title;
            this._subtitle = subtitle;
            this._description = description;
            this._imagePath = imagePath;
        }

        private string _uniqueId = string.Empty;
        public string UniqueId
        {
            get { return this._uniqueId; }
            set { this.SetProperty(ref this._uniqueId, value); }
        }

        private string _title = string.Empty;
        public string Title
        {
            get { return this._title; }
            set { this.SetProperty(ref this._title, value); }
        }

        private string _subtitle = string.Empty;
        public string Subtitle
        {
            get { return this._subtitle; }
            set { this.SetProperty(ref this._subtitle, value); }
        }

        private string _description = string.Empty;
        public string Description
        {
            get { return this._description; }
            set { this.SetProperty(ref this._description, value); }
        }


        private string _GroupItemImage = string.Empty;
        public string GroupItemImage
        {
            get { return this._GroupItemImage; }
            set { this.SetProperty(ref this._GroupItemImage, value); }
        }

        private ImageSource _image = null;
        private String _imagePath = null;
        public ImageSource Image
        {
            get
            {
                if (this._image == null && this._imagePath != null)
                {
                    this._image = new BitmapImage(new Uri(WomenDataCommon._baseUri, this._imagePath));
                }
                return this._image;
            }

            set
            {
                this._imagePath = null;
                this.SetProperty(ref this._image, value);
            }
        }

        public void SetImage(String path)
        {
            this._image = null;
            this._imagePath = path;
            this.OnPropertyChanged("Image");
        }

        public override string ToString()
        {
            return this.Title;
        }
    }

    /// <summary>
    /// Generic item data model.
    /// </summary>
    public class WomenDataItem : WomenDataCommon
    {
        public WomenDataItem(String uniqueId, String title, String subtitle, String imagePath, String description, String content, WomenDataGroup group)
            : base(uniqueId, title, subtitle, imagePath, description)
        {
            this._content = content;
            this._group = group;
        }

        private string _content = string.Empty;
        public string Content
        {
            get { return this._content; }
            set { this.SetProperty(ref this._content, value); }
        }

        private WomenDataGroup _group;
        public WomenDataGroup Group
        {
            get { return this._group; }
            set { this.SetProperty(ref this._group, value); }
        }
    }

    /// <summary>
    /// Generic group data model.
    /// </summary>
    public class WomenDataGroup : WomenDataCommon
    {
        public WomenDataGroup(String uniqueId, String title, String subtitle, String imagePath, String description)
            : base(uniqueId, title, subtitle, imagePath, description)
        {
            Items.CollectionChanged += ItemsCollectionChanged;
        }

        private void ItemsCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            // Provides a subset of the full items collection to bind to from a GroupedItemsPage
            // for two reasons: GridView will not virtualize large items collections, and it
            // improves the user experience when browsing through groups with large numbers of
            // items.
            //
            // A maximum of 12 items are displayed because it results in filled grid columns
            // whether there are 1, 2, 3, 4, or 6 rows displayed

            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    if (e.NewStartingIndex < 12)
                    {
                        TopItems.Insert(e.NewStartingIndex, Items[e.NewStartingIndex]);
                        if (TopItems.Count > 12)
                        {
                            TopItems.RemoveAt(12);
                        }
                    }
                    break;
                case NotifyCollectionChangedAction.Move:
                    if (e.OldStartingIndex < 12 && e.NewStartingIndex < 12)
                    {
                        TopItems.Move(e.OldStartingIndex, e.NewStartingIndex);
                    }
                    else if (e.OldStartingIndex < 12)
                    {
                        TopItems.RemoveAt(e.OldStartingIndex);
                        TopItems.Add(Items[11]);
                    }
                    else if (e.NewStartingIndex < 12)
                    {
                        TopItems.Insert(e.NewStartingIndex, Items[e.NewStartingIndex]);
                        TopItems.RemoveAt(12);
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    if (e.OldStartingIndex < 12)
                    {
                        TopItems.RemoveAt(e.OldStartingIndex);
                        if (Items.Count >= 12)
                        {
                            TopItems.Add(Items[11]);
                        }
                    }
                    break;
                case NotifyCollectionChangedAction.Replace:
                    if (e.OldStartingIndex < 12)
                    {
                        TopItems[e.OldStartingIndex] = Items[e.OldStartingIndex];
                    }
                    break;
                case NotifyCollectionChangedAction.Reset:
                    TopItems.Clear();
                    while (TopItems.Count < Items.Count && TopItems.Count < 12)
                    {
                        TopItems.Add(Items[TopItems.Count]);
                    }
                    break;
            }
        }

        private ObservableCollection<WomenDataItem> _items = new ObservableCollection<WomenDataItem>();
        public ObservableCollection<WomenDataItem> Items
        {
            get { return this._items; }
        }

        private ObservableCollection<WomenDataItem> _topItem = new ObservableCollection<WomenDataItem>();
        public ObservableCollection<WomenDataItem> TopItems
        {
            get {return this._topItem; }
        }
    }

    /// <summary>
    /// Creates a collection of groups and items with hard-coded content.
    /// 
    /// WomenDataSource initializes with placeholder data rather than live production
    /// data so that sample data is provided at both design-time and run-time.
    /// </summary>
    public sealed class WomenDataSource
    {
        private static WomenDataSource _WomenDataSource = new WomenDataSource();

        private ObservableCollection<WomenDataGroup> _allGroups = new ObservableCollection<WomenDataGroup>();
        public ObservableCollection<WomenDataGroup> AllGroups
        {
            get { return this._allGroups; }
        }

        public static IEnumerable<WomenDataGroup> GetGroups(string uniqueId)
        {
            if (!uniqueId.Equals("AllGroups")) throw new ArgumentException("Only 'AllGroups' is supported as a collection of groups");
            
            return _WomenDataSource.AllGroups;
        }

        public static WomenDataGroup GetGroup(string uniqueId)
        {
            // Simple linear search is acceptable for small data sets
            var matches = _WomenDataSource.AllGroups.Where((group) => group.UniqueId.Equals(uniqueId));
            if (matches.Count() == 1) return matches.First();
            return null;
        }

        public static WomenDataItem GetItem(string uniqueId)
        {
            // Simple linear search is acceptable for small data sets
            var matches = _WomenDataSource.AllGroups.SelectMany(group => group.Items).Where((item) => item.UniqueId.Equals(uniqueId));
            if (matches.Count() == 1) return matches.First();
            return null;
        }

        public WomenDataSource()
        {
            String ITEM_CONTENT = String.Format("Item Content: {0}\n\n{0}\n\n{0}\n\n{0}\n\n{0}\n\n{0}\n\n{0}",
                        "Nivax Women Data Source");

            var group1 = new WomenDataGroup("Group-1",
                    "casual dresses",
                    "Group Subtitle: 1",
                    "Assets/DarkGray.png",
                    "Group Description: Nivax Group Description");
            group1.Items.Add(new WomenDataItem("Group-1-Item-1",
                    "Lorem ipsum dolor",
                    "830.00",
                    "Assets/WomenPage/WomenPage1.png",
                    "women",
                    ITEM_CONTENT,
                    group1) { GroupItemImage = "../Assets/groupDetailPage/WomenGroupItemIImage1.png" });
            group1.Items.Add(new WomenDataItem("Group-1-Item-2",
                   "Lorem ipsum dolor",
                    "830.00",
                     "Assets/WomenPage/WomenPage2.png",
                    "women",
                    ITEM_CONTENT,
                    group1){ GroupItemImage = "../Assets/groupDetailPage/WomenGroupItemIImage2.png" });
            group1.Items.Add(new WomenDataItem("Group-1-Item-3",
                    "Lorem ipsum dolor",
                    "830.00",
                     "Assets/WomenPage/WomenPage3.png",
                   "women",
                    ITEM_CONTENT,
                    group1){ GroupItemImage = "../Assets/groupDetailPage/WomenGroupItemIImage3.png" });
            group1.Items.Add(new WomenDataItem("Group-1-Item-4",
                    "Lorem ipsum dolor",
                    "830.00",
                     "Assets/WomenPage/WomenPage4.png",
                    "women",
                    ITEM_CONTENT,
                    group1){ GroupItemImage = "../Assets/groupDetailPage/WomenGroupItemIImage4.png" });
            group1.Items.Add(new WomenDataItem("Group-1-Item-5",
                    "Lorem ipsum dolor",
                    "830.00",
                    "Assets/HubPage/HubPage5.png",
                    "women",
                    ITEM_CONTENT,
                    group1){ GroupItemImage = "../Assets/groupDetailPage/WomenGroupItemIImage5.png" });
            group1.Items.Add(new WomenDataItem("Group-1-Item-6",
                    "Lorem ipsum dolor",
                    "830.00",
                     "Assets/WomenPage/WomenPage6.png",
                    "women",
                    ITEM_CONTENT,
                    group1){ GroupItemImage = "../Assets/groupDetailPage/WomenGroupItemIImage6.png" });
            this.AllGroups.Add(group1);

            var group2 = new WomenDataGroup("Group-2",
                    "formal dresses",
                    "Group Subtitle: 2",
                    "Assets/LightGray.png",
                    "Group Description: Lorem ipsum dolor sit amet, consectetur adipiscing elit. Vivamus tempor scelerisque lorem in vehicula. Aliquam tincidunt, lacus ut sagittis tristique, turpis massa volutpat augue, eu rutrum ligula ante a ante");
            group2.Items.Add(new WomenDataItem("Group-2-Item-1",
                   "Lorem ipsum dolor",
                    "830.00",
                     "Assets/WomenPage/WomenPage7.png",
                    "women",
                    ITEM_CONTENT,
                    group2){ GroupItemImage = "../Assets/WomenPage/WomenPage7.png" });
            group2.Items.Add(new WomenDataItem("Group-2-Item-2",
                    "Lorem ipsum dolor",
                    "830.00",
                     "Assets/WomenPage/WomenPage8.png",
                    "women",
                    ITEM_CONTENT,
                    group2){ GroupItemImage = "../Assets/WomenPage/WomenPage8.png" });
            group2.Items.Add(new WomenDataItem("Group-2-Item-3",
                     "Lorem ipsum dolor",
                    "830.00",
                     "Assets/WomenPage/WomenPage9.png",
                    "women",
                    ITEM_CONTENT,
                    group2){ GroupItemImage = "../Assets/WomenPage/WomenPage9.png" });
            group2.Items.Add(new WomenDataItem("Group-2-Item-4",
                    "Lorem ipsum dolor",
                   "830.00",
                    "Assets/WomenPage/WomenPage10.png",
                   "women",
                    ITEM_CONTENT,
                   group2){ GroupItemImage = "../Assets/WomenPage/WomenPage10.png" });
            group2.Items.Add(new WomenDataItem("Group-2-Item-5",
                    "Lorem ipsum dolor",
                   "830.00",
                    "Assets/WomenPage/WomenPage11.png",
                   "women",
                    ITEM_CONTENT,
                   group2){ GroupItemImage = "../Assets/WomenPage/WomenPage11.png" });
            group2.Items.Add(new WomenDataItem("Group-2-Item-6",
                    "Lorem ipsum dolor",
                   "830.00",
                    "Assets/WomenPage/WomenPage12.png",
                   "women",
                    ITEM_CONTENT,
                   group2){ GroupItemImage = "../Assets/WomenPage/WomenPage12.png" });
            this.AllGroups.Add(group2);
			
			 var group3 = new WomenDataGroup("Group-3",
                    "accessories",
                    "Group Subtitle: 2",
                    "Assets/LightGray.png",
                    "Group Description: Lorem ipsum dolor sit amet, consectetur adipiscing elit. Vivamus tempor scelerisque lorem in vehicula. Aliquam tincidunt, lacus ut sagittis tristique, turpis massa volutpat augue, eu rutrum ligula ante a ante");
            group3.Items.Add(new WomenDataItem("Group-3-Item-1",
                   "Lorem ipsum dolor",
                    "830.00",
                    "Assets/WomenPage/WomenPage13.png",
                    "women",
                    ITEM_CONTENT,
                    group3){ GroupItemImage = "../Assets/WomenPage/WomenPage13.png" });
            group3.Items.Add(new WomenDataItem("Group-3-Item-2",
                    "Lorem ipsum dolor",
                    "830.00",
                    "Assets/WomenPage/WomenPage14.png",
                    "women",
                    ITEM_CONTENT,
                    group3){ GroupItemImage = "../Assets/WomenPage/WomenPage14.png" });
            group3.Items.Add(new WomenDataItem("Group-2-Item-3",
                     "Lorem ipsum dolor",
                    "830.00",
                     "Assets/WomenPage/WomenPage15.png",
                    "women",
                    ITEM_CONTENT,
                    group3){ GroupItemImage = "../Assets/WomenPage/WomenPage15.png" });
            group3.Items.Add(new WomenDataItem("Group-3-Item-4",
                    "Lorem ipsum dolor",
                   "830.00",
                    "Assets/WomenPage/WomenPage16.png",
                   "women",
                    ITEM_CONTENT,
                   group3){ GroupItemImage = "../Assets/WomenPage/WomenPage16.png" });
            group3.Items.Add(new WomenDataItem("Group-2-Item-5",
                    "Lorem ipsum dolor",
                   "830.00",
                    "Assets/WomenPage/WomenPage17.png",
                   "women",
                    ITEM_CONTENT,
                   group3){ GroupItemImage = "../Assets/WomenPage/WomenPage17.png" });
            group3.Items.Add(new WomenDataItem("Group-3-Item-6",
                    "Lorem ipsum dolor",
                   "830.00",
                    "Assets/WomenPage/WomenPage18.png",
                   "women",
                    ITEM_CONTENT,
                   group3){ GroupItemImage = "../Assets/WomenPage/WomenPage18.png" });
            this.AllGroups.Add(group3);
            
           
        }
    }
}
