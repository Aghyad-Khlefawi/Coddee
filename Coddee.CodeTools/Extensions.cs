// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System.Xml;
using Coddee.CodeTools.Components.Data;

namespace Coddee.CodeTools
{
    public static class Extensions
    {
        public static string GetAssemblyName(this CsProject csProject)
        {
            var xml = new XmlDocument();
            xml.Load(csProject.ProjectPath);
            foreach (XmlElement propertyGroup in xml.GetElementsByTagName("PropertyGroup"))
            {
                var rootNamespace = propertyGroup.GetElementsByTagName("AssemblyName");
                if (rootNamespace.Count > 0)
                {
                    return rootNamespace.Item(0).LastChild.Value;
                }
            }
            return null;
        }
    }
}
