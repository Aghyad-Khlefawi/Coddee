// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using Coddee.AppBuilder;
using Coddee.Xamarin.Forms.AppBuilder;
using Xamarin.Forms;

namespace Coddee.Xamarin.Forms
{
    public class XamarinFormsApplication : Application<IXamarinApplicationBuilder>
    {
        /// <inheritdoc />
        public XamarinFormsApplication(Guid applicationID, string applicationName, IContainer container)
        : base(applicationID, applicationName, ApplicationTypes.XamarinForms, container)
        {
        }

        protected override IXamarinApplicationBuilder ResolveBuilder()
        {
            return _container.Resolve<XamarinApplicationBuilder>();
        }

        /// <summary>
        /// The instance of the currently running xamarin application.
        /// </summary>
        protected Application XamarinApp { get; private set; }

        public override void Run(Action<IXamarinApplicationBuilder> BuildApplication)
        {
            XamarinApp = Application.Current;
            base.Run(BuildApplication);
        }
    }
}
