using System.Net.Http.Headers;
using System.Threading.Tasks;
using Coddee.Data;
using Coddee.Data.REST;
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

        public async Task<HRAuthenticationResponse> AuthenticationUser(string username, string password)
        {
            var res = await Post<HRAuthenticationResponse>(_controller,
                                                nameof(AuthenticationUser),
                                                new { Username = username, Password = password });
            if (res.Status == AuthenticationStatus.Successfull)
            {
                _httpClient.DefaultRequestHeaders.Authorization =
                    AuthenticationHeaderValue.Parse($"Bearer {res.AuthenticationToken}");
            }
            return res;
        }
    }
}
