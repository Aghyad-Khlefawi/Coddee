﻿// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System.Threading;
using System.Threading.Tasks;
using Coddee.WPF;

namespace HR.Clients.WPF.Settings
{
    public class SettingsViewModel : ViewModelBase<SettingsView>
    {
        protected override Task OnInitialization()
        {
            Thread.Sleep(5000);
            return base.OnInitialization();
        }
    }
}