// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System.Collections.Generic;
using System.Threading.Tasks;

namespace Coddee.WPF
{
    public interface IViewModel
    {
        IViewModel Parent { get; set; }
        IList<IViewModel> Childreen { get; }

        bool IsInitialized { get; }
        Task Initialize();
    }
}