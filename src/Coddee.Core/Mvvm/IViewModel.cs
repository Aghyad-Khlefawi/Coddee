// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using System.Collections.Generic;
using System.ComponentModel;
using Coddee.Validation;

namespace Coddee.Mvvm
{
    /// <summary>
    /// A ViewModel object.
    /// </summary>
    public interface IViewModel : IInitializable, IDisposable, INotifyPropertyChanged
    {
        /// <summary>
        /// Triggered when the ViewModel initialization complete.
        /// </summary>
        event ViewModelEventHandler Initialized;

        /// <summary>
        /// Triggered when the validation rules of the ViewModel is set.
        /// </summary>
        event ViewModelEventHandler<IEnumerable<IValidationRule>> ValidationRulesSet;

        /// <summary>
        /// Triggered when a validation operation is completed.
        /// </summary>
        event ViewModelEventHandler<ValidationResult> Validated;


        /// <summary>
        /// Name to identify the ViewModel
        /// </summary>
        string __Name { get; }

        /// <summary>
        /// The current ViewModel options.
        /// </summary>
        ViewModelOptions ViewModelOptions { get; }

        /// <summary>
        /// The default value for the ViewModel options.
        /// </summary>
        ViewModelOptions DefaultViewModelOptions { get; }

        /// <summary>
        /// A group name to identify ViewModels in the same group.
        /// </summary>
        string ViewModelGroup { get; }

        /// <summary>
        /// The ViewModel validation rules.
        /// </summary>
        List<IValidationRule> ValidationRules { get; }

        /// <summary>
        /// The result of the last validation operation
        /// <remarks>Can be null if the ViewModel was not validated yet.</remarks>
        /// </summary>
        ValidationResult ValidationResult { get; }

        /// <summary>
        /// Checks if the last validation result is valid.
        /// </summary>
        bool IsValid { get; }


        /// <summary>
        /// Sets <see cref="ViewModelGroup"/> property.
        /// </summary>
        /// <param name="group"></param>
        void SetViewModelGroup(string group);

        /// <summary>
        /// Set <see cref="ViewModelOptions"/> property.
        /// </summary>
        /// <param name="options"></param>
        void SetViewModelOptions(ViewModelOptions options);

        
        /// <summary>
        /// Begin the validation process.
        /// </summary>
        /// <param name="validateChildren">if set to true the ViewModels created by this ViewModel will be validated too.</param>
        /// <returns></returns>
        ValidationResult Validate(bool validateChildren = false);

       
    }

    /// <summary>
    /// A ViewModel object that can be displayed.
    /// </summary>
    public interface IPresentableViewModel : IViewModel, IPresentable
    {
        /// <summary>
        /// The index of the currently displayed view.
        /// </summary>
        int CurrentViewIndex { get; }


        /// <summary>
        /// Triggered when the <see cref="CurrentViewIndex"/> property is changed.
        /// </summary>
        event ViewModelEventHandler<int> ViewIndexChanged;
    }
}