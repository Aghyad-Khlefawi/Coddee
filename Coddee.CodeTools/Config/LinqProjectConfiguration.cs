// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security;
using System.Security.Policy;
using Coddee.Data.LinqToSQL;
using Coddee.VsExtensibility;

namespace Coddee.CodeTools.Config
{
    public class LinqProjectConfiguration : ProjectConfiguration
    {

        private bool _useCutomCrudBase;
        public bool UseCutomCrudBase
        {
            get { return _useCutomCrudBase; }
            set { SetProperty(ref _useCutomCrudBase, value); }
        }

        private bool _useCutomReadBase;
        public bool UseCutomReadBase
        {
            get { return _useCutomReadBase; }
            set { SetProperty(ref _useCutomReadBase, value); }
        }
        private List<string> _linqBaseTypes;
        public List<string> LinqBaseTypes
        {
            get { return _linqBaseTypes; }
            set { SetProperty(ref _linqBaseTypes, value); }
        }

        private string _selectedLinqCrudBase;
        public string SelectedLinqCrudBase
        {
            get { return _selectedLinqCrudBase; }
            set { SetProperty(ref _selectedLinqCrudBase, value); }
        }

        private string _selectedLinqReadBase;
        public string SelectedLinqReadBase
        {
            get { return _selectedLinqReadBase; }
            set { SetProperty(ref _selectedLinqReadBase, value); }
        }

        protected override void OnProjectPathChanged()
        {
            base.OnProjectPathChanged();
            LinqBaseTypes = new List<string>(LoadLinqBaseTypes());
        }

        public static explicit operator LinqProjectConfigurationSerializable(LinqProjectConfiguration config)
        {
            return new LinqProjectConfigurationSerializable
            {
                ProjectPath = config.ProjectPath,
                DefaultNamespace = config.DefaultNamespace,
                GeneratedCodeFolder = config.GeneratedCodeFolder,
                SelectedLinqCrudBase = config.SelectedLinqCrudBase,
                SelectedLinqReadBase = config.SelectedLinqReadBase,
                UseCutomCrudBase = config.UseCutomCrudBase,
                UseCutomReadBase = config.UseCutomReadBase
            };
        }

        public static explicit operator LinqProjectConfiguration(LinqProjectConfigurationSerializable config)
        {
            var temp = new LinqProjectConfiguration
            {
                ProjectPath = config.ProjectPath,
                DefaultNamespace = config.DefaultNamespace,
                GeneratedCodeFolder = config.GeneratedCodeFolder,
                UseCutomCrudBase = config.UseCutomCrudBase,
                UseCutomReadBase = config.UseCutomReadBase,
            };
            if (temp.LinqBaseTypes != null && temp.LinqBaseTypes.Any())
            {
                if (!string.IsNullOrWhiteSpace(config.SelectedLinqCrudBase))
                {
                    temp.SelectedLinqCrudBase = temp.LinqBaseTypes.FirstOrDefault(e => e == config.SelectedLinqCrudBase);
                }
                if (!string.IsNullOrWhiteSpace(config.SelectedLinqReadBase))
                {
                    temp.SelectedLinqReadBase = temp.LinqBaseTypes.FirstOrDefault(e => e == config.SelectedLinqReadBase);
                }
            }
            return temp;
        }

        private DirectoryInfo _tempBin;
        private IEnumerable<string> LoadLinqBaseTypes()
        {
            var linqBaseRepositoryTypes = new List<string>();
            if (!string.IsNullOrWhiteSpace(ProjectPath))
            {
                var activeConfig = SolutionInfo.GetActiveConfiguration();
                var assemblyName = this.GetAssemblyName();
                if (!string.IsNullOrWhiteSpace(assemblyName))
                {
                    var bin = Path.Combine(Path.GetDirectoryName(ProjectPath), "bin", activeConfig);
                    var path = Path.Combine(bin, assemblyName + ".dll");

                    if (File.Exists(path))
                    {
                        _tempBin = new DirectoryInfo(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Coddee", "CoddeeTools", "tempBin",Guid.NewGuid().ToString()));
                        if (!_tempBin.Exists)
                            _tempBin.Create();
                        foreach (var file in _tempBin.GetFiles())
                        {
                            file.Delete();
                        }
                        foreach (var file in Directory.GetFiles(bin))
                        {
                            File.Copy(file, Path.Combine(_tempBin.FullName, Path.GetFileName(file)));
                        }

                        var assembly = Assembly.ReflectionOnlyLoadFrom(Path.Combine(_tempBin.FullName, assemblyName + ".dll"));
                        AppDomain.CurrentDomain.ReflectionOnlyAssemblyResolve += ReflectionOnlyAssemblyResolve;
                        foreach (var type in assembly.GetTypes())
                        {
                            if (type.IsGenericType && type.BaseType.GetGenericTypeDefinition().FullName == typeof(CRUDLinqRepositoryBase<,,,>).FullName ||
                                type.IsGenericType && type.BaseType.GetGenericTypeDefinition().FullName == typeof(ReadOnlyLinqRepositoryBase<,,,>).FullName ||
                                type.IsGenericType && type.BaseType.GetGenericTypeDefinition().FullName == typeof(LinqRepositoryBase<,,,>).FullName ||
                                type.IsGenericType && type.BaseType.GetGenericTypeDefinition().FullName == typeof(LinqRepositoryBase<,>).FullName ||
                                type.IsGenericType && type.BaseType.GetGenericTypeDefinition().FullName == typeof(LinqRepositoryBase<>).FullName)
                            {
                                linqBaseRepositoryTypes.Add(type.Name);
                            }
                        }
                        AppDomain.CurrentDomain.ReflectionOnlyAssemblyResolve -= ReflectionOnlyAssemblyResolve;
                    }
                }
            }
            return linqBaseRepositoryTypes;
        }

        private Assembly ReflectionOnlyAssemblyResolve(object sender, ResolveEventArgs args)
        {
            AssemblyName name = new AssemblyName(args.Name);
            var path = Path.Combine(_tempBin.FullName, name.Name + ".dll");
            if (File.Exists(path))
            {
                return Assembly.ReflectionOnlyLoadFrom(path);
            }
            return Assembly.ReflectionOnlyLoad(args.Name);
        }
    }

    public class LinqProjectConfigurationSerializable : ProjectConfigurationSerializable
    {
        public bool UseCutomCrudBase { get; set; }
        public bool UseCutomReadBase { get; set; }
        public string SelectedLinqCrudBase { get; set; }
        public string SelectedLinqReadBase { get; set; }

    }
}
