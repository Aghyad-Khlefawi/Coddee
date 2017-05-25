// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using System.Collections.Generic;

namespace Coddee.Services
{
    public interface IGlobalVariablesService
    {
        event EventHandler<ValueChangedEventArgs> VariableValueChanged;
        void SetValue(string Key, object value);
        T GetValue<T>(string key);
        bool TryGetValue<T>(string key, out T result);
        object this[string index] { get; set; }
        IDictionary<string, object> GetAllGlobals();
    }
}
