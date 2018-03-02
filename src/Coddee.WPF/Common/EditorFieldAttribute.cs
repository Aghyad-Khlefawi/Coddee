// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using System.Collections.Generic;

namespace Coddee.WPF
{
    /// <summary>
    /// Type of operation used to clear a field.
    /// </summary>
    public enum ClearAction
    {
        /// <summary>
        /// Re-initialize the field to <see langword="default"/>
        /// </summary>
        Default,
        /// <summary>
        /// Keeps the field value
        /// </summary>
        None,
        /// <summary>
        /// Uses the clear method if the field is of type <see cref="ICollection{T}"/>
        /// </summary>
        Clear
    }

    /// <summary>
    /// Used to mark properties in an EditorViewModel class.
    /// Properties marked with this attribute will benefit from some functionalities like 
    /// automatically clearing fields and adding validation rules for the property.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
    public sealed class EditorFieldAttribute : Attribute
    {
        /// <inheritdoc />
        public EditorFieldAttribute()
        {
            
        }

        /// <inheritdoc />
        public EditorFieldAttribute(ClearAction clearAction)
        {
            ClearAction = clearAction;
        }

        /// <summary>
        /// The action to be taken on <see cref="EditorViewModelBase{TEditor,TView,TModel}.Clear"/>
        /// </summary>
        public ClearAction ClearAction { get; set; }
        /// <summary>
        /// When set to true an error validation rule will be created for the field.
        /// </summary>
        public bool IsRequired { get; set; }
    }
}