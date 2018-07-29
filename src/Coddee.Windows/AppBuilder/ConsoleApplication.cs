// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;

namespace Coddee.AppBuilder
{
    /// <summary>
    /// A windows console application.
    /// </summary>
    public class ConsoleApplication : Application<IConsoleApplicationBuilder>
    {
        /// <inheritdoc />
        public ConsoleApplication(Guid applicationID, string applicationName, IContainer container)
            : base(applicationID, applicationName, ApplicationTypes.Console, container)
        {
        }

        /// <inheritdoc />
        public ConsoleApplication(string applicationName, IContainer container)
            : this(Guid.NewGuid(), applicationName, container)
        {

        }

        protected override IConsoleApplicationBuilder ResolveBuilder()
        {
            return _container.Resolve<ConsoleApplicationBuilder>();
        }
    }
}
