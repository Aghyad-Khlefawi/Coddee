using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
