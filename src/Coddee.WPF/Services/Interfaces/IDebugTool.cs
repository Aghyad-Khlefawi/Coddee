// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Coddee.Services
{
    public interface IDebugTool
    {

        Task Initialize(bool forceInitialize = false);

        /// <summary>
        /// Set the condition on which the tool visibility will be toggled
        /// </summary>
        /// <param name="toggleCondition"></param>
        void SetToggleCondition(Func<KeyEventArgs, bool> toggleCondition);

    }
}
