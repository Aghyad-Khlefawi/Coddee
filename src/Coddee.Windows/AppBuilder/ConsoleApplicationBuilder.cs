// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

namespace Coddee.AppBuilder
{
    /// <summary>
    /// Sets up the <see cref="ConsoleApplication"/> build steps.
    /// </summary>
    public class ConsoleApplicationBuilder : WindowsApplicationBuilder,IConsoleApplicationBuilder
    {
        /// <inheritdoc />
        public ConsoleApplicationBuilder(IApplication app, IContainer container) : base(app, container)
        {

        }

    }
}
