using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Coddee.Core.Auth
{
    public enum AuthenticationStatus
    {
        Error,
        Successful,
        Invalid,
        Disabled,
    }

    public interface IAuthenticationResponse
    {
        AuthenticationStatus Status { get; }
        string Message { get; }
    }

    public interface IAuthenticationRequest
    {
        string Username { get; }
        string Password { get; }
    }

    public interface IAuthProvider
    {
        Task<IAuthenticationResponse> Authenticate(IAuthenticationRequest request);
    }
}
