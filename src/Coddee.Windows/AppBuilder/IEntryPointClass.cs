// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

namespace Coddee.Windows.AppBuilder
{
    /// <summary>
    /// Represents an entry point class for a console application
    /// </summary>
    public interface IEntryPointClass
    {
        /// <summary>
        /// The main method of the application.
        /// </summary>
        void Start(IContainer container);
    }
}
