// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;

namespace Coddee
{
    /// <summary>
    /// Used to discover modules on start up
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public sealed class ModuleAttribute : Attribute
    {
        /// <inheritdoc />
        public ModuleAttribute(string moduleName)
            : this(moduleName, ModuleInitializationTypes.Auto)
        {
        }

        /// <inheritdoc />
        public ModuleAttribute(string moduleName, ModuleInitializationTypes initializationTypes)
            : this(moduleName, initializationTypes, null)
        {
        }

        /// <inheritdoc />
        public ModuleAttribute(
            string moduleName,
            ModuleInitializationTypes initializationTypes,
            params string[] dependencies)
        {
            ModuleName = moduleName;
            InitializationTypes = initializationTypes;
            Dependencies = dependencies;
        }

       
        /// <summary>
        /// The name of the module
        /// </summary>
        public string ModuleName { get; set; }
        

        /// <summary>
        /// The initialization type of the module
        /// </summary>
        public ModuleInitializationTypes InitializationTypes { get; set; }

        /// <summary>
        /// The modules dependencies that needs to be registered before this module.
        /// </summary>
        public string[] Dependencies { get; set; }
    }

    /// <summary>
    /// Represent the minimum information of an application module
    /// </summary>
    public class Module
    {
        /// <summary>
        /// A new to identify the module
        /// </summary>
        public string Name { get; set; }
        
        /// <summary>
        /// The modules dependencies that needs to be registered before this module.
        /// </summary>
        public string[] Dependencies { get; set; }

        /// <summary>
        /// The initialization type of the module
        /// </summary>
        public ModuleInitializationTypes InitializationType { get; set; }

        /// <summary>
        /// The class type of the module.
        /// </summary>
        public virtual Type Type { get; set; }
        
        /// <summary>
        /// The registered instance of the module.
        /// </summary>
        public IModule Instance { get; set; }

        /// <summary>
        /// Indicates if the module is registered.
        /// </summary>
        public bool Initialized { get; set; }
    }
    
}