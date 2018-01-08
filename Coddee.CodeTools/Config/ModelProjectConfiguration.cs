// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Coddee.Collections;
using Coddee.WPF.Commands;

namespace Coddee.CodeTools
{
    public class ModelProjectConfiguration : ProjectConfiguration
    {
        public ModelProjectConfiguration()
        {
            AdditionalProperties = AsyncObservableCollection<ModelAdditionalProperty>.Create();
            AddCommand = new RelayCommand(Add);
            RemoveCommand = new RelayCommand<ModelAdditionalProperty>(Remove);
        }


        public ICommand AddCommand { get; set; }
        public ICommand RemoveCommand { get; set; }

        private string _additionalInterfaces;
        public string AdditionalInterfaces
        {
            get { return _additionalInterfaces; }
            set { SetProperty(ref _additionalInterfaces, value); }
        }

        private AsyncObservableCollection<ModelAdditionalProperty> _additionalProperties;
        public AsyncObservableCollection<ModelAdditionalProperty> AdditionalProperties
        {
            get { return _additionalProperties; }
            set { SetProperty(ref _additionalProperties, value); }
        }

        private string _propertyType;
        public string PropertyType
        {
            get { return _propertyType; }
            set { SetProperty(ref _propertyType, value); }
        }

        private string _propertyName;
        public string PropertyName
        {
            get { return _propertyName; }
            set { SetProperty(ref _propertyName, value); }
        }

        void Remove(ModelAdditionalProperty item)
        {
            AdditionalProperties.Remove(item);
        }

        private void Add()
        {
            if (!string.IsNullOrWhiteSpace(PropertyType) && !string.IsNullOrWhiteSpace(PropertyName) && AdditionalProperties.All(e => e.Name != PropertyName))
            {
                AdditionalProperties.Add(new ModelAdditionalProperty
                {
                    Type = PropertyType,
                    Name = PropertyName
                });
                PropertyName = string.Empty;
            }
        }


        public static explicit operator ModelProjectConfigurationSerializable(ModelProjectConfiguration config)
        {
            return new ModelProjectConfigurationSerializable
            {
                ProjectPath = config.ProjectPath,
                DefaultNamespace = config.DefaultNamespace,
                GeneratedCodeFolder = config.GeneratedCodeFolder,
                AdditionalInterfaces = config.AdditionalInterfaces,
                AdditionalProperties = config.AdditionalProperties?.ToList() ?? new List<ModelAdditionalProperty>()
            };
        }
        public static explicit operator ModelProjectConfiguration(ModelProjectConfigurationSerializable config)
        {
            return new ModelProjectConfiguration
            {
                ProjectPath = config.ProjectPath,
                DefaultNamespace = config.DefaultNamespace,
                GeneratedCodeFolder = config.GeneratedCodeFolder,
                AdditionalInterfaces = config.AdditionalInterfaces,
                AdditionalProperties = config.AdditionalProperties?.ToAsyncObservableCollection() ?? new AsyncObservableCollection<ModelAdditionalProperty>()
            };
        }
    }

    public class ModelAdditionalProperty
    {
        public string Type { get; set; }
        public string Name { get; set; }
    }

    public class ModelProjectConfigurationSerializable: ProjectConfigurationSerializable
    {
        public string AdditionalInterfaces { get; set; }
        public List<ModelAdditionalProperty> AdditionalProperties { get; set; }
    }
}