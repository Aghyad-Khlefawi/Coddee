// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using System.Collections.Generic;
using System.Windows;
using Coddee.Mvvm;

namespace Coddee.WPF.Regions
{
    /// <summary>
    /// Holds a reference to all the created regions. 
    /// </summary>
    public static class RegionManager
    {
        /// <summary>
        /// all created regions, with their name as key
        /// </summary>
        private static readonly Dictionary<string, Region> _regions = new Dictionary<string, Region>();

        /// <summary>
        /// Change the region content
        /// </summary>
        /// <param name="regionName">Target region</param>
        /// <param name="view">The new content</param>
        public static void View(string regionName, object view)
        {
            if (!_regions.ContainsKey(regionName))
                throw new ArgumentException($"Region [{regionName}] was not found ");
            _regions[regionName].View(view);
        }

        /// <summary>
        /// Change the region content
        /// </summary>
        /// <param name="regionName">Target region</param>
        /// <param name="view">The new content</param>
        public static void View(string regionName, IPresentable view)
        {
            View(regionName, view.GetView());
        }

        /// <summary>
        /// Add a new region 
        /// </summary>
        /// <param name="region">Region to add</param>
        public static void Add(Region region)
        {
            //if (_regions.ContainsKey(region.GetName()))
            //    if (!DesignerProperties.GetIsInDesignMode(new DependencyObject()))
            //    {
            //        throw new ArgumentException($"A region with the same name [{region.GetName()}] already exists");
            //    }
            _regions[region.GetName()] = region;
        }

        /// <summary>
        /// Clear a specific region
        /// </summary>
        /// <param name="regionName">Region to clear</param>
        public static void Clear(string regionName)
        {
            if (!_regions.ContainsKey(regionName))
                throw new ArgumentException($"Region [{regionName}] was not found ");
            _regions[regionName].Clear();
        }
    }
}