// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System.Threading.Tasks;

namespace Coddee.Mvvm
{
    /// <summary>
    /// Delegate for ViewModel events.
    /// </summary>
    /// <param name="sender">The ViewModel that triggered the event.</param>
    public delegate void ViewModelEventHandler(IViewModel sender);

    /// <summary>
    /// Delegate for ViewModel events.
    /// </summary>
    /// <param name="sender">The ViewModel that triggered the event.</param>
    /// <param name="args">The event arguments</param>
    public delegate void ViewModelEventHandler<in TArgs>(IViewModel sender, TArgs args);


    /// <summary>
    /// Delegate for an Async ViewModel events.
    /// </summary>
    /// <param name="sender">The ViewModel that triggered the event.</param>
    /// <param name="args">The event arguments</param>
    public delegate Task AsyncViewModelEventHandler<in TArgs>(IViewModel sender, TArgs args);
}
