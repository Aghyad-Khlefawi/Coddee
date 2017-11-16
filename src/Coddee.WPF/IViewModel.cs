// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using Coddee.Validation;

namespace Coddee.WPF
{
    public interface IViewModel:IInitializable,IDisposable,INotifyPropertyChanged
    {
        string __Name { get; }
        List<string> Errors { get; }
        bool IsValid { get; }
      

        string ViewModelGroup { get; }
        void SetViewModelGroup(string group);

        RequiredFieldCollection RequiredFields { get; }

        event ViewModelEventHandler Initialized;
        event ViewModelEventHandler<IEnumerable<string>> Validated;

        IEnumerable<string> Validate(bool validateChildren = false);

    }

    public interface IPresentableViewModel : IViewModel, IPresentable
    {
        int CurrentViewIndex { get; }
        event ViewModelEventHandler<int> ViewIndexChanged;
    }
}