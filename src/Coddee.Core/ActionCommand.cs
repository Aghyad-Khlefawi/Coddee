// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using System.Threading.Tasks;

namespace Coddee
{
    /// <summary>
    /// Common action command tags
    /// </summary>
    public static class ActionCommandTags
    {
        /// <summary>
        /// Indicates that the command is save command (save button)
        /// </summary>
        public const string SaveCommand = nameof(SaveCommand);
    }

    /// <summary>
    /// A class the represent an action command
    /// </summary>
    public abstract class ActionCommandBase
    {
        /// <summary>
        /// </summary>
        /// <param name="title">The title for the command</param>
        protected ActionCommandBase(string title)
        {
            CanExecute = true;
            Title = title;
        }

        /// <summary>
        /// Raised when the value of <see cref="CanExecute"/> is changed
        ///  </summary>
        public event EventHandler<bool> CanExecuteChanged;

        /// <summary>
        /// A title to display for the command
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// A helper tag to clear the functionality of the command
        /// </summary>
        public string Tag { get; set; }

        /// <summary>
        /// The position of the command on the screen horizontally
        /// </summary>
        public HorizontalPosition HorizontalPosition { get; set; } = HorizontalPosition.Right;

        /// <summary>
        /// Indicates whether the command can be executed(enabled)
        /// </summary>
        public bool CanExecute { get; private set; }

        /// <summary>
        /// Set the value of <see cref="CanExecute"/> property
        /// </summary>
        /// <param name="value">The new value</param>
        public void SetCanExecute(bool value)
        {
            CanExecute = value;
            CanExecuteChanged?.Invoke(this, value);
        }
    }

    /// <inheritdoc/>
    public class ActionCommand : ActionCommandBase
    {
        /// <inheritdoc/>
        public ActionCommand(string title, Action action)
            : base(title)
        {
            Action = action;
        }

        /// <summary>
        /// The action that will be executed by the command
        /// </summary>
        public Action Action { get; set; }

    }

    /// <summary>
    /// An async implementation of <see cref="ActionCommand"/>
    /// </summary>
    public class AsyncActionCommand : ActionCommandBase
    {
        /// <inheritdoc/>
        public AsyncActionCommand(string title, Func<Task<bool>> action)
            : base(title)
        {
            Action = action;
        }

        /// <summary>
        /// The Task that will be executed by the command.
        /// </summary>
        public Func<Task<bool>> Action { get; set; }

    }
}
