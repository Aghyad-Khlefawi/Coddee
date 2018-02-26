// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using System.Windows.Input;
using System.Windows.Media;
using Coddee.WPF.Commands;

namespace Coddee.Services.ApplicationSearch
{
    /// <summary>
    /// Used to present search results.
    /// </summary>
    public class SearchItem
    {
        /// <inheritdoc />
        public SearchItem()
        {
            NavigateCommand = new RelayCommand(Navigate);
        }

        /// <inheritdoc />
        public SearchItem(object id, string title, string subtitle, string category, string searchField, Geometry icon, EventHandler<SearchItem> navigationHandler)
            :this(id,title,subtitle,category,searchField,icon)
        {
            OnNavigate += navigationHandler;
        }

        /// <inheritdoc />
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

        /// <summary>
        /// An object to Identify the result.
        /// </summary>
        public object ID { get; set; }

        /// <summary>
        /// The main item title.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Subtitle
        /// </summary>
        public string Subtitle { get; set; }

        /// <summary>
        /// Category to determine the navigation behavior.
        /// </summary>
        public string Category { get; set; }

        /// <summary>
        /// The value that included in the search.
        /// </summary>
        public string SearchField { get; set; }

        /// <summary>
        /// Icon geometry
        /// </summary>
        public Geometry Icon { get; set; }


        /// <summary>
        /// triggered when the navigation to this item is requested.
        /// </summary>
        public event EventHandler<SearchItem> OnNavigate;


        /// <inheritdoc cref="Navigate"/>
        public ICommand NavigateCommand { get; }
        
        /// <summary>
        /// Navigate to the search result.
        /// </summary>
        public virtual void Navigate()
        {
            OnNavigate?.Invoke(this,this);
        }
    }
}
