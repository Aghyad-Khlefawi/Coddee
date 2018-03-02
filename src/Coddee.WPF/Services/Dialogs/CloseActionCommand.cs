// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using Coddee.Services;

namespace Coddee.WPF.Services.Dialogs
{
    /// <summary>
    /// A close button <see cref="ActionCommand"/>
    /// </summary>
    public class CloseActionCommand : ActionCommand
    {
        /// <inheritdoc />
        public CloseActionCommand(string title) : base(title, null)
        {
        }

        /// <inheritdoc />
        public CloseActionCommand(string title, Action aditionalAction) : base(title, aditionalAction)
        {
        }

        /// <summary>
        /// Default instance of the type.
        /// </summary>
        public static CloseActionCommand Default = new CloseActionCommand(LocalizationManager.DefaultLocalizationManager["Close"]);
    }
}
