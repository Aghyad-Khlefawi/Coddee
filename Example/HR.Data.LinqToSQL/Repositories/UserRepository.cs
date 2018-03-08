// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using System.Linq;
using System.Threading.Tasks;
using Coddee;
using Coddee.Crypto;
using Coddee.Data;
using Coddee.Security;
using HR.Data.Models;
using HR.Data.Repositories;

namespace HR.Data.LinqToSQL.Repositories
{
    [Repository(typeof(IUserRepository))]
    public class UserRepository : HRRepositoryBase<DB.User>, IUserRepository
    {
        public override void RegisterMappings(IObjectMapper mapper)
        {
            base.RegisterMappings(mapper);
            mapper.RegisterMap<User, DB.User>();
        }

        public Task<HRAuthenticationResponse> AuthenticationUser(string username, string password)
        {
            return Execute((db, table) =>
            {
                try
                {
                    username = username.ToLower();
                    var user = table.FirstOrDefault(e => e.Username.ToLower() == username);
                    if (user == null || !PasswordHelper.ValidatePassword(password, user.PasswordSalt, user.PasswordHash))
                        return new HRAuthenticationResponse
                        {
                            Status = AuthenticationStatus.InvalidCredentials
                        };
                    return new HRAuthenticationResponse
                    {
                        Status = AuthenticationStatus.Successfull,
                        Username = user.Username
                    };
                }
                catch (Exception e)
                {
                    return new HRAuthenticationResponse
                    {
                        Error = e.Message,
                        Status = AuthenticationStatus.Failed
                    };
                }
            });
        }

        public Task CreateUser(User user, string password)
        {
            return Execute((db, table) =>
            {
                var dbUser = _mapper.Map<DB.User>(user);
                var pass = PasswordHelper.GenerateHashedPassword(password);
                dbUser.PasswordHash = pass.Password;
                dbUser.PasswordSalt = pass.Salt;

                table.InsertOnSubmit(dbUser);
            });
        }
    }
}