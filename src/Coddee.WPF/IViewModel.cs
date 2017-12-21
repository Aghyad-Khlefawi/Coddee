// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using Coddee.Validation;

namespace Coddee.WPF
{
    public interface IViewModel : IInitializable, IDisposable, INotifyPropertyChanged
    {
        string __Name { get; }

        
        ViewModelOptions ViewModelOptions { get; }
        ViewModelOptions DefaultViewModelOptions { get; }

        string ViewModelGroup { get; }
        void SetViewModelGroup(string group);
        void SetViewModelOptions(ViewModelOptions options);

        event ViewModelEventHandler Initialized;
        event ViewModelEventHandler<ValidationResult> Validated;

        ValidationResult Validate(bool validateChildren = false);
        List<IValidationRule> ValidationRules { get; }
        ValidationResult ValidationResult { get; }
        bool IsValid { get; }

    }

    public interface IPresentableViewModel : IViewModel, IPresentable
    {
        int CurrentViewIndex { get; }
        event ViewModelEventHandler<int> ViewIndexChanged;
    }
}