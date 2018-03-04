// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System.Reflection;

namespace Coddee.WPF
{
    /// <summary>
    /// Contains the information of an editor form field.
    /// </summary>
    public class EditorFieldInfo
    {
        /// <summary>
        /// The <see cref="EditorFieldAttribute"/> value.
        /// </summary>
        public EditorFieldAttribute Attribute { get; set; }

        /// <summary>
        /// The reflection information of the property.
        /// </summary>
        public PropertyInfo Property { get; set; }
    }
}
