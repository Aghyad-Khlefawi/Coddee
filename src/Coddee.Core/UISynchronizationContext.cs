// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using System.Threading;

namespace Coddee
{
    public static class UISynchronizationContext
    {
        private static SynchronizationContext _context;

        public static void SetContext(SynchronizationContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Execute an action on SynchornizationContext (supposedly the UI context)
        /// </summary>
        /// <param name="action">The action to execute</param>
        public static void ExecuteOnUIContext(Action action)
        {
            if (SynchronizationContext.Current == _context || _context == null)
                action();
            else
                _context.Send(obj => action(), null);
        }
    }
}