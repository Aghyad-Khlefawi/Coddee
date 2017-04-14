// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System.Threading.Tasks;

namespace Coddee.Security
{
    public interface IAuthenticationProvider<TResponse>
        where TResponse : AuthenticationResponse
    {
        Task<TResponse> AuthenticationUser(string username, string password);
    }
}