// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;


namespace Coddee.AppBuilder
{
    public class DefaultBuildActions
    {
        public static BuildAction ToastBuildAction(Action<IContainer> action)
        {
            return new BuildAction(BuildActionsKeys.Toast, action);
        }
        public static BuildAction LoggerBuildAction(Action<IContainer> action)
        {
            return new BuildAction(BuildActionsKeys.Logger, action, 0);
        }
        public static BuildAction RegisterDefaultModulesBuildAction(Action<IContainer> action)
        {
            return new BuildAction(BuildActionsKeys.RegisterDefaultModules, action, 1);
        }
        public static BuildAction DiscoverModulesBuildAction(Action<IContainer> action)
        {
            return new BuildAction(BuildActionsKeys.DiscoverModules, action, 2);
        }
        public static BuildAction ConfigureGlobalVariablesBuildAction(Action<IContainer> action)
        {
            return new BuildAction(BuildActionsKeys.ConfigureGlobalVariabls, action, 3);
        }
        public static BuildAction LocalizationBuildAction(Action<IContainer> action)
        {
            return new BuildAction(BuildActionsKeys.Localization, action, 4);
        }
        public static BuildAction MapperBuildAction(Action<IContainer> action)
        {
            return new BuildAction(BuildActionsKeys.Mapper, action, 5);
        }
        public static BuildAction ConfigFileBuildAction(Action<IContainer> action)
        {
            return new BuildAction(BuildActionsKeys.ConfigFile, action, 6);
        }
        public static BuildAction LinqRepositoryBuildAction(Action<IContainer> action)
        {
            return new BuildAction(BuildActionsKeys.LinqRepository, action, 7);
        }
        public static BuildAction RESTRepositoryBuildAction(Action<IContainer> action)
        {
            return new BuildAction(BuildActionsKeys.RESTRepository, action,7);
        }
        public static BuildAction MongoRepositoryBuildAction(Action<IContainer> action)
        {
            return new BuildAction(BuildActionsKeys.MongoRepository, action, 7);
        }
        public static BuildAction InMemoryRepositoryBuildAction(Action<IContainer> action)
        {
            return new BuildAction(BuildActionsKeys.InMemoryRepository, action, 7);
        }
        public static BuildAction ConsoleMainBuildAction(Action<IContainer> action)
        {
            return new BuildAction(BuildActionsKeys.ConsoleMain, action, 8);
        }
        public static BuildAction SetupViewModelBaseBuildAction(Action<IContainer> action)
        {
            return new BuildAction(BuildActionsKeys.SetupViewModelBase, action, 9);
        }
        public static BuildAction LoginBuildAction(Action<IContainer> action)
        {
            return new BuildAction(BuildActionsKeys.Login, action, 10);
        }
        public static BuildAction ShellBuildAction(Action<IContainer> action)
        {
            return new BuildAction(BuildActionsKeys.Shell, action, 11);
        }
        public static BuildAction AppConsoleBuildAction(Action<IContainer> action)
        {
            return new BuildAction(BuildActionsKeys.AppConsole, action, 12);
        }
        public static BuildAction DebugToolBuildAction(Action<IContainer> action)
        {
            return new BuildAction(BuildActionsKeys.DebugTool, action, 12);
        }
        public static BuildAction DialogServiceBuildAction(Action<IContainer> action)
        {
            return new BuildAction(BuildActionsKeys.DialogService, action, 13);
        }
        public static BuildAction NavigationBuildAction(Action<IContainer> action)
        {
            return new BuildAction(BuildActionsKeys.Navigation, action, 14);
        }

    }
}
