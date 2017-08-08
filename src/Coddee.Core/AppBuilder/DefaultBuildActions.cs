// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using Microsoft.Practices.Unity;

namespace Coddee.AppBuilder
{
    public class DefaultBuildActions
    {
        public static BuildAction ToastBuildAction(Action<IUnityContainer> action)
        {
            return new BuildAction(BuildActionsKeys.Toast, action);
        }
        public static BuildAction LoggerBuildAction(Action<IUnityContainer> action)
        {
            return new BuildAction(BuildActionsKeys.Logger, action, 0);
        }
        public static BuildAction DiscoverModulesBuildAction(Action<IUnityContainer> action)
        {
            return new BuildAction(BuildActionsKeys.DiscoverModules, action, 1);
        }
        public static BuildAction ConfigureGlobalVariablesBuildAction(Action<IUnityContainer> action)
        {
            return new BuildAction(BuildActionsKeys.ConfigureGlobalVariabls, action, 2);
        }
        public static BuildAction LocalizationBuildAction(Action<IUnityContainer> action)
        {
            return new BuildAction(BuildActionsKeys.Localization, action, 3);
        }
        public static BuildAction MapperBuildAction(Action<IUnityContainer> action)
        {
            return new BuildAction(BuildActionsKeys.Mapper, action, 4);
        }
        public static BuildAction ConfigFileBuildAction(Action<IUnityContainer> action)
        {
            return new BuildAction(BuildActionsKeys.ConfigFile, action, 5);
        }
        public static BuildAction RepositoryBuildAction(Action<IUnityContainer> action)
        {
            return new BuildAction(BuildActionsKeys.Repository, action, 6);
        }
        public static BuildAction SetupViewModelBaseBuildAction(Action<IUnityContainer> action)
        {
            return new BuildAction(BuildActionsKeys.SetupViewModelBase, action, 7);
        }
        public static BuildAction LoginBuildAction(Action<IUnityContainer> action)
        {
            return new BuildAction(BuildActionsKeys.Login, action, 8);
        }
        public static BuildAction ShellBuildAction(Action<IUnityContainer> action)
        {
            return new BuildAction(BuildActionsKeys.Shell, action, 9);
        }

        public static BuildAction AppConsoleBuildAction(Action<IUnityContainer> action)
        {
            return new BuildAction(BuildActionsKeys.AppConsole, action, 10);
        }
        public static BuildAction DebugToolBuildAction(Action<IUnityContainer> action)
        {
            return new BuildAction(BuildActionsKeys.DebugTool, action, 11);
        }
        public static BuildAction DialogServiceBuildAction(Action<IUnityContainer> action)
        {
            return new BuildAction(BuildActionsKeys.DialogService, action, 12);
        }
        public static BuildAction NavigationBuildAction(Action<IUnityContainer> action)
        {
            return new BuildAction(BuildActionsKeys.Navigation, action, 13);
        }

    }
}
