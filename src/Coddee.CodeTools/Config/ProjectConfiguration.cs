// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System.IO;
using System.Xml;
using Coddee.WPF;

namespace Coddee.CodeTools
{
    public class ProjectConfiguration:BindableBase
    {

        private string _projectPath;
        public string ProjectPath
        {
            get { return _projectPath; }
            set
            {
                SetProperty(ref _projectPath, value);
                SetDefaultNameSpace();
                ProjectFolder = Path.GetDirectoryName(value);
                OnProjectPathChanged();
            }
        }

        protected virtual void OnProjectPathChanged()
        {
        }

        private string _projectFolder;
        public string ProjectFolder
        {
            get { return _projectFolder; }
            set { SetProperty(ref _projectFolder, value); }
        }

        private string _generatedCodeFolde;
        public string GeneratedCodeFolder
        {
            get { return _generatedCodeFolde; }
            set { SetProperty(ref _generatedCodeFolde, value); }
        }

        private string _defaultNamespace;
        public string DefaultNamespace
        {
            get { return _defaultNamespace; }
            set { SetProperty(ref _defaultNamespace, value); }
        }

        private void SetDefaultNameSpace()
        {
            if (string.IsNullOrWhiteSpace(DefaultNamespace) && !string.IsNullOrWhiteSpace(DefaultNamespace))
            {
                var xml = new XmlDocument();
                xml.Load(ProjectPath);
                foreach (XmlElement propertyGroup in xml.GetElementsByTagName("PropertyGroup"))
                {
                    var rootNamespace = propertyGroup.GetElementsByTagName("RootNamespace");
                    if (rootNamespace.Count > 0)
                    {
                        DefaultNamespace = $"{rootNamespace.Item(0).LastChild.Value}.{GeneratedCodeFolder}";
                        break;
                    }
                }
            }
        }
        public static explicit operator ProjectConfigurationSerializable(ProjectConfiguration config)
        {
            return new ProjectConfigurationSerializable
            {
                ProjectPath = config.ProjectPath,
                DefaultNamespace = config.DefaultNamespace,
                GeneratedCodeFolder = config.GeneratedCodeFolder
            };
        }
        public static explicit operator ProjectConfiguration(ProjectConfigurationSerializable config)
        {
            return new ModelProjectConfiguration
            {
                ProjectPath = config.ProjectPath,
                DefaultNamespace = config.DefaultNamespace,
                GeneratedCodeFolder = config.GeneratedCodeFolder
            };
        }
    }
    public class ProjectConfigurationSerializable
    {
        public string ProjectPath { get; set; }
        public string DefaultNamespace { get; set; }
        public string GeneratedCodeFolder { get; set; }
    }
}
