using System;
using System.Reflection;
using System.Threading.Tasks;

namespace Coddee.AspNet
{
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

        /// <summary>
        /// Create an <see cref="ActionDelegate"/> instance
        /// </summary>
        public static ActionDelegate Create(MethodInfo method, Type[] args, object instance)
        {
            switch (args.Length)
            {
                case 1:
                    return (ActionDelegate)Activator.CreateInstance(typeof(ActionDelegate<>).MakeGenericType(args), method, instance);
                case 2:
                    return (ActionDelegate)Activator.CreateInstance(typeof(ActionDelegate<,>).MakeGenericType(args), method, instance);
                case 3:
                    return (ActionDelegate)Activator.CreateInstance(typeof(ActionDelegate<,,>).MakeGenericType(args), method, instance);
                case 4:
                    return (ActionDelegate)Activator.CreateInstance(typeof(ActionDelegate<,,,>).MakeGenericType(args), method, instance);
                case 5:
                    return (ActionDelegate)Activator.CreateInstance(typeof(ActionDelegate<,,,,>).MakeGenericType(args), method, instance);
                case 6:
                    return (ActionDelegate)Activator.CreateInstance(typeof(ActionDelegate<,,,,,>).MakeGenericType(args), method, instance);
                case 7:
                    return (ActionDelegate)Activator.CreateInstance(typeof(ActionDelegate<,,,,,,>).MakeGenericType(args), method, instance);
                case 8:
                    return (ActionDelegate)Activator.CreateInstance(typeof(ActionDelegate<,,,,,,,>).MakeGenericType(args), method, instance);
                case 9:
                    return (ActionDelegate)Activator.CreateInstance(typeof(ActionDelegate<,,,,,,,,>).MakeGenericType(args), method, instance);
                case 10:
                    return (ActionDelegate)Activator.CreateInstance(typeof(ActionDelegate<,,,,,,,,,>).MakeGenericType(args), method, instance);
                case 11:
                    return (ActionDelegate)Activator.CreateInstance(typeof(ActionDelegate<,,,,,,,,,,>).MakeGenericType(args), method, instance);
                case 12:
                    return (ActionDelegate)Activator.CreateInstance(typeof(ActionDelegate<,,,,,,,,,,,>).MakeGenericType(args), method, instance);
                case 13:
                    return (ActionDelegate)Activator.CreateInstance(typeof(ActionDelegate<,,,,,,,,,,,,>).MakeGenericType(args), method, instance);
                case 14:
                    return (ActionDelegate)Activator.CreateInstance(typeof(ActionDelegate<,,,,,,,,,,,,,>).MakeGenericType(args), method, instance);
                case 15:
                    return (ActionDelegate)Activator.CreateInstance(typeof(ActionDelegate<,,,,,,,,,,,,,,>).MakeGenericType(args), method, instance);
                case 16:
                    return (ActionDelegate)Activator.CreateInstance(typeof(ActionDelegate<,,,,,,,,,,,,,,,>).MakeGenericType(args), method, instance);
                case 17:
                    return (ActionDelegate)Activator.CreateInstance(typeof(ActionDelegate<,,,,,,,,,,,,,,,,>).MakeGenericType(args), method, instance);
            }

            return null;
        }
    }

    /// <inheritdoc />
    public abstract class ActionFuncDelegate<TFunc> : ActionDelegate
    {
        /// <inheritdoc />
        protected ActionFuncDelegate(MethodInfo method, object instance)
            : base(method)
        {
            object del = method.CreateDelegate(typeof(TFunc), instance);
            _delegate = (TFunc)del;
        }

        /// <summary>
        /// Internal delegate;
        /// </summary>
        protected readonly TFunc _delegate;
    }

    /// <inheritdoc />
    public class ActionDelegate<TReturn> : ActionFuncDelegate<Func<TReturn>>
    {
        /// <inheritdoc />
        public ActionDelegate(MethodInfo method, object instance)
        : base(method, instance)
        {
        }

        /// <inheritdoc />
        public override object InvokeDelegate(DynamicApiActionParameterValue[] param)
        {
            return _delegate();
        }
    }

    /// <inheritdoc />
    public class ActionDelegate<T1, TReturn> : ActionFuncDelegate<Func<T1, TReturn>>
    {
        /// <inheritdoc />
        public ActionDelegate(MethodInfo method, object instance)
        : base(method, instance)
        {
        }

        /// <inheritdoc />
        public override object InvokeDelegate(DynamicApiActionParameterValue[] param)
        {
            return _delegate((T1)param[0].Value);
        }
    }

    /// <inheritdoc />
    public class ActionDelegate<T1, T2, TReturn> : ActionFuncDelegate<Func<T1, T2, TReturn>>
    {
        /// <inheritdoc />
        public ActionDelegate(MethodInfo method, object instance)
        : base(method, instance)
        {
        }

        /// <inheritdoc />
        public override object InvokeDelegate(DynamicApiActionParameterValue[] param)
        {
            return _delegate((T1)param[0].Value, (T2)param[1].Value);
        }
    }

    /// <inheritdoc />
    public class ActionDelegate<T1, T2, T3, TReturn> : ActionFuncDelegate<Func<T1, T2, T3, TReturn>>
    {
        /// <inheritdoc />
        public ActionDelegate(MethodInfo method, object instance)
        : base(method, instance)
        {
        }

        /// <inheritdoc />
        public override object InvokeDelegate(DynamicApiActionParameterValue[] param)
        {
            return _delegate((T1)param[0].Value, (T2)param[1].Value, (T3)param[2].Value);
        }
    }

    /// <inheritdoc />
    public class ActionDelegate<T1, T2, T3, T4, TReturn> : ActionFuncDelegate<Func<T1, T2, T3, T4, TReturn>>
    {
        /// <inheritdoc />
        public ActionDelegate(MethodInfo method, object instance)
        : base(method, instance)
        {
        }

        /// <inheritdoc />
        public override object InvokeDelegate(DynamicApiActionParameterValue[] param)
        {
            return _delegate((T1)param[0].Value, (T2)param[1].Value, (T3)param[2].Value, (T4)param[3].Value);
        }
    }

    /// <inheritdoc />
    public class ActionDelegate<T1, T2, T3, T4, T5, TReturn> : ActionFuncDelegate<Func<T1, T2, T3, T4, T5, TReturn>>
    {
        /// <inheritdoc />
        public ActionDelegate(MethodInfo method, object instance)
        : base(method, instance)
        {
        }

        /// <inheritdoc />
        public override object InvokeDelegate(DynamicApiActionParameterValue[] param)
        {
            return _delegate((T1)param[0].Value, (T2)param[1].Value, (T3)param[2].Value, (T4)param[3].Value, (T5)param[4].Value);
        }
    }

    /// <inheritdoc />
    public class ActionDelegate<T1, T2, T3, T4, T5, T6, TReturn> : ActionFuncDelegate<Func<T1, T2, T3, T4, T5, T6, TReturn>>
    {
        /// <inheritdoc />
        public ActionDelegate(MethodInfo method, object instance)
            : base(method, instance)
        {
        }

        /// <inheritdoc />
        public override object InvokeDelegate(DynamicApiActionParameterValue[] param)
        {
            return _delegate((T1)param[0].Value, (T2)param[1].Value, (T3)param[2].Value, (T4)param[3].Value, (T5)param[4].Value, (T6)param[5].Value);
        }
    }

    /// <inheritdoc />
    public class ActionDelegate<T1, T2, T3, T4, T5, T6, T7, TReturn> : ActionFuncDelegate<Func<T1, T2, T3, T4, T5, T6, T7, TReturn>>
    {
        /// <inheritdoc />
        public ActionDelegate(MethodInfo method, object instance)
            : base(method, instance)
        {
        }

        /// <inheritdoc />
        public override object InvokeDelegate(DynamicApiActionParameterValue[] param)
        {
            return _delegate((T1)param[0].Value, (T2)param[1].Value, (T3)param[2].Value, (T4)param[3].Value, (T5)param[4].Value, (T6)param[5].Value, (T7)param[6].Value);
        }
    }
    /// <inheritdoc />
    public class ActionDelegate<T1, T2, T3, T4, T5, T6, T7, T8, TReturn> : ActionFuncDelegate<Func<T1, T2, T3, T4, T5, T6, T7, T8, TReturn>>
    {
        /// <inheritdoc />
        public ActionDelegate(MethodInfo method, object instance)
            : base(method, instance)
        {
        }

        /// <inheritdoc />
        public override object InvokeDelegate(DynamicApiActionParameterValue[] param)
        {
            return _delegate((T1)param[0].Value, (T2)param[1].Value, (T3)param[2].Value, (T4)param[3].Value, (T5)param[4].Value, (T6)param[5].Value, (T7)param[6].Value, (T8)param[7].Value);
        }
    }
    /// <inheritdoc />
    public class ActionDelegate<T1, T2, T3, T4, T5, T6, T7, T8, T9, TReturn> : ActionFuncDelegate<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TReturn>>
    {
        /// <inheritdoc />
        public ActionDelegate(MethodInfo method, object instance)
            : base(method, instance)
        {
        }

        /// <inheritdoc />
        public override object InvokeDelegate(DynamicApiActionParameterValue[] param)
        {
            return _delegate((T1)param[0].Value, (T2)param[1].Value, (T3)param[2].Value, (T4)param[3].Value, (T5)param[4].Value, (T6)param[5].Value, (T7)param[6].Value, (T8)param[7].Value, (T9)param[8].Value);
        }
    }
    /// <inheritdoc />
    public class ActionDelegate<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TReturn> : ActionFuncDelegate<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TReturn>>
    {
        /// <inheritdoc />
        public ActionDelegate(MethodInfo method, object instance)
            : base(method, instance)
        {
        }

        /// <inheritdoc />
        public override object InvokeDelegate(DynamicApiActionParameterValue[] param)
        {
            return _delegate((T1)param[0].Value, (T2)param[1].Value, (T3)param[2].Value, (T4)param[3].Value, (T5)param[4].Value, (T6)param[5].Value, (T7)param[6].Value, (T8)param[7].Value, (T9)param[8].Value, (T10)param[9].Value);
        }
    }
    /// <inheritdoc />
    public class ActionDelegate<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TReturn> : ActionFuncDelegate<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TReturn>>
    {
        /// <inheritdoc />
        public ActionDelegate(MethodInfo method, object instance)
            : base(method, instance)
        {
        }

        /// <inheritdoc />
        public override object InvokeDelegate(DynamicApiActionParameterValue[] param)
        {
            return _delegate((T1)param[0].Value, (T2)param[1].Value, (T3)param[2].Value, (T4)param[3].Value, (T5)param[4].Value, (T6)param[5].Value, (T7)param[6].Value, (T8)param[7].Value, (T9)param[8].Value, (T10)param[9].Value, (T11)param[10].Value);
        }
    }
    /// <inheritdoc />
    public class ActionDelegate<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TReturn> : ActionFuncDelegate<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TReturn>>
    {
        /// <inheritdoc />
        public ActionDelegate(MethodInfo method, object instance)
            : base(method, instance)
        {
        }

        /// <inheritdoc />
        public override object InvokeDelegate(DynamicApiActionParameterValue[] param)
        {
            return _delegate((T1)param[0].Value, (T2)param[1].Value, (T3)param[2].Value, (T4)param[3].Value, (T5)param[4].Value, (T6)param[5].Value, (T7)param[6].Value, (T8)param[7].Value, (T9)param[8].Value, (T10)param[9].Value, (T11)param[10].Value, (T12)param[11].Value);
        }
    }
    /// <inheritdoc />
    public class ActionDelegate<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TReturn> : ActionFuncDelegate<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TReturn>>
    {
        /// <inheritdoc />
        public ActionDelegate(MethodInfo method, object instance)
            : base(method, instance)
        {
        }

        /// <inheritdoc />
        public override object InvokeDelegate(DynamicApiActionParameterValue[] param)
        {
            return _delegate((T1)param[0].Value, (T2)param[1].Value, (T3)param[2].Value, (T4)param[3].Value, (T5)param[4].Value, (T6)param[5].Value, (T7)param[6].Value, (T8)param[7].Value, (T9)param[8].Value, (T10)param[9].Value, (T11)param[10].Value, (T12)param[11].Value, (T13)param[12].Value);
        }
    }
    /// <inheritdoc />
    public class ActionDelegate<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TReturn> : ActionFuncDelegate<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TReturn>>
    {
        /// <inheritdoc />
        public ActionDelegate(MethodInfo method, object instance)
            : base(method, instance)
        {
        }

        /// <inheritdoc />
        public override object InvokeDelegate(DynamicApiActionParameterValue[] param)
        {
            return _delegate((T1)param[0].Value, (T2)param[1].Value, (T3)param[2].Value, (T4)param[3].Value, (T5)param[4].Value, (T6)param[5].Value, (T7)param[6].Value, (T8)param[7].Value, (T9)param[8].Value, (T10)param[9].Value, (T11)param[10].Value, (T12)param[11].Value, (T13)param[12].Value, (T14)param[13].Value);
        }
    }
    /// <inheritdoc />
    public class ActionDelegate<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TReturn> : ActionFuncDelegate<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TReturn>>
    {
        /// <inheritdoc />
        public ActionDelegate(MethodInfo method, object instance)
            : base(method, instance)
        {
        }

        /// <inheritdoc />
        public override object InvokeDelegate(DynamicApiActionParameterValue[] param)
        {
            return _delegate((T1)param[0].Value, (T2)param[1].Value, (T3)param[2].Value, (T4)param[3].Value, (T5)param[4].Value, (T6)param[5].Value, (T7)param[6].Value, (T8)param[7].Value, (T9)param[8].Value, (T10)param[9].Value, (T11)param[10].Value, (T12)param[11].Value, (T13)param[12].Value, (T14)param[13].Value, (T15)param[14].Value);
        }
    }
    /// <inheritdoc />
    public class ActionDelegate<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TReturn> : ActionFuncDelegate<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TReturn>>
    {
        /// <inheritdoc />
        public ActionDelegate(MethodInfo method, object instance)
            : base(method, instance)
        {
        }

        /// <inheritdoc />
        public override object InvokeDelegate(DynamicApiActionParameterValue[] param)
        {
            return _delegate((T1)param[0].Value, (T2)param[1].Value, (T3)param[2].Value, (T4)param[3].Value, (T5)param[4].Value, (T6)param[5].Value, (T7)param[6].Value, (T8)param[7].Value, (T9)param[8].Value, (T10)param[9].Value, (T11)param[10].Value, (T12)param[11].Value, (T13)param[12].Value, (T14)param[13].Value, (T15)param[14].Value, (T16)param[15].Value);
        }
    }

}