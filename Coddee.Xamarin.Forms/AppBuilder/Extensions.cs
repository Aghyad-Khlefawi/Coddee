// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using Coddee.Mvvm;
using Coddee.Xamarin.Forms.AppBuilder;
using Xamarin.Forms;

namespace Coddee.AppBuilder
{
    public static class BuilderExtensions
    {
        /// <summary>
        /// Sets the application MainPage
        /// </summary>
        public static IXamarinApplicationBuilder UseMainPage<TMain>(
            this IXamarinApplicationBuilder builder)
        where TMain : IPresentableViewModel
        {
            builder.BuildActionsCoordinator.AddAction(DefaultBuildActions.MainPageAction((container) =>
            {
                var app = Application.Current;
                var main = container.Resolve<TMain>();
                app.MainPage = (Page)main.GetView();
                main.Initialize();
            }));
            return builder;
        }

        /// <summary>
        /// Sets the application MainNavigationPage
        /// </summary>
        public static IXamarinApplicationBuilder UseNavigationMainPage<TMain>(
            this IXamarinApplicationBuilder builder)
            where TMain : IPresentableViewModel
        {
            builder.BuildActionsCoordinator.AddAction(DefaultBuildActions.MainPageAction((container) =>
            {
                var app = Application.Current;
                var main = container.Resolve<TMain>();
                app.MainPage = new NavigationPage((Page)main.GetView());
                main.Initialize();
            }));
            return builder;
        }

    }
}
