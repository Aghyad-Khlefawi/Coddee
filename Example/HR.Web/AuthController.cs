// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Coddee;
using Coddee.AspNet;
using Coddee.Data;
using Coddee.Security;
using HR.Data;
using HR.Data.Repositories;
using Microsoft.IdentityModel.Tokens;

namespace HR.Web
{
    public class AuthController
    {
        private readonly IRepositoryManager _repositoryManager;
        private readonly IUserRepository _userRepository;
        public AuthController(IRepositoryManager repositoryManager)
        {
            _repositoryManager = repositoryManager;
            _userRepository = repositoryManager.GetRepository<IUserRepository>();

        }

        [ApiAction("User/AuthenticationUser")]
        public async Task<AuthenticationResponse> Authenticate(AuthRequest param)
        {
            var res = await _userRepository.AuthenticationUser(param.Username, param.Password);
            if (res.Status == AuthenticationStatus.Successfull)
                res.AuthenticationToken = CreateToken(new List<Claim> { new Claim("username", res.Username) }, DateTime.Now.AddDays(1));
            return res;
        }

        public static string CreateToken(IEnumerable<Claim> claims, DateTime expires)
        {
            try
            {
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Startup.Configuration["Tokens:Key"]));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                var token = new JwtSecurityToken(
                                                 Startup.Configuration["Tokens:Issuer"],
                                                 Startup.Configuration["Tokens:Audience"],
                                                 claims,
                                                 expires: expires,
                                                 signingCredentials: creds);
                return new JwtSecurityTokenHandler().WriteToken(token);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}