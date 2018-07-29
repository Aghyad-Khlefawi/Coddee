// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using Coddee.AppBuilder;

namespace Coddee.Xamarin.Forms.AppBuilder
{
    public interface IXamarinApplicationBuilder:IApplicationBuilder
    {
    }

    public class XamarinApplicationBuilder : ApplicationBuilder, IXamarinApplicationBuilder
    {
        public XamarinApplicationBuilder(IApplication app, IContainer container) : base(app, container)
        {

        }
    }
}
