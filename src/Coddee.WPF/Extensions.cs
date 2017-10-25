﻿// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using Coddee.Collections;
using Coddee.Validation;
using Coddee.WPF.Commands;

namespace Coddee.WPF
{
    public static class Extensions
    {
        public static Task InitializeAll(this IEnumerable<IViewModel> items, bool forceInitialization = false)
        {
            return Task.WhenAll(items.Where(e => forceInitialization || !e.IsInitialized).Select(e => e.Initialize()));
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

        public static IEnumerable<SelectableItem<T>> AsSelectable<T>(this IEnumerable<T> collection)
        {
            return collection.Select(e => new SelectableItem<T>(e));
        }

        public static async Task<IEnumerable<SelectableItem<T>>> AsSelectable<T>(this Task<IEnumerable<T>> collection)
        {
            return (await collection).AsSelectable();
        }
        public static IEnumerable<SelectableItem<T>> AsSelectable<T>(this IEnumerable<T> collection, EventHandler<T> onItemSelected, EventHandler<T> onItemUnselected)
        {
            return collection.Select(e =>
            {
                var res = new SelectableItem<T>(e);
                if (onItemSelected != null)
                    res.Selected += onItemSelected;
                if (onItemUnselected != null)
                    res.UnSelected += onItemUnselected;
                return res;
            });
        }

        public static async Task<IEnumerable<SelectableItem<T>>> AsSelectable<T>(this Task<IEnumerable<T>> collection, EventHandler<T> onItemSelected, EventHandler<T> onItemUnselected)
        {
            return (await collection).AsSelectable(onItemSelected, onItemUnselected);
        }
        public static IEnumerable<T> GetSelected<T>(this IEnumerable<SelectableItem<T>> collection)
        {
            return collection.Where(e => e.IsSelected).Select(e => e.Item);
        }

        public static void UnSelectAll<T>(this IEnumerable<SelectableItem<T>> collection)
        {
            collection.ForEach(e => e.IsSelected = false);
        }

        public static List<TKey> GetSelectedKeys<TKey, T>(this IEnumerable<SelectableItem<T>> collection) where T : IUniqueObject<TKey>
        {
            return collection.Where(e => e.IsSelected).Select(e => e.Item.GetKey).ToList();
        }

        public static ReactiveCommand<TObserved> ObserveProperty<TObserved>(this ReactiveCommand<TObserved> command, Expression<Func<TObserved, object>> property)
        {
            var propertyName = ExpressionHelper.GetMemberName(property);
            var type = ExpressionHelper.GetMemberType(property);
            command.ObserveProperty(propertyName, RequiredFieldValidators.GetValidator(type));
            return command;
        }

        public static async Task EnsureInitialization(this IViewModel vm)
        {
            if (!vm.IsInitialized)
                await vm.Initialize();
        }

        public static AsyncObservableCollectionView<T> ToAsyncObservableCollectionView<T>(this IEnumerable<T> collection, FilterHandler<T> filterPredicate)
        {
            return AsyncObservableCollectionView<T>.Create(filterPredicate, collection);
        }

        public static async Task<AsyncObservableCollectionView<T>> ToAsyncObservableCollectionView<T>(this Task<IEnumerable<T>> collection, FilterHandler<T> filterPredicate)
        {
            return AsyncObservableCollectionView<T>.Create(filterPredicate, await collection);
        }
    }
}