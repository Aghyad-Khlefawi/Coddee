// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;

namespace Coddee.WPF.Services.Dialogs
{
    public class CloseActionCommand : ActionCommand
    {
        public CloseActionCommand(string title) : base(title, null)
        {
        }

        public CloseActionCommand(string title, Action aditionalAction) : base(title, aditionalAction)
        {
        }
    }
}
