// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;

namespace Coddee
{
    public interface IGlobaleVariable
    {

    }
    public class GlobalVarialbe<T> : IGlobaleVariable
    {
        protected T _value;
        public bool IsValueSet { get; private set; }

        public event EventHandler<T> ValueChanged;

        public void SetValue(T value)
        {
            _value = value;
            IsValueSet = true;
            ValueChanged?.Invoke(this, value);
        }

        public T GetValue()
        {
            return _value;
        }
        public bool TryGetValue(out T value)
        {
            value = IsValueSet ? _value : default(T);
            return IsValueSet;
        }
    }
}
