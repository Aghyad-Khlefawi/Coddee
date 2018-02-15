// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;

namespace Coddee
{
    /// <summary>
    /// Identifies a global variable object.
    /// </summary>
    public interface IGlobaleVariable
    {

    }

    /// <summary>
    /// A global variable that can be Get or Set from anywhere in the application
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class GlobalVarialbe<T> : IGlobaleVariable
    {
        /// <summary>
        /// if false then the variable has the default value (<see langword="default"/>>(T))
        /// </summary>
        public bool IsValueSet { get; private set; }

        /// <summary>
        /// The inner value of the variable.
        /// </summary>
        protected T _value;

        /// <summary>
        /// Triggered when <see cref="SetValue"/> is called.
        /// </summary>
        public event EventHandler<T> ValueChanged;

        
        /// <summary>
        /// The the value of the variable
        /// </summary>
        public void SetValue(T value)
        {
            _value = value;
            IsValueSet = true;
            ValueChanged?.Invoke(this, value);
        }

        /// <summary>
        /// Returns the value of the variable
        /// </summary>
        /// <remarks>If <see cref="IsValueSet"/> is false then the default value will be returned.</remarks>
        /// <returns></returns>
        public T GetValue()
        {
            return _value;
        }

        /// <summary>
        /// Returns the value of the variable if its value was set
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool TryGetValue(out T value)
        {
            value = IsValueSet ? _value : default(T);
            return IsValueSet;
        }
    }
}
