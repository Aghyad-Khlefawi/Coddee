// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using System.Threading.Tasks;

namespace Coddee
{

    public abstract class ActionCommandBase
    {
        public ActionCommandBase(string title)
        {
            CanExecute = true;
            Title = title;
        }

        public event EventHandler<bool> CanExecuteChanged;

        public string Title { get; set; }

        public HorizontalPosition HorizontalPosition { get; set; } = HorizontalPosition.Right;

        public bool CanExecute { get; private set; }

        public void SetCanExecute(bool value)
        {
            CanExecute = value;
            CanExecuteChanged?.Invoke(this, value);
        }
    }

    public class ActionCommand : ActionCommandBase
    {
        public ActionCommand(string title, Action action)
            : base(title)
        {
            Action = action;
        }
        public Action Action { get; set; }

    }
    public class AsyncActionCommand : ActionCommandBase
    {
        public AsyncActionCommand(string title, Func<Task<bool>> action)
            : base(title)
        {
            Action = action;
        }
        public Func<Task<bool>> Action { get; set; }

    }
}
