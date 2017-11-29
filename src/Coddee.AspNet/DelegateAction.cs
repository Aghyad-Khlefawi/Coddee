// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Coddee.AspNet
{
    public class DelegateAction : IApiAction
    {
        public DelegateAction(string path, object owner, MethodInfo method)
        {
            Path = path;
            Owner = owner;
            Method = method;
            ReturnType = method.ReturnType;
            RetrunsValue = ReturnType != typeof(Task);


            if (RetrunsValue)
            {
                TaskResult = ReturnType.GetProperty("Result");
            }
        }

        public DelegateAction(string path, object owner, MethodInfo method, ParameterInfo[] parametersInfo)
            : this(path, owner, method)
        {
            ParametersInfo = parametersInfo.Select(ActionParameter.Create).ToList();
        }

        private Func<Task> _delegate;

        public object Owner { get; set; }
        public MethodInfo Method { get; set; }
        public List<ActionParameter> ParametersInfo { get; set; }
        public Type ReturnType { get; set; }
        public bool RetrunsValue { get; set; }
        public string Path { get; set; }
        public bool RequiredAuthentication { get; set; }
        public string Claim { get; set; }

        protected PropertyInfo TaskResult { get; set; }

        public virtual async Task<object> Invoke(IEnumerable<object> param)
        {
            var res = InvokeDelegate(param);
            if (!RetrunsValue)
                await res;
            else
                return TaskResult.GetValue(res);

            return null;
        }

        protected virtual Task InvokeDelegate(IEnumerable<object> param)
        {
            try
            {
                if (_delegate == null)
                    _delegate = (Func<Task>)Method.CreateDelegate(typeof(Func<Task>),Owner);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

            return _delegate.Invoke();
        }

        public static DelegateAction CreateDelegateAction(object controller, MethodInfo memberInfo, string pathLower)
        {
            var parameters = memberInfo.GetParameters();
            var type = GetFuncType(parameters);
            if (type != null)
            {
                if (parameters.Length == 0)
                {
                    return (DelegateAction)Activator.CreateInstance(typeof(DelegateAction), pathLower, controller, memberInfo, parameters);
                }

                var delegateActionType = type.MakeGenericType(parameters.Select(e => e.ParameterType).ToArray());
                return (DelegateAction)Activator.CreateInstance(delegateActionType, pathLower, controller, memberInfo, parameters);
            }
            return null;
        }
        protected static Type GetFuncType(ParameterInfo[] parametersInfo)
        {
            switch (parametersInfo.Length)
            {
                case 0:
                    return typeof(DelegateAction);
                case 1:
                    return typeof(DelegateAction<>);
                case 2:
                    return typeof(DelegateAction<,>);
                case 3:
                    return typeof(DelegateAction<,,>);
                case 4:
                    return typeof(DelegateAction<,,,>);
                    //case 5:
                    //    return typeof(DelegateAction<,,,,>);
                    //case 6:
                    //    return typeof(DelegateAction<,,,,,>);
                    //case 7:
                    //    return typeof(DelegateAction<,,,,,,>);
                    //case 8:
                    //    return typeof(DelegateAction<,,,,,,,>);
                    //case 9:
                    //    return typeof(DelegateAction<,,,,,,,,>);
                    //case 10:
                    //    return typeof(DelegateAction<,,,,,,,,,>);
                    //case 11:
                    //    return typeof(DelegateAction<,,,,,,,,,,>);
                    //case 12:
                    //    return typeof(DelegateAction<,,,,,,,,,,,>);
                    //case 13:
                    //    return typeof(DelegateAction<,,,,,,,,,,,,>);
                    //case 14:
                    //    return typeof(DelegateAction<,,,,,,,,,,,,,>);
                    //case 15:
                    //    return typeof(DelegateAction<,,,,,,,,,,,,,,>);
                    //case 16:
                    //    return typeof(DelegateAction<,,,,,,,,,,,,,,,>);
            }
            return null;
        }
    }

    public class DelegateAction<T1> : DelegateAction
    {
        public DelegateAction(string path, object owner, MethodInfo method) : base(path, owner, method)
        {
        }

        public DelegateAction(string path, object owner, MethodInfo method, ParameterInfo[] parametersInfo) : base(path, owner, method, parametersInfo)
        {
            _delegate = (Func<T1, Task>)method.CreateDelegate(typeof(Func<T1, Task>), owner);
        }

        private readonly Func<T1, Task> _delegate;

        protected override Task InvokeDelegate(IEnumerable<object> param)
        {
            return _delegate.Invoke((T1)param.ElementAt(0));
        }
    }
    public class DelegateAction<T1, T2> : DelegateAction
    {
        public DelegateAction(string path, object owner, MethodInfo method) : base(path, owner, method)
        {
        }

        public DelegateAction(string path, object owner, MethodInfo method, ParameterInfo[] parametersInfo) : base(path, owner, method, parametersInfo)
        {
            _delegate = (Func<T1, T2, Task>)method.CreateDelegate(typeof(Func<T1, T2, Task>), owner);
        }

        private readonly Func<T1, T2, Task> _delegate;

        protected override Task InvokeDelegate(IEnumerable<object> param)
        {
            return _delegate.Invoke((T1)param.ElementAt(0), (T2)param.ElementAt(1));
        }
    }
    public class DelegateAction<T1, T2, T3> : DelegateAction
    {
        public DelegateAction(string path, object owner, MethodInfo method) : base(path, owner, method)
        {
        }

        public DelegateAction(string path, object owner, MethodInfo method, ParameterInfo[] parametersInfo) : base(path, owner, method, parametersInfo)
        {
            _delegate = (Func<T1, T2, T3, Task>)method.CreateDelegate(typeof(Func<T1, T2, T3, Task>), owner);
        }

        private readonly Func<T1, T2, T3, Task> _delegate;

        protected override Task InvokeDelegate(IEnumerable<object> param)
        {
            return _delegate.Invoke((T1)param.ElementAt(0), (T2)param.ElementAt(1), (T3)param.ElementAt(2));
        }
    }
    public class DelegateAction<T1, T2, T3, T4> : DelegateAction
    {
        public DelegateAction(string path, object owner, MethodInfo method) : base(path, owner, method)
        {
        }

        public DelegateAction(string path, object owner, MethodInfo method, ParameterInfo[] parametersInfo) : base(path, owner, method, parametersInfo)
        {
            _delegate = (Func<T1, T2, T3, T4, Task>)method.CreateDelegate(typeof(Func<T1, T2, T3, T4, Task>), owner);
        }

        private readonly Func<T1, T2, T3, T4, Task> _delegate;

        protected override Task InvokeDelegate(IEnumerable<object> param)
        {
            return _delegate.Invoke((T1)param.ElementAt(0), (T2)param.ElementAt(1), (T3)param.ElementAt(2), (T4)param.ElementAt(3));
        }
    }
}