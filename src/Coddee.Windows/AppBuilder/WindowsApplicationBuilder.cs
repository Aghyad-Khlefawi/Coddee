// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using System.Linq;
using Coddee.ModuleDefinitions;

namespace Coddee.AppBuilder
{
    /// <summary>
    /// Base class for windows applications builders.
    /// </summary>
    public abstract class WindowsApplicationBuilder : ApplicationBuilder
    {
        /// <inheritdoc />
        protected WindowsApplicationBuilder(IApplication app, IContainer container)
        :base(app,container)
        {
        }

     
        /// <summary>
        /// set the default application build steps.
        /// </summary>
        protected override void SetupDefaultBuildActions()
        {
            base.SetupDefaultBuildActions();
#if NET46
            if (!BuildActionsCoordinator.BuildActionExists(BuildActionsKeys.ConfigFile))
                this.UseConfigurationFile(AppDomain.CurrentDomain.BaseDirectory);
#endif
        }

        /// <summary>
        /// Get the default modules supported by a windows application.
        /// </summary>
        protected override Type[] GetDefaultModules()
        {
            return base.GetDefaultModules().Concat(WindowsModuleDefinitions.Modules).ToArray();
        }
    }
}
