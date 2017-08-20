// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using System.Collections.Generic;
using System.Linq;
using Coddee.Loggers;


namespace Coddee.AppBuilder
{
    /// <summary>
    /// Responsible of coordinating build actions and adding new ones.
    /// </summary>
    public class BuildActionsCoordinator
    {
        private const string _eventsSource = "BuildActionsCoordinator";
        private readonly ILogger _logger;

        public BuildActionsCoordinator(ILogger logger)
        {
            _logger = logger;
            _buildActions = new List<BuildAction>();
        }

        /// <summary>
        /// Collection of the added build actions.
        /// </summary>
        private readonly List<BuildAction> _buildActions;

        /// <summary>
        /// Checks if a build action was already added.
        /// </summary>
        public bool BuildActionExists(string actionName)
        {
            return _buildActions.Any(e => e.Name == actionName);
        }

        /// <summary>
        /// Returns a build action if it was already added.
        /// </summary>
        /// <returns>The action if it was added or null if not.</returns>
        public BuildAction GetAction(string actionName)
        {
            return _buildActions.FirstOrDefault(e => e.Name == actionName);
        }

        /// <summary>
        /// Executes a build action if it was added.
        /// </summary>
        /// <returns>True if the action exists and false if not</returns>
        public bool TryInvokeAction(string actionName, IContainer container)
        {
            var action = GetAction(actionName);
            if (action == null)
                return false;
            action.Invoke(container);
            return true;
        }

        /// <summary>
        /// Add a build action the collection.
        /// </summary>
        /// <param name="action">The action to add.</param>
        /// <param name="index">The order in which the action should be executed in.</param>
        public void AddAction(BuildAction action, int index)
        {
            if (_buildActions.Any(e => e.Name == action.Name))
                throw new ApplicationBuildException($"There is already a build action with the same name '{action.Name}'");
            action.InvokeOrder = index;
            _buildActions.Add(action);
            Log($"Build action Added: {action.Name}");
        }

        /// <summary>
        /// Add a build action the collection.
        /// </summary>
        /// <param name="action">The action to add.</param>
        public void AddAction(BuildAction action)
        {
            AddAction(action, action.DefaultInvokeOrder ?? _buildActions.Count);
        }

        /// <summary>
        /// Add a build action the collection and executes it after a cerine action.
        /// </summary>
        /// <param name="actionToAdd">The action to add.</param>
        /// <param name="targetActionName">The action that this action should be executed after.</param>
        public void AddActionAfter(BuildAction actionToAdd, string targetActionName)
        {
            var action = _buildActions.FirstOrDefault(e => e.Name == targetActionName);
            if (action == null)
                throw new ApplicationBuildException($"Build action '{targetActionName}' was not found.");

            _buildActions.OrderBy(e => e.InvokeOrder)
                .Where(e => e.InvokeOrder > action.InvokeOrder)
                .ForEach(e => e.InvokeOrder++);

            AddAction(actionToAdd, action.InvokeOrder + 1);
        }

        /// <summary>
        /// Add a build action the collection and executes it before a cerine action.
        /// </summary>
        /// <param name="actionToAdd">The action to add.</param>
        /// <param name="targetActionName">The action that this action should be executed before.</param>
        public void AddActionBefor(BuildAction actionToAdd, string targetActionName)
        {
            var action = _buildActions.FirstOrDefault(e => e.Name == targetActionName);
            if (action == null)
                throw new ApplicationBuildException($"Build action '{targetActionName}' was not found.");

            _buildActions.OrderBy(e => e.InvokeOrder)
                .Where(e => e.InvokeOrder >= action.InvokeOrder)
                .ForEach(e => e.InvokeOrder++);

            AddAction(actionToAdd, action.InvokeOrder);
        }

        /// <summary>
        /// Execute all the added build actions.
        /// </summary>
        /// <param name="container">The depenedncy container.</param>
        public void InvokeAll(IContainer container)
        {
            foreach (var buildAction in _buildActions.OrderBy(e => e.InvokeOrder))
            {
                try
                {
                    Log($"Invoking build action : {buildAction.Name}");
                    buildAction.Invoke(container);
                    Log($"Build action completed: {buildAction.Name} ");
                }
                catch (Exception ex)
                {
                    var buildEx = new ApplicationBuildException($"Error while invoking build action '{buildAction.Name}'",ex);
                    _logger.Log(_eventsSource, ex);
                    throw buildEx;
                }
            }
        }

        private void Log(string log, LogRecordTypes type = LogRecordTypes.Debug)
        {
            _logger.Log(_eventsSource, log, type);
        }
    }
}
