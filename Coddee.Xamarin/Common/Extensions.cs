using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coddee.Xamarin.Common
{
    public static class Extensions
    {
        public static Task InitializeAll(this IEnumerable<IViewModel> items, bool forceInitialization = false)
        {
            return Task.WhenAll(items.Where(e => forceInitialization || !e.IsInitialized).Select(e => e.Initialize()));
        }
    }
}