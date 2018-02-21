// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using Coddee.Mvvm;
using Coddee.WPF;

namespace Coddee.Services.ApplicationSearch
{
    public interface IApplicationQuickSearch : IPresentableViewModel
    {
        void Focus();
    }
}
