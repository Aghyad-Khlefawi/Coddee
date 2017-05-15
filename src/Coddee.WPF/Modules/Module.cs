// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;

namespace Coddee.WPF.Modules
{
    /// <summary>
    /// Used to discover modules on start up
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public sealed class ModuleAttribute : Attribute
    {
        public ModuleAttribute(string moduleName)
            : this(moduleName, ModuleInitializationTypes.Auto)
        {
        }

        public ModuleAttribute(string moduleName, ModuleInitializationTypes initializationTypes)
            : this(moduleName, initializationTypes, null)
        {
        }

        public ModuleAttribute(
            string moduleName,
            ModuleInitializationTypes initializationTypes,
            params string[] dependencies)
        {
            ModuleName = moduleName;
            InitializationTypes = initializationTypes;
            Dependencies = dependencies;
        }

        public string ModuleName { get; set; }
        public ModuleInitializationTypes InitializationTypes { get; set; }
        public string[] Dependencies { get; set; }
    }

    /// <summary>
    /// Represent the minimum information of an application module
    /// </summary>
    public class Module
    {
        public string Name { get; set; }
        public string[] Dependencies { get; set; }
        public ModuleInitializationTypes InitializationType { get; set; }
        public virtual Type Type { get; set; }
        public IModule Instance { get; set; }
        public bool Initialized { get; set; }
    }
}