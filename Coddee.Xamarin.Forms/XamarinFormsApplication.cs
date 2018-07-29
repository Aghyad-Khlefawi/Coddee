// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using Coddee.AppBuilder;
using Coddee.Xamarin.Forms.AppBuilder;

namespace Coddee.Xamarin.Forms
{
    public class XamarinFormsApplication : Application<IXamarinApplicationBuilder>
    {
        /// <inheritdoc />
        public XamarinFormsApplication(Guid applicationID, string applicationName,IContainer container)
        :base(applicationID,applicationName,ApplicationTypes.XamarinForms,container)
        {
        }

        protected override IXamarinApplicationBuilder ResolveBuilder()
        {
            return _container.Resolve<XamarinApplicationBuilder>();
        }
    }
}
