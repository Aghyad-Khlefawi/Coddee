// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using Coddee.Services.Dialogs;

namespace Coddee.WPF.Services.Dialogs
{
    /// <summary>
    /// The possible states of an <see cref="IDialog"/>
    /// </summary>
    public enum DialogState
    {
        /// <summary>
        /// The dialog is opened.
        /// </summary>
        Active,

        /// <summary>
        /// The dialog is minimized.
        /// </summary>
        Minimized
    }
}
