using System;
using System.Collections.Generic;
using System.ComponentModel;
using Coddee.Validation;

namespace Coddee.Xamarin.Common
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
        event ViewModelEventHandler<IEnumerable<IValidationRule>> ValidationRulesSet;
        event ViewModelEventHandler<ValidationResult> Validated;
        ValidationResult Validate(bool validateChildren = false);
        List<IValidationRule> ValidationRules { get; }
        ValidationResult ValidationResult { get; }
        bool IsValid { get; }
    }

    public interface IPresentableViewModel : IViewModel, IPresentable
    {
        int CurrentPageIndex { get; }
        event ViewModelEventHandler<int> PageIndexChanged;
    }
}