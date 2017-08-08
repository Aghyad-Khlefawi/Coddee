using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Coddee.AspTest.Controllers;
using Coddee.Data;
using Coddee.Loggers;
using Coddee.Security;
using HR.Data.Models;
using HR.Data.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace HR.Web.Controllers
{
    public class UserController : ApiControllerBase<IUserRepository>
    {
        public class UserCreateArgs
        {
            public User User { get; set; }
            public string Password { get; set; }
        }
        public class AuthenticateUserArgs
        {
            public string Username { get; set; }
            public string Password { get; set; }
        }
        public UserController(IRepositoryManager repoManager, ILogger logger) : base(repoManager, logger)
        {
            
        }


        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] UserCreateArgs args)
        {
            try
            {
                await _repository.CreateUser(args.User, args.Password);
                return Ok();
            }
            catch (Exception e)
            {
                return Error(e);
            }
        }

        [HttpPost]
        public async Task<IActionResult> AuthenticationUser([FromBody] AuthenticateUserArgs args)
        {
            try
            {
                return Json(await _repository.AuthenticationUser(args.Username, args.Password));
            }
            catch (Exception e)
            {
                return Error(e);
            }
        }
    }
}