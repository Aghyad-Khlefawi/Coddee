// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using System.Windows.Input;
using System.Windows.Media;
using Coddee.WPF.Commands;

namespace Coddee.Services.ApplicationSearch
{
    public class SearchItem
    {
        public SearchItem()
        {
            NavigateCommand = new RelayCommand(Navigate);
        }

        public SearchItem(object id, string title, string subtitle, string category, string searchField, Geometry icon, EventHandler<SearchItem> navigationHandler)
            :this(id,title,subtitle,category,searchField,icon)
        {
            OnNavigate += navigationHandler;
        }

        public SearchItem(object id, string title, string subtitle, string category, string searchField, Geometry icon)
            : this()
        {
            ID = id;
            Title = title;
            Subtitle = subtitle;
            Category = category;
            SearchField = searchField;
            Icon = icon;
        }
        public object ID { get; set; }
        public string Title { get; set; }
        public string Subtitle { get; set; }
        public string Category { get; set; }
        public string SearchField { get; set; }
        public Geometry Icon { get; set; }

        public event EventHandler<SearchItem> OnNavigate;
        public ICommand NavigateCommand { get; }
        
        public virtual void Navigate()
        {
            OnNavigate?.Invoke(this,this);
        }
    }
}
