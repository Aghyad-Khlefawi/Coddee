// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

namespace Coddee.Security
{
    /// <summary>
    /// Define the possible results of an authentication operation.
    /// </summary>
    public enum AuthenticationStatus
    {
        /// <summary>
        /// The user is valid.
        /// </summary>
        Successfull,

        /// <summary>
        /// The provided credentials do not belong to a valid user.
        /// </summary>
        InvalidCredentials,

        /// <summary>
        /// The user is locked from authenticating.
        /// </summary>
        Locked,

        /// <summary>
        /// An error occurred while authenticating.
        /// </summary>
        Failed
    }
}
