// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Coddee.WPF
{
    public interface IViewModel:IDisposable
    {
        IViewModel ParentViewModel { get; set; }
        IList<IViewModel> ChildViewModels { get; }

        bool IsInitialized { get; }
        Task Initialize();
    }
}