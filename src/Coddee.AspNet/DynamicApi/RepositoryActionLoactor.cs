using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Coddee.Data;

namespace Coddee.AspNet
{
    /// <summary>
    /// Finds and create API actions for repositories.
    /// </summary>
    public class RepositoryActionLoactor
    {
        /// <summary>
        /// Creates an <see cref="IDynamicApiAction"/> for a repository action.
        /// </summary>
        public IDynamicApiAction CreateRepositoryAction(IRepositoryManager repositoryManager, DynamicApiRequest request)
        {
            if (repositoryManager == null)
                return null;

            var repositoryName = request.RequestedActionPath.RequestedController;
            var repository = repositoryManager.GetRepository(repositoryName);
            if (repository == null)
                return null;

            var actionName = request.RequestedActionPath.RequestedAction;
            if (actionName == "getitem")
                actionName = "get_item";

            var method = repository
                         .GetType()
                         .GetMethods()
                         .FirstOrDefault(e => e.Name.Equals(actionName, StringComparison.InvariantCultureIgnoreCase));

            var interfaceMethod = GetInterfaceMethod(repository, actionName);

            if (method == null || interfaceMethod == null)
                return null;

            var action = new DynamicApiRepositoryAction
            {
                RepositoryManager = repositoryManager,
                RepositoryName = repositoryName,
                Parameters = DynamicApiActionParameter.GetParameters(interfaceMethod),
                Path = new DynamicApiActionPath(repositoryName, actionName),
            };
            action.SetMethod(method);
            return action;
        }

        private static MethodInfo GetInterfaceMethod(IRepository repository, string actionName)
        {
            MethodInfo GetMethodFromType(Type type)
            {
                return type.GetMethods()
                       .FirstOrDefault(e => e.Name.Equals(actionName, StringComparison.InvariantCultureIgnoreCase));
            }

            var method = GetMethodFromType(repository.ImplementedInterface);
            if (method == null)
            {
                foreach (var interf in repository.ImplementedInterface.GetInterfaces())
                {
                    method = GetMethodFromType(interf);
                    if (method != null)
                        return method;
                }
            }
            return method;
        }
    }
}
