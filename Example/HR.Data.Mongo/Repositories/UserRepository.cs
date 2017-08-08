// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using System.Linq;
using System.Threading.Tasks;
using Coddee;
using Coddee.Crypto;
using Coddee.Data;
using Coddee.Data.MongoDB;
using Coddee.Security;
using HR.Data.Models;
using HR.Data.Repositories;
using MongoDB.Driver;

namespace HR.Data.Mongo.Repositories
{
    public class UserDBModel : IUniqueObject<Guid>
    {
        public Guid ID { get; set; }
        public string Username { get; set; }
        public Guid GetKey => ID;
        public string PasswordSalt { get; set; }
        public string PasswordHash { get; set; }
    }

    [Repository(typeof(IUserRepository))]
    public class UserRepository : MongoRepositoryBase<UserDBModel, Guid>, IUserRepository
    {
        public UserRepository() : base(HRMongoCollections.Users)
        {
        }

        public override void RegisterMappings(IObjectMapper mapper)
        {
            base.RegisterMappings(mapper);
            mapper.RegisterMap<User,UserDBModel>();
        }

        public async Task<HRAuthenticationResponse> AuthenticationUser(string username, string password)
        {
            try
            {
                username = username.ToLower();
                var user = (await _collection.Find(e => e.Username.ToLower() == username).ToListAsync())
                    .FirstOrDefault();
                if (user == null || !PasswordHelper.ValidatePassword(password, user.PasswordSalt, user.PasswordHash))
                    return new HRAuthenticationResponse
                    {
                        Status = AuthenticationStatus.InvalidCredinitals
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
        }

        public Task CreateUser(User user, string password)
        {
            var dbUser = _mapper.Map<UserDBModel>(user);
            var pass = PasswordHelper.GenerateHashedPassword(password);
            dbUser.PasswordHash = pass.Password;
            dbUser.PasswordSalt = pass.Salt;
            return _collection.InsertOneAsync(dbUser);
        }
    }
}