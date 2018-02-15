// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;

namespace Coddee
{
    /// <summary>
    /// Static math helper class
    /// </summary>
    public static class Math
    {
        /// <summary>
        /// Constrains a value between a max and a min value.
        /// </summary>
        public static T Clamp<T>(T min, T max, T value) where T : IComparable<T>
        {
            return value.CompareTo(min) < 0 ? min : value.CompareTo(max) > 0 ? max : value;
        }
    }
}
