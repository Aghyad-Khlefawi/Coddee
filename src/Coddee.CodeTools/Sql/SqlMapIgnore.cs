// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;

namespace Coddee.CodeTools.Sql
{
    [AttributeUsage(AttributeTargets.All, Inherited = true, AllowMultiple = false)]
    public sealed class SqlMapIgnoreAttribute : Attribute
    {
        public SqlMapIgnoreAttribute()
        {
            
        }
    }
}
