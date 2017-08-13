// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System.Threading.Tasks;
using Coddee.Data;
using Coddee.Security;
using HR.Data.Models;

namespace HR.Data.Repositories
{
    public interface IUserRepository:IRepository, IAuthenticationProvider<HRAuthenticationResponse>
    {
        Task CreateUser(User user, string password);
    }
}
