using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coddee.CodeTools.Components
{
    public class DataProjectConfiguration:ProjectConfiguration
    {
        public static explicit operator DataProjectConfigurationSerializable(DataProjectConfiguration config)
        {
            return new DataProjectConfigurationSerializable
            {
                ProjectPath = config.ProjectPath,
                DefaultNamespace = config.DefaultNamespace,
                GeneratedCodeFolder = config.GeneratedCodeFolder
            };
        }
        public static explicit operator DataProjectConfiguration(DataProjectConfigurationSerializable config)
        {
            return new DataProjectConfiguration
            {
                ProjectPath = config.ProjectPath,
                DefaultNamespace = config.DefaultNamespace,
                GeneratedCodeFolder = config.GeneratedCodeFolder,
            };
        }
    }

    public class DataProjectConfigurationSerializable : ProjectConfigurationSerializable
    {
        
    }
}
