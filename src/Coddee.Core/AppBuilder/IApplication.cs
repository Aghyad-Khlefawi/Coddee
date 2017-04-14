// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coddee.AppBuilder
{
    public interface IApplication
    {
        Guid ApplicationID { get; }
        string ApplicationName { get; }
        ApplicationTypes ApplicationType { get; }
    }
}