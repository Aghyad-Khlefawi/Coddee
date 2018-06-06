using System.Collections.Generic;
using System.Threading.Tasks;
using Coddee.Attributes;

namespace Coddee.AspNet
{
    /// <summary>
    /// Represents an action that can be invoked by the <see cref="DynamicApi"/>
    /// </summary>
    public interface IDynamicApiAction
    {
        /// <summary>
        /// The route path of the action.
        /// </summary>
        DynamicApiActionPath Path { get; set; }

        /// <summary>
        /// Action parameters.
        /// </summary>
        List<DynamicApiActionParameter> Parameters { get; set; }

        /// <summary>
        /// Invoke the action.
        /// </summary>
        Task<object> Invoke(DynamicApiActionParameterValue[] parametersValue);
        
        /// <summary>
        /// Indicates whether the client require a specific permission to call the action.
        /// </summary>
        bool RequiresAuthorization { get; set; }

        /// <summary>
        /// The authorization required by the user.
        /// <remarks>Null if <see cref="RequiresAuthorization"/> is false.</remarks>
        /// </summary>
        AuthorizeAttribute AuthorizeAttribute { get; set; }
    }
}