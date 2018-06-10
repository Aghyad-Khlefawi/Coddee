using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Coddee.Attributes;

namespace Coddee.AspNet
{
    /// <summary>
    /// A base implementation for <see cref="IDynamicApiAction"/>.
    /// </summary>
    public abstract class DynamicApiActionBase : IDynamicApiAction
    {
        private ActionDelegate _delegate;

        /// <inheritdoc />
        public DynamicApiActionPath Path { get; set; }

        /// <inheritdoc />
        public List<DynamicApiActionParameter> Parameters { get; set; }

        /// <inheritdoc />
        public Task<object> Invoke(DynamicApiActionParameterValue[] parametersValue)
        {
            return _delegate.Invoke(parametersValue);
        }

        /// <summary>
        /// Get an instance of the owner type to invoke the action.
        /// </summary>
        /// <returns></returns>
        protected abstract object GetInstnaceOwner();

        /// <inheritdoc />
        public bool RequiresAuthorization { get; set; }

        /// <inheritdoc />
        public AuthorizeAttribute AuthorizeAttribute { get; set; }

        /// <summary>
        /// The method info object.
        /// </summary>
        public MethodInfo Method { get; private set; }

        /// <summary>
        /// Set the method property.
        /// </summary>
        public void SetMethod(MethodInfo method)
        {
            Method = method;
            SetAuthorizationInfo();
            CreateDelegate();
        }

        private void CreateDelegate()
        {
            var args = new Type[Parameters.Count + 1];
            for (int i = 0; i < Parameters.Count; i++)
            {
                args[i] = Parameters[i].Type;
            }
            args[args.Length - 1] = Method.ReturnType;
            var instance = GetInstnaceOwner();
            _delegate = ActionDelegate.Create(Method, args, instance);
        }

        /// <summary>
        /// Sets the value of <see cref="RequiresAuthorization"/> and <see cref="AuthorizeAttribute"/>
        /// </summary>
        protected void SetAuthorizationInfo()
        {
            AuthorizeAttribute authAttribute = Method.GetCustomAttribute<AuthorizeAttribute>() ?? Method.DeclaringType.GetCustomAttribute<AuthorizeAttribute>();
            AllowAnonymousAttribute anonymousAttribute = Method.GetCustomAttribute<AllowAnonymousAttribute>();

            bool requireAuth = authAttribute != null;
            if (anonymousAttribute != null)
                requireAuth = false;

            RequiresAuthorization = requireAuth;
            AuthorizeAttribute = requireAuth ? authAttribute : null;
        }
    }



}