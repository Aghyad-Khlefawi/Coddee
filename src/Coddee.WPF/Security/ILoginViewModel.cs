// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using System.Threading.Tasks;
using Coddee.Security;

namespace Coddee.WPF.Security
{
    public interface ILoginViewModel : IPresentable
    {
        event EventHandler<AuthenticationResponse> LoggedIn;
        Task Initialize();
    }
}