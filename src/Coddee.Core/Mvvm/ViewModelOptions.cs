// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

namespace Coddee.Mvvm
{
    /// <summary>
    /// options for ViewMode object to determine its behavior.  
    /// </summary>
    public class ViewModelOptions
    {
        /// <summary>
        /// Default ViewModel options.
        /// </summary>
        public static readonly ViewModelOptions Default = new ViewModelOptions
        {
            IncludeInHierarchicalValidation = true,
            ShowErrors = true
        };

        /// <summary>
        /// Default EditoViewModel options.
        /// </summary>
        public static readonly ViewModelOptions Editor = new ViewModelOptions
        {
            IncludeInHierarchicalValidation = false,
            ShowErrors = true
        };

        /// <summary>
        /// EditoViewModel options that does not show errors to the user.
        /// </summary>
        public static readonly ViewModelOptions EditorNoErrors = new ViewModelOptions
        {
            IncludeInHierarchicalValidation = false,
            ShowErrors = false
        };

        /// <summary>
        /// If set to false the ViewModel will not
        /// be included in the hierarchical validation operation.
        /// </summary>
        public bool IncludeInHierarchicalValidation { get; set; }
        
        /// <summary>
        /// If false errors should not be displayed to the user.
        /// </summary>
        public bool ShowErrors { get; set; }
    }
}
