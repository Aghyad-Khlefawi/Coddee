// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System.Threading.Tasks;

namespace Coddee.WPF
{
    public delegate void ViewModelEventHandler(IViewModel sender);

    public delegate void ViewModelEventHandler<TArgs>(IViewModel sender, TArgs args);
    public delegate Task AsyncViewModelEventHandler<TArgs>(IViewModel sender, TArgs args);
}
