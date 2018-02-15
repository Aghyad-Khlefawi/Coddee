// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using Coddee.Services;

namespace Coddee.AppBuilder
{
    /// <summary>
    /// <see cref="IApplicationBuilder"/> extensions.
    /// </summary>
    public static class BuilderExtensions
    {
        /// <summary>
        /// Configure the <see cref="ILocalizationManager"/> service.
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="defaultCluture"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseLocalization(
            this IApplicationBuilder builder,
            string defaultCluture = "en-US")
        {
            builder.BuildActionsCoordinator.AddAction(DefaultBuildActions.LocalizationBuildAction(
                                                                                                  (container) =>
                    {
                        var localizationManager = container.Resolve<ILocalizationManager>();
                        localizationManager.SetCulture(defaultCluture);
                    }));
            return builder;
        }

        /// <summary>
        /// Use the basic object mapper
        /// </summary>
        public static IApplicationBuilder UseBasicMapper(this IApplicationBuilder builder)
        {
            builder.BuildActionsCoordinator.AddAction(DefaultBuildActions.MapperBuildAction((container) =>
                  {
                      container.RegisterInstance<IObjectMapper, BasicObjectMapper>();
                  }));
            return builder;
        }

        /// <summary>
        /// Initialize the configuration manager
        /// </summary>
        /// <param name="builder">the application builder.</param>
        /// <param name="defaultFile">The default configurations file.</param>
        public static IApplicationBuilder UseConfigurationFile(
            this IApplicationBuilder builder,
           IConfigurationFile defaultFile=null)
        {
            builder.BuildActionsCoordinator.AddAction(DefaultBuildActions.ConfigFileBuildAction((container) =>
                  {
                      var config = container.Resolve<IConfigurationManager>();
                      config.Initialize(defaultFile);
                  }));
            return builder;
        }

    }

}
