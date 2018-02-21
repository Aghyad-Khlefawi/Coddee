// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

namespace Coddee.Mvvm
{
    /// <summary>
    /// Implemented by object that have an associated view
    /// </summary>
    public interface IPresentable
    {
        /// <summary>
        /// Returns the view associated with this object
        /// </summary>
        /// <returns></returns>
        object GetView(int viewIndex);

        /// <summary>
        /// Returns the view associated with this object
        /// </summary>
        /// <returns></returns>
        object GetView();
    }

    /// <summary>
    /// Implemented by object that have an associated view
    /// </summary>
    public interface IPresentable<TView> : IPresentable where TView : class
    {
        /// <summary>
        /// The default view.
        /// </summary>
        TView View { get; }
    }
}