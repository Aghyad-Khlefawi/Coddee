using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Coddee.Data;
using Coddee.Data.Rest;
using Coddee.Security;
using HR.Data.Models;
using HR.Data.Repositories;

namespace HR.Data.REST.Repositories
{
    [Repository(typeof(IUserRepository))]
    public class UserRepository : RESTRepositoryBase, IUserRepository
    {
        private string _controller = "User";

        public Task CreateUser(User user, string password)
        {
            return Post(_controller,
                        nameof(CreateUser),
                        new
                        {
                            User = user,
                            Password = password
                        });
        }

        public Task<HRAuthenticationResponse> AuthenticationUser(string username, string password)
        {
            return Post<HRAuthenticationResponse>(_controller,
                                                nameof(AuthenticationUser),
                                                new {Username = username, Password = password});
        }
    }
}