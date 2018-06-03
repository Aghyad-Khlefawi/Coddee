// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Coddee.Attributes;
using Microsoft.Extensions.DependencyInjection;

namespace Coddee.AspNet
{
    public class CoddeeControllersManager
    {
        private readonly IServiceCollection _container;

        public CoddeeControllersManager(IServiceCollection container)
        {
            _container = container;
            _apiActions = new ConcurrentDictionary<string, IApiAction>();
            _controllerTypes = new List<Type>();
        }

        private readonly List<Type> _controllerTypes;
        private readonly ConcurrentDictionary<string, IApiAction> _apiActions;

        public ConcurrentDictionary<string, IApiAction> GetRegisteredActions()
        {
            foreach (var type in _controllerTypes)
            {
                var actionsInfo = type.GetMethods().Where(e => Attribute.IsDefined((MemberInfo)e, typeof(ApiActionAttribute)));
                var controller = _container.BuildServiceProvider().GetService(type);
                foreach (var memberInfo in actionsInfo)
                {
                    var paths = memberInfo.GetCustomAttributes<ApiActionAttribute>().Select(e => e.Path).ToList();
                    foreach (var path in paths)
                    {
                        var pathLower = path.ToLower();
                        var delegateAction = DelegateAction.CreateDelegateAction(null, memberInfo, pathLower);
                        delegateAction.SetOwner(controller);
                        if (delegateAction == null)
                            break;

                        var authAttr = memberInfo.GetCustomAttribute<AuthorizeAttribute>() ?? type.GetCustomAttribute<AuthorizeAttribute>();

                        if (authAttr != null)
                        {
                            delegateAction.RequiredAuthentication = true;
                            delegateAction.Claim = authAttr.Claim;
                        }
                        if (!_apiActions.ContainsKey(pathLower))
                            _apiActions.TryAdd(pathLower, delegateAction);
                    }
                }
            }
            return _apiActions;
        }

        public void RegisterController<T>() where T : class
        {
            RegisterController(typeof(T));
        }
        public void RegisterController(Type type)
        {
            _controllerTypes.Add(type);
            _container.AddTransient(type);
        }
    }
}