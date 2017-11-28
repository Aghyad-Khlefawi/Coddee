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
                TaskResult = ReturnType.GetProperty("Result");
        }

        public DelegateAction(string path, object owner, MethodInfo method, ParameterInfo[] parametersInfo)
            : this(path, owner, method)
        {
            ParametersInfo = parametersInfo.Select(ActionParameter.Create);
        }

        public Delegate Delegate { get; set; }
        public object Owner { get; set; }
        public MethodInfo Method { get; set; }
        public IEnumerable<ActionParameter> ParametersInfo { get; set; }
        public Type ReturnType { get; set; }
        public bool RetrunsValue { get; set; }
        public string Path { get; set; }
        public bool RequiredAuthentication { get; set; }
        public string Claim { get; set; }

        private PropertyInfo TaskResult { get; set; }

        public async Task<object> Invoke(IEnumerable<object> param)
        {
            if (!RetrunsValue)
                await (Task)Method.Invoke(Owner, param.ToArray());
            else
            {
                var task = ((Task)Method.Invoke(Owner, param.ToArray()));
                return TaskResult.GetValue(task);
            }
            return null;
        }

    }
}