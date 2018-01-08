﻿// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using Coddee.CodeTools.Components;
using Coddee.VsExtensibility;

namespace Coddee.CodeTools
{
    public class CoddeeSolutionInfo : SolutionInfo
    {
        public CoddeeSolutionInfo(VsHelper vsHelper) : base(vsHelper)
        {

        }

        public ModelProjectConfiguration ModelProjectConfiguration { get; set; }
        public DataProjectConfiguration DataProjectConfiguration { get; set; }
    }
}
