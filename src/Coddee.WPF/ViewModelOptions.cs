// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

namespace Coddee.WPF
{
    public class ViewModelOptions
    {
        public static readonly ViewModelOptions Default = new ViewModelOptions
        {
            IncludeInHierarchicalValidation = true
        };
        public static readonly ViewModelOptions Editor  = new ViewModelOptions
        {
            IncludeInHierarchicalValidation = false
        };

        public bool IncludeInHierarchicalValidation { get; set; }
    }
}
