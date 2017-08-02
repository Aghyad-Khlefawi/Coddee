// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

namespace Coddee.Services
{
    /// <summary>
    /// service for storing application configurations in one or multiple files
    /// </summary>
    public interface IConfigurationManager
    {
        void Initialize(IConfigurationFile defaultConfigurationFile);

        void AddConfigurationFile(IConfigurationFile configFile);

        TValue GetValue<TValue>(string key, string fileName = null);
        bool TryGetValue<TValue>(string key, out TValue value, string fileName = null);
        void SetValue<TValue>(string key, TValue value, string fileName = null);
    }
}