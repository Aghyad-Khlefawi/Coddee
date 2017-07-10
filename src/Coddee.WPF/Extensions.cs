// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Practices.Unity;

namespace Coddee.WPF
{
    public static class Extensions
    {
        public static Task InitializeAll(this IEnumerable<IViewModel> items)
        {
            return Task.WhenAll(items.Select(e => e.Initialize()));
        }
    }
}