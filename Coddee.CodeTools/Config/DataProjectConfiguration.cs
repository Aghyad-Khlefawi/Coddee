// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

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
