// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;


namespace Coddee.AppBuilder
{
    /// <summary>
    /// Static class that contains the default application build actions
    /// </summary>
    public static class DefaultBuildActions
    {
        /// <summary>
        /// Creates a build action for the Toast service
        /// </summary>
        public static BuildAction ToastBuildAction(Action<IContainer> action)
        {
            return new BuildAction(BuildActionsKeys.Toast, action);
        }

        /// <summary>
        /// Creates a build action for the Logger service
        /// </summary>
        public static BuildAction LoggerBuildAction(Action<IContainer> action)
        {
            return new BuildAction(BuildActionsKeys.Logger, action, 0);
        }

        /// <summary>
        /// Creates a build action for registering the default framework modules.
        /// </summary>
        public static BuildAction RegisterDefaultModulesBuildAction(Action<IContainer> action)
        {
            return new BuildAction(BuildActionsKeys.RegisterDefaultModules, action, 1);
        }

        /// <summary>
        /// Creates a build action for discovering additional modules.
        /// </summary>
        public static BuildAction DiscoverModulesBuildAction(Action<IContainer> action)
        {
            return new BuildAction(BuildActionsKeys.DiscoverModules, action, 2);
        }

        /// <summary>
        /// Creates a build action for the Global variables service
        /// </summary>
        public static BuildAction ConfigureGlobalVariablesBuildAction(Action<IContainer> action)
        {
            return new BuildAction(BuildActionsKeys.ConfigureGlobalVariabls, action, 3);
        }

        /// <summary>
        /// Creates a build action for the Localization service
        /// </summary>
        public static BuildAction LocalizationBuildAction(Action<IContainer> action)
        {
            return new BuildAction(BuildActionsKeys.Localization, action, 4);
        }

        /// <summary>
        /// Creates a build action for the Object mapper service
        /// </summary>
        public static BuildAction MapperBuildAction(Action<IContainer> action)
        {
            return new BuildAction(BuildActionsKeys.Mapper, action, 5);
        }

        /// <summary>
        /// Creates a build action for the Configuration service
        /// </summary>
        public static BuildAction ConfigFileBuildAction(Action<IContainer> action)
        {
            return new BuildAction(BuildActionsKeys.ConfigFile, action, 6);
        }

        /// <summary>
        /// Creates a build action for the repository manager
        /// </summary>
        public static BuildAction RepositoryManagerBuildAction(Action<IContainer> action)
        {
            return new BuildAction(BuildActionsKeys.RepositoryManager, action, 6);
        }

        /// <summary>
        /// Creates a build action for the Linq repositories
        /// </summary>
        public static BuildAction LinqRepositoryBuildAction(Action<IContainer> action)
        {
            return new BuildAction(BuildActionsKeys.LinqRepository, action, 7);
        }

        /// <summary>
        /// Creates a build action for the rest repositories
        /// </summary>
        public static BuildAction RESTRepositoryBuildAction(Action<IContainer> action)
        {
            return new BuildAction(BuildActionsKeys.RESTRepository, action, 7);
        }

        /// <summary>
        /// Creates a build action for the MongoDB
        /// </summary>
        public static BuildAction MongoRepositoryBuildAction(Action<IContainer> action)
        {
            return new BuildAction(BuildActionsKeys.MongoRepository, action, 7);
        }

        /// <summary>
        /// Creates a build action for the InMemory
        /// </summary>
        public static BuildAction InMemoryRepositoryBuildAction(Action<IContainer> action)
        {
            return new BuildAction(BuildActionsKeys.InMemoryRepository, action, 7);
        }

        /// <summary>
        /// Creates a build action for the console main method.
        /// </summary>
        public static BuildAction ConsoleMainBuildAction(Action<IContainer> action)
        {
            return new BuildAction(BuildActionsKeys.ConsoleMain, action, 8);
        }

        /// <summary>
        /// Creates a build action for the application theme.
        /// </summary>
        public static BuildAction ApplicationThemeAction(Action<IContainer> action)
        {
            return new BuildAction(BuildActionsKeys.ApplicationTheme, action, 9);
        }

        /// <summary>
        /// Creates a build action for setting up the ViewModel base class.
        /// </summary>
        public static BuildAction SetupViewModelBaseBuildAction(Action<IContainer> action)
        {
            return new BuildAction(BuildActionsKeys.SetupViewModelBase, action, 9.1f);
        }

        /// <summary>
        /// Creates a build action for setting up the ViewModel base class.
        /// </summary>
        public static BuildAction SetupWpfViewModelBaseBuildAction(Action<IContainer> action)
        {
            return new BuildAction(BuildActionsKeys.SetupWpfViewModelBase, action, 9.2f);
        }

        /// <summary>
        /// Creates a build action for the application login.
        /// </summary>
        public static BuildAction LoginBuildAction(Action<IContainer> action)
        {
            return new BuildAction(BuildActionsKeys.Login, action, 10);
        }

        /// <summary>
        /// Creates a build action for the application shell.
        /// </summary>
        public static BuildAction ShellBuildAction(Action<IContainer> action)
        {
            return new BuildAction(BuildActionsKeys.Shell, action, 11);
        }


        /// <summary>
        /// Creates a build action for the application console.
        /// </summary>
        public static BuildAction AppConsoleBuildAction(Action<IContainer> action)
        {
            return new BuildAction(BuildActionsKeys.AppConsole, action, 12);
        }

        /// <summary>
        /// Creates a build action for the application debug tools.
        /// </summary>
        public static BuildAction DebugToolBuildAction(Action<IContainer> action)
        {
            return new BuildAction(BuildActionsKeys.DebugTool, action, 12);
        }

        /// <summary>
        /// Creates a build action for the dialog service
        /// </summary>
        public static BuildAction DialogServiceBuildAction(Action<IContainer> action)
        {
            return new BuildAction(BuildActionsKeys.DialogService, action, 13);
        }

        /// <summary>
        /// Creates a build action for the navigation service
        /// </summary>
        public static BuildAction NavigationBuildAction(Action<IContainer> action)
        {
            return new BuildAction(BuildActionsKeys.Navigation, action, 14);
        }

    }
}
