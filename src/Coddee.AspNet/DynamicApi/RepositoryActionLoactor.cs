using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
            DynamicApiRepositoryAction action = CreateAction(repositoryManager, repositoryName, actionName, method, interfaceMethod);
            return action;
        }

        private static DynamicApiRepositoryAction CreateAction(IRepositoryManager repositoryManager, string repositoryName, string actionName, MethodInfo method, MethodInfo interfaceMethod)
        {
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

        /// <summary>
        /// Create an <see cref="IDynamicApiAction"/> for a repository method.
        /// </summary>
        /// <returns></returns>
        public IDynamicApiAction CreateRepositoryAction(IRepositoryManager repositoryManager, IRepository repository, MethodInfo methodInfo, MethodInfo interfaceMethodInfo)
        {
            var repositoryName = repositoryManager.GetRepositoryName(repository);
            return CreateAction(repositoryManager, repositoryName, interfaceMethodInfo.Name.ToLower(), methodInfo, interfaceMethodInfo);
        }

        /// <summary>
        /// Returns the actions available in repository.
        /// </summary>
        public IEnumerable<IDynamicApiAction> GetRepositoryActions(IRepositoryManager repositoryManager, IRepository repository)
        {
            var repositoryType = repository.GetType();
            var interfaceType = repository.ImplementedInterface;

            const BindingFlags flags = BindingFlags.Public | BindingFlags.FlattenHierarchy | BindingFlags.Instance;
            var interfaceMethods = new List<MethodInfo>(interfaceType.GetMethods(flags)
                                                                     .Where(e => Attribute.IsDefined(e, typeof(ApiActionAttribute))));
            foreach (Type interf in interfaceType.GetInterfaces())
            {
                foreach (MethodInfo method in interf.GetMethods(flags)
                                                    .Where(e => Attribute.IsDefined(e, typeof(ApiActionAttribute))))
                    if (!interfaceMethods.Contains(method))
                        interfaceMethods.Add(method);
            }


            foreach (var interfaceMethodInfo in interfaceMethods)
            {
                var methodInfo = repositoryType.GetMethod(interfaceMethodInfo.Name);
                yield return CreateRepositoryAction(repositoryManager, repository, methodInfo, interfaceMethodInfo);
            }
        }
    }
}
