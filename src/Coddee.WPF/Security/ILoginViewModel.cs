// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using Coddee.Mvvm;
using Coddee.Security;

namespace Coddee.WPF.Security
{
    /// <summary>
    /// Login service.
    /// </summary>
    public interface ILoginViewModel : IPresentableViewModel
    {
        /// <summary>
        /// Triggered when the login operation is completed.
        /// </summary>
        event EventHandler<AuthenticationResponse> LoggedIn;
    }
}