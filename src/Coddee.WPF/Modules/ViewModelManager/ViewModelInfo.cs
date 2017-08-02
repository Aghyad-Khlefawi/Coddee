// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System.Collections.Generic;
using Coddee.WPF;

namespace Coddee.Services.ViewModelManager
{
    public class ViewModelInfo
    {
        public ViewModelInfo(IViewModel viewModel)
        {
            ViewModel = viewModel;
            ChildViewModels = new List<ViewModelInfo>();
        }

        public int ID { get; set; }
        public IViewModel ViewModel { get; set; }
        public List<ViewModelInfo> ChildViewModels { get; set; }
        public ViewModelInfo ParentViewModel { get; set; }
    }
}
