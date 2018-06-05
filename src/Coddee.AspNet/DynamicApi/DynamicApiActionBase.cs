using System;
using System.Collections.Generic;
using System.Linq;
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
        private PropertyInfo _taskResult;

        /// <inheritdoc />
        public DynamicApiActionPath Path { get; set; }

        /// <inheritdoc />
        public List<DynamicApiActionParameter> Parameters { get; set; }

        /// <inheritdoc />
        public async Task<object> Invoke(DynamicApiActionParameterValue[] parametersValue)
        {
            object[] parameters = null;
            parametersValue = parametersValue.OrderBy(e => e.Parameter.Index).ToArray();
            var instance = GetInstnaceOwner();
            if (parametersValue.Any())
            {
                parameters = new object[parametersValue.Length];
                for (int i = 0; i < parametersValue.Length; i++)
                {
                    parameters[i] = parametersValue[i].Value;
                }
            }

            var result = Method.Invoke(instance, parameters);

            if (result is Task task)
            {
                await task;
                if (ReturnsValue)
                {
                    if (_taskResult == null)
                        _taskResult = result.GetType().GetProperty(nameof(Task<object>.Result));
                    result = _taskResult.GetValue(task);
                }
            }

            return result;
        }

        /// <summary>
        /// Get an instance of the owner type to invoke the action.
        /// </summary>
        /// <returns></returns>
        protected abstract object GetInstnaceOwner();
        
        /// <inheritdoc />
        public bool ReturnsValue { get; set; }

        /// <inheritdoc />
        public bool RequiresAuthorization { get; set; }

        /// <inheritdoc />
        public AuthorizeAttribute AuthorizeAttribute { get; set; }

        /// <summary>
        /// The action return type.
        /// </summary>
        public Type ReturnType { get; set; }


        MethodInfo _method;
        /// <summary>
        /// The method info object.
        /// </summary>
        public MethodInfo Method
        {
            get { return _method; }
            set
            {
                _method = value;
                SetAuthorizationInfo();
            }
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