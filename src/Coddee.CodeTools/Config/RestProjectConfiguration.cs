// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

namespace Coddee.CodeTools.Components
{
    public class RestProjectConfiguration:ProjectConfiguration
    {
        public static explicit operator RestProjectConfigurationSerializable(RestProjectConfiguration config)
        {
            return new RestProjectConfigurationSerializable
            {
                ProjectPath = config.ProjectPath,
                DefaultNamespace = config.DefaultNamespace,
                GeneratedCodeFolder = config.GeneratedCodeFolder
            };
        }
        public static explicit operator RestProjectConfiguration(RestProjectConfigurationSerializable config)
        {
            return new RestProjectConfiguration
            {
                ProjectPath = config.ProjectPath,
                DefaultNamespace = config.DefaultNamespace,
                GeneratedCodeFolder = config.GeneratedCodeFolder,
            };
        }
    }

    public class RestProjectConfigurationSerializable : ProjectConfigurationSerializable
    {
        
    }
}
