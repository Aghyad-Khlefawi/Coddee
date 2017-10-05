// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using Coddee.Validation;

namespace Coddee.WPF
{
    public interface IViewModel:IDisposable,INotifyPropertyChanged
    {
        string __Name { get; }

        event EventHandler Initialized;
        
        bool IsInitialized { get; }
        Task Initialize();

        RequiredFieldCollection RequiredFields { get; }
        IEnumerable<string> Validate();
        event EventHandler<IEnumerable<string>> Validated;
    }

    public interface IPresentableViewModel : IViewModel, IPresentable
    {
        
    }
}