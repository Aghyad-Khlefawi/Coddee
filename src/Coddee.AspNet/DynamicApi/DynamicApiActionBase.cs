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
            _delegate = CreateDelegateInstance(args, instance);
        }

        private ActionDelegate CreateDelegateInstance(Type[] args, object instance)
        {
            switch (args.Length)
            {
                case 1:
                    return (ActionDelegate)Activator.CreateInstance(typeof(ActionDelegate<>).MakeGenericType(args), Method, instance);
                case 2:
                    return (ActionDelegate)Activator.CreateInstance(typeof(ActionDelegate<,>).MakeGenericType(args), Method, instance);
                case 3:
                    return (ActionDelegate)Activator.CreateInstance(typeof(ActionDelegate<,,>).MakeGenericType(args), Method, instance);
                case 4:
                    return (ActionDelegate)Activator.CreateInstance(typeof(ActionDelegate<,,,>).MakeGenericType(args), Method, instance);
                case 5:
                    return (ActionDelegate)Activator.CreateInstance(typeof(ActionDelegate<,,,,>).MakeGenericType(args), Method, instance);
                case 6:
                    return (ActionDelegate)Activator.CreateInstance(typeof(ActionDelegate<,,,,,>).MakeGenericType(args), Method, instance);
            }

            return null;
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

    /// <summary>
    /// Generated delegate for an API action
    /// </summary>
    public abstract class ActionDelegate
    {
        /// <inheritdoc />
        protected ActionDelegate(MethodInfo method)
        {
            _method = method;
            _returnsValue = _method.ReturnType != typeof(Task) && _method.ReturnType != typeof(void);
        }

        /// <summary>
        /// The encapsulated method.
        /// </summary>
        protected readonly MethodInfo _method;
        private PropertyInfo _taskResult;

        /// <summary>
        /// Indicates that the method returns a value.
        /// </summary>
        protected bool _returnsValue;

        /// <summary>
        /// Invoke the delegate.
        /// </summary>
        /// <returns></returns>
        public async Task<object> Invoke(DynamicApiActionParameterValue[] param)
        {
            var result = InvokeDelegate(param);
            if (result is Task task)
            {
                await task;
                if (_returnsValue)
                {
                    if (_taskResult == null)
                        _taskResult = result.GetType().GetProperty(nameof(Task<object>.Result));
                    result = _taskResult.GetValue(task);
                }
            }
            return result;
        }

        /// <summary>
        /// Invoke the internal delegate
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public abstract object InvokeDelegate(DynamicApiActionParameterValue[] param);
    }

    /// <inheritdoc />
    public class ActionDelegate<TReturn> : ActionDelegate
    {
        /// <inheritdoc />
        public ActionDelegate(MethodInfo method, object instance)
        : base(method)
        {
            _delegate = (Func<TReturn>)method.CreateDelegate(typeof(Func<TReturn>), instance);
        }

        private readonly Func<TReturn> _delegate;


        /// <inheritdoc />
        public override object InvokeDelegate(DynamicApiActionParameterValue[] param)
        {
            return _delegate();
        }
    }

    /// <inheritdoc />
    public class ActionDelegate<T1, TReturn> : ActionDelegate
    {
        /// <inheritdoc />
        public ActionDelegate(MethodInfo method, object instance)
        : base(method)
        {
            _delegate = (Func<T1, TReturn>)method.CreateDelegate(typeof(Func<T1, TReturn>), instance);
        }

        private readonly Func<T1, TReturn> _delegate;

        /// <inheritdoc />
        public override object InvokeDelegate(DynamicApiActionParameterValue[] param)
        {
            return _delegate((T1)param[0].Value);
        }
    }

    /// <inheritdoc />
    public class ActionDelegate<T1, T2, TReturn> : ActionDelegate
    {
        /// <inheritdoc />
        public ActionDelegate(MethodInfo method, object instance)
        : base(method)
        {
            _delegate = (Func<T1, T2, TReturn>)method.CreateDelegate(typeof(Func<T1, T2, TReturn>), instance);
        }

        private readonly Func<T1, T2, TReturn> _delegate;

        /// <inheritdoc />
        public override object InvokeDelegate(DynamicApiActionParameterValue[] param)
        {
            return _delegate((T1)param[0].Value, (T2)param[1].Value);
        }
    }

    /// <inheritdoc />
    public class ActionDelegate<T1, T2, T3, TReturn> : ActionDelegate
    {
        /// <inheritdoc />
        public ActionDelegate(MethodInfo method, object instance)
        : base(method)
        {
            _delegate = (Func<T1, T2, T3, TReturn>)method.CreateDelegate(typeof(Func<T1, T2, T3, TReturn>), instance);
        }

        private readonly Func<T1, T2, T3, TReturn> _delegate;

        /// <inheritdoc />
        public override object InvokeDelegate(DynamicApiActionParameterValue[] param)
        {
            return _delegate((T1)param[0].Value, (T2)param[1].Value, (T3)param[2].Value);
        }
    }

    /// <inheritdoc />
    public class ActionDelegate<T1, T2, T3, T4, TReturn> : ActionDelegate
    {
        /// <inheritdoc />
        public ActionDelegate(MethodInfo method, object instance)
        : base(method)
        {
            _delegate = (Func<T1, T2, T3, T4, TReturn>)method.CreateDelegate(typeof(Func<T1, T2, T3, T4, TReturn>), instance);
        }

        private readonly Func<T1, T2, T3, T4, TReturn> _delegate;

        /// <inheritdoc />
        public override object InvokeDelegate(DynamicApiActionParameterValue[] param)
        {
            return _delegate((T1)param[0].Value, (T2)param[1].Value, (T3)param[2].Value, (T4)param[3].Value);
        }
    }

    /// <inheritdoc />
    public class ActionDelegate<T1, T2, T3, T4, T5, TReturn> : ActionDelegate
    {
        /// <inheritdoc />
        public ActionDelegate(MethodInfo method, object instance)
        : base(method)
        {
            _delegate = (Func<T1, T2, T3, T4, T5, TReturn>)method.CreateDelegate(typeof(Func<T1, T2, T3, T4, T5, TReturn>), instance);
        }

        private readonly Func<T1, T2, T3, T4, T5, TReturn> _delegate;

        /// <inheritdoc />
        public override object InvokeDelegate(DynamicApiActionParameterValue[] param)
        {
            return _delegate((T1)param[0].Value, (T2)param[1].Value, (T3)param[2].Value, (T4)param[3].Value, (T5)param[4].Value);
        }
    }

}