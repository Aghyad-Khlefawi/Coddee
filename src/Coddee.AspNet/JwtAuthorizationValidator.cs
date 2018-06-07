using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coddee.AspNet
{
    /// <summary>
    /// <see cref="IAuthorizationValidator"/> implementation for Jwt
    /// </summary>
    public class JwtAuthorizationValidator : IAuthorizationValidator
    {
        /// <inheritdoc />
        public bool IsAuthorized(IDynamicApiAction action, DynamicApiRequest request)
        {
            var user = request.HttpContext.User;
            if (user.Identity.IsAuthenticated)
            {
                if (string.IsNullOrEmpty(action.AuthorizeAttribute.Claim))
                    return true;
                if (user.HasClaim(e => e.Type.Equals(action.AuthorizeAttribute.Claim)))
                    return true;
            }
            return false;
        }
    }
}
