// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

namespace Coddee.AspNet
{

    public class ActionParameter
    {
        public Type Type { get; set; }
        public string Name { get; set; }

        public static ActionParameter Create(ParameterInfo arg)
        {
            return new ActionParameter
            {
                Name = arg.Name.ToLower(),
                Type = arg.ParameterType
            };
        }
    }
    public interface IApiAction
    {
        string RepositoryName { get; }
        string Path { get; set; }
        Task<object> Invoke(object target, IEnumerable<object> param);
        Task<object> Invoke(IEnumerable<object> param);
        bool RetrunsValue { get; }
        List<ActionParameter> ParametersInfo { get; set; }
        bool RequiredAuthentication { get; set; }
        string Claim { get; set; }
        object Owner { get; set; }
    }
}