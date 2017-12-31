// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;

namespace Coddee
{
    public class ActionCommand
    {
        public ActionCommand(string title, Action action)
        {
            CanExecute = true;
            Title = title;
            Action = action;
        }

        public event EventHandler<bool> CanExecuteChanged;

        public string Title { get; set; }
        public Action Action { get; set; }
        public HorizontalPosition HorizontalPosition { get; set; } = HorizontalPosition.Right;

        public bool CanExecute { get; private set; }

        public void SetCanExecute(bool value)
        {
            CanExecute = value;
            CanExecuteChanged?.Invoke(this, value);
        }
    }
}
