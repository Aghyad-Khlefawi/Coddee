// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System.Windows.Input;
using Coddee.Validation;

namespace Coddee.WPF.Commands
{
    /// <summary>
    /// A command that updates the CanExecute property based on properties changes.
    /// </summary>
    public interface IReactiveCommand : ICommand
    {
        /// <summary>
        /// Set a property to be observed by the command.
        /// </summary>
        /// <param name="propertyName">The name of the property</param>
        /// <param name="validator">The validation function</param>
        /// <returns></returns>
        IReactiveCommand ObserveProperty(string propertyName, Validator validator);

        /// <summary>
        /// Updates the CanExecute value.
        /// </summary>
        void UpdateCanExecute();

        /// <summary>
        /// Checks if the command can be executed.
        /// </summary>
        /// <returns></returns>
        bool CanExecute();
    }

    /// <inheritdoc />
    public interface IReactiveCommand<TObserved> : IReactiveCommand
    {
        /// <summary>
        /// Set a property to be observed by the command.
        /// </summary>
        /// <param name="propertyName">The name of the property</param>
        /// <param name="validator">The validation function</param>
        /// <returns></returns>
        IReactiveCommand<TObserved> ObserveProperty<TProperty>(string propertyName, Validator<TProperty> validator);
    }
}