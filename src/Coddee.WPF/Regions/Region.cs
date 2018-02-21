// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System.Windows;
using Coddee.Mvvm;
using Coddee.WPF.Regions;

namespace Coddee.WPF
{
    /// <summary>
    /// A region of the application UI.
    /// </summary>
    public class Region : BindableBase
    {
        /// <summary>
        /// Regions must be creates with Region.CreateRegion
        /// </summary>
        /// <param name="Name"></param>
        private Region(string Name)
        {
            _name = Name;
        }

        /// <summary>
        /// Creates a new region
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static Region CreateRegion(string name)
        {
            var region = new Region(name);
            RegionManager.Add(region);
            return region;
        }

        private readonly string _name;

        private object _content;
        
        /// <summary>
        /// The content displayed in the Region.
        /// </summary>
        public object Content
        {
            get { return _content; }
            set { SetProperty(ref this._content, value); }
        }

        /// <summary>
        /// Returns the name of the region.
        /// </summary>
        /// <returns></returns>
        public string GetName()
        {
            return _name;
        }

        /// <summary>
        /// Change the region content
        /// </summary>
        /// <param name="content">The new content</param>
        public void View(object content)
        {
            Content = content;
        }

        /// <summary>
        /// Change the region content
        /// </summary>
        /// <param name="content">The new content</param>
        public void View(IPresentable content)
        {
            View(content.GetView());
        }

        /// <summary>
        /// Clears the region content
        /// </summary>
        public void Clear()
        {
            Content = null;
        }
    }
}