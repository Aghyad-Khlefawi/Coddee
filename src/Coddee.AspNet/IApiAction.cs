// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

namespace Coddee.AspNet
{
    public interface IApiAction
    {
        string Path { get; set; }
        Task<object> Invoke(IEnumerable<object> param);
        bool RetrunsValue { get; }
        ParameterInfo[] ParametersInfo { get; set; }
        bool RequiredAuthentication { get; set; }
        string Claim { get; set; }
    }
}