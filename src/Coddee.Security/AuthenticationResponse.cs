// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  


namespace Coddee.Security
{
    /// <summary>
    /// The result of an authentication operation.
    /// </summary>
    public class AuthenticationResponse
    {
        /// <summary>
        /// The username of the authenticated user.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// The operation result.
        /// </summary>
        public AuthenticationStatus Status { get; set; }

        /// <summary>
        /// Error message in case the authentication failed.
        /// </summary>
        public string Error { get; set; }

        /// <summary>
        /// Authentication token string.
        /// </summary>
        public string AuthenticationToken { get; set; }
    }

    /// <inheritdoc />
    public class AuthenticationResponse<TUser>: AuthenticationResponse
    {
        /// <summary>
        /// The authenticated user.
        /// </summary>
        public TUser User { get; set; }
    }
}