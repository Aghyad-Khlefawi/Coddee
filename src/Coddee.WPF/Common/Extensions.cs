// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Coddee.Collections;
using Coddee.Mvvm;
using Coddee.Services;
using Coddee.Services.Dialogs;
using Coddee.Validation;
using Coddee.WPF.Commands;
using Coddee.WPF.Services.Dialogs;

namespace Coddee.WPF
{
    public static class Extensions
    {
        public static void SetFocus(this IInputElement element)
        {
            element.Focus();
            Keyboard.Focus(element);
        }

        public static void FocusOnLoad(this FrameworkElement parent, IInputElement element)
        {
            parent.Loaded += delegate
            {
                element.SetFocus();
            };
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

        public static IReactiveCommand<TObserved> ObserveProperty<TObserved>(this IReactiveCommand<TObserved> command, Expression<Func<TObserved, object>> property)
        {
            var propertyName = ExpressionHelper.GetMemberName(property);
            var type = ExpressionHelper.GetMemberType(property);
            command.ObserveProperty(propertyName, Validators.GetValidator(type));
            return command;
        }
        public static IReactiveCommand<TObserved> ObserveProperty<TObserved,TProperty>(this IReactiveCommand<TObserved> command, Expression<Func<TObserved, TProperty>> propertyName, Validator<TProperty> validator)
        {
            return command.ObserveProperty(ExpressionHelper.GetMemberName(propertyName), validator);
        }
        public static IReactiveCommand ObserveProperty<TObserved>(this IReactiveCommand<TObserved> command, Expression<Func<TObserved, object>> propertyName, Validator validator)
        {
            return command.ObserveProperty(ExpressionHelper.GetMemberName(propertyName), validator);
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

        public static IDialog ShowDialog(this IDialogService dialogs, string title, UIElement content, DialogOptions options, params ActionCommandBase[] actions)
        {
            var dialog = dialogs.CreateDialog(title, content, options, actions);
            dialog.Show();
            return dialog;
        }

        public static IDialog ShowDialog(this IDialogService dialogs, UIElement content, DialogOptions options, params ActionCommandBase[] actions)
        {
            var dialog = dialogs.CreateDialog(content, options, actions);
            dialog.Show();
            return dialog;
        }

        public static IDialog ShowDialog(this IDialogService dialogs, string title, UIElement content, params ActionCommandBase[] actions)
        {
            var dialog = dialogs.CreateDialog(title, content, actions);
            dialog.Show();
            return dialog;
        }

        public static IDialog ShowDialog(this IDialogService dialogs, UIElement content, params ActionCommandBase[] actions)
        {
            var dialog = dialogs.CreateDialog(content, actions);
            dialog.Show();
            return dialog;
        }

        public static IDialog ShowDialog(this IDialogService dialogs, string title, IEditorViewModel editor, DialogOptions options)
        {
            var dialog = dialogs.CreateDialog(title, editor, options);
            dialog.Show();
            return dialog;
        }

        public static IDialog ShowDialog(this IDialogService dialogs, IEditorViewModel editor, DialogOptions options)
        {
            var dialog = dialogs.CreateDialog(editor, options);
            dialog.Show();
            return dialog;
        }

        public static IDialog ShowDialog(this IDialogService dialogs, string title, IEditorViewModel editor)
        {
            var dialog = dialogs.CreateDialog(title, editor);
            dialog.Show();
            return dialog;
        }

        public static IDialog ShowDialog(this IDialogService dialogs, IEditorViewModel editor)
        {
            var dialog = dialogs.CreateDialog(editor);
            dialog.Show();
            return dialog;
        }

        public static IDialog ShowConfirmation(this IDialogService dialogs, string message, Action yesAction, Action noAction = null)
        {
            var dialog = dialogs.CreateConfirmation(message, yesAction, noAction);
            dialog.Show();
            return dialog;
        }

        public static IDialog ShowDialog(this IDialogService dialogs, IPresentable presentable)
        {
            var dialog = dialogs.CreateDialog(presentable);
            dialog.Show();
            return dialog;
        }

        public static IDialog ShowDialog(this IDialogService dialogs, IPresentable presentable, DialogOptions options)
        {
            var dialog = dialogs.CreateDialog(presentable, options);
            dialog.Show();
            return dialog;
        }

        public static IDialog ShowDialog(this IDialogService dialogs, IPresentable presentable, DialogOptions options, params ActionCommandBase[] actions)
        {
            var dialog = dialogs.CreateDialog(presentable, options, actions);
            dialog.Show();
            return dialog;
        }

        public static IDialog ShowDialog(this IDialogService dialogs,string title, IPresentable presentable)
        {
            var dialog = dialogs.CreateDialog(title,presentable);
            dialog.Show();
            return dialog;
        }

        public static IDialog ShowDialog(this IDialogService dialogs, string title, IPresentable presentable, DialogOptions options)
        {
            var dialog = dialogs.CreateDialog(title, presentable, options);
            dialog.Show();
            return dialog;
        }

        public static IDialog ShowDialog(this IDialogService dialogs, string title, IPresentable presentable, DialogOptions options, params ActionCommandBase[] actions)
        {
            var dialog = dialogs.CreateDialog(title, presentable, options, actions);
            dialog.Show();
            return dialog;
        }
    }
}