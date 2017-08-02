// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System.Collections.Generic;
using System.Linq;
using Coddee.Loggers;
using Microsoft.Practices.Unity;

namespace Coddee.AppBuilder
{
    public class BuildActionsCoordinator
    {
        private const string _eventsSource = "BuildActionsCoordinator";
        private readonly ILogger _logger;

        public BuildActionsCoordinator(ILogger logger)
        {
            _logger = logger;
            _buildActions = new List<BuildAction>();
        }


        private readonly List<BuildAction> _buildActions;


        public bool BuildActionExists(string actionName)
        {
            return _buildActions.Any(e => e.Name == actionName);
        }
        public BuildAction GetAction(string actionName)
        {
            return _buildActions.FirstOrDefault(e => e.Name == actionName);
        }

        public bool TryInvokeAction(string actionName, IUnityContainer container)
        {
            var action = GetAction(actionName);
            if (action == null)
                return false;
            action.Invoke(container);
            return true;
        }

        public void AddAction(BuildAction action, int index)
        {
            if (_buildActions.Any(e => e.Name == action.Name))
                throw new ApplicationBuildException($"There is already a build action with the same name '{action.Name}'");
            action.InvokeOrder = index;
            _buildActions.Add(action);
            Log($"Build action Added: {action.Name}");
        }
        public void AddAction(BuildAction action)
        {
            AddAction(action, action.DefaultInvokeOrder ?? _buildActions.Count);
        }
        public void AddActionAfter(BuildAction actionToAdd, string targetActionName)
        {
            var action = _buildActions.FirstOrDefault(e => e.Name == targetActionName);
            if (action == null)
                throw new ApplicationBuildException($"Build action '{targetActionName}' was not found.");

            AddAction(actionToAdd, _buildActions.IndexOf(action) + 1);
        }

        public void AddActionBefor(BuildAction actionToAdd, string targetActionName)
        {
            var action = _buildActions.FirstOrDefault(e => e.Name == targetActionName);
            if (action == null)
                throw new ApplicationBuildException($"Build action '{targetActionName}' was not found.");

            AddAction(actionToAdd, _buildActions.IndexOf(action));
        }

        public void InvokeAll(IUnityContainer container)
        {
            foreach (var buildAction in _buildActions.OrderBy(e => e.InvokeOrder))
            {
                Log($"Invoking build action : {buildAction.Name}");
                buildAction.Invoke(container);
                Log($"Invoking build action : {buildAction.Name} completed");
            }
        }

        private void Log(string log, LogRecordTypes type = LogRecordTypes.Debug)
        {
            _logger.Log(_eventsSource, log, type);
        }
    }
}
