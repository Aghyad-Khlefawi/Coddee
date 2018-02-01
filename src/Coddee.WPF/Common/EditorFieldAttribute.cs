using System;
using System.Collections.Generic;

namespace Coddee.WPF
{
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

    [AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
    public sealed class EditorFieldAttribute : Attribute
    {
        

        public EditorFieldAttribute()
        {
            
        }

        public EditorFieldAttribute(ClearAction clearAction)
        {
            ClearAction = clearAction;
        }

        public ClearAction ClearAction { get; set; }

    }
}