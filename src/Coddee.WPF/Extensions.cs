// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Coddee.Validation;
using Coddee.WPF.Commands;

namespace Coddee.WPF
{
    public static class Extensions
    {
        public static Task InitializeAll(this IEnumerable<IViewModel> items)
        {
            return Task.WhenAll(items.Select(e => e.Initialize()));
        }
        public static ReactiveCommandBase<T> ObserveRequiredFields<T>(this ReactiveCommandBase<T> command)
        {
            if (command.ObservedObject is IViewModel vm)
            {
                foreach (var requiredField in vm.RequiredFields)
                {
                    command.ObserveProperty(requiredField.FieldName, requiredField.ValidateField);
                }
            }
            return command;
        }
    }
}