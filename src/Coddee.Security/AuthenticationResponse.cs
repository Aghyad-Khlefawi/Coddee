// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  


namespace Coddee.Security
{
    public class AuthenticationResponse
    {
        public string Username { get; set; }
        public AuthenticationStatus Status { get; set; }
        public string Error { get; set; }
        public string AuthenticationToken { get; set; }
    }
    public class AuthenticationResponse<TUser>: AuthenticationResponse
    {
        public TUser User { get; set; }
    }
}