using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Coddee.Attributes;
using Microsoft.AspNetCore.Http;

namespace Coddee.AspNet
{
    /// <summary>
    /// Validates that a client have sufficient permissions to call a dynamic API action.
    /// </summary>
    public interface IAuthorizationValidator
    {
        /// <summary>
        /// Checks if the caller have sufficient permissions to call a dynamic API action.
        /// </summary>
        bool IsAuthorized(IDynamicApiAction action, DynamicApiRequest request);
    }
}
