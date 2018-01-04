// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System.Xml;

namespace Coddee.CodeTools.Components.Data
{
    public class CsProject
    {
        public string Title { get; set; }

        public string ProjectPath { get; set; }
        public string Folder { get; set; }


        public string DefaultNamespace { get; set; }
    }
}
