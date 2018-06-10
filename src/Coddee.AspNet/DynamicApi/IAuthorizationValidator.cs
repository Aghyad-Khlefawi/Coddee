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
