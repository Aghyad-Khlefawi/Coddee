// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Coddee
{
    /// <summary>
    /// Base class for type that needs <see cref="INotifyPropertyChanged"/>
    /// </summary>
    public class BindableBase : INotifyPropertyChanged
    {
        /// <summary>
        /// Sets the field value to the new value then raise the PropertyChanged event
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="field">The field reference</param>
        /// <param name="value">the new value</param>
        /// <param name="propertyName">the property name</param>
        /// <returns></returns>
        protected virtual bool SetProperty<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (Equals(field, value)) return false;

            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        /// <inheritdoc />
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Raises the <see cref="PropertyChanged"/> event.
        /// </summary>
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}