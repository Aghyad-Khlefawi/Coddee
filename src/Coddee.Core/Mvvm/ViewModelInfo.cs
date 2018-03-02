// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System.Collections.Generic;

namespace Coddee.Mvvm
{
    /// <summary>
    /// Contains the information of a ViewModel object hierarchy
    /// </summary>
    public class ViewModelInfo
    {
        /// <inheritdoc />
        public ViewModelInfo(IViewModel viewModel)
        {
            ViewModel = viewModel;
            ChildViewModels = new List<ViewModelInfo>();
        }

        /// <summary>
        /// An ID to identify the ViewModel.
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// The ViewModel instance.
        /// </summary>
        public IViewModel ViewModel { get; set; }

        /// <summary>
        /// The ViewModels created by the subject ViewModel.
        /// </summary>
        public List<ViewModelInfo> ChildViewModels { get; set; }

        /// <summary>
        /// The ViewModel that created the subject ViewModel.
        /// </summary>
        public ViewModelInfo ParentViewModel { get; set; }
    }
}
