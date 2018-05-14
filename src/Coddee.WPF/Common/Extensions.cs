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
using Coddee.Data;
using Coddee.Mvvm;
using Coddee.Services;
using Coddee.Services.Dialogs;
using Coddee.Validation;
using Coddee.WPF.Commands;
using Coddee.WPF.Services.Dialogs;

namespace Coddee.WPF
{
    /// <summary>
    /// Extension methods for WPF classes
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Sets focus on a visual element.
        /// </summary>
        /// <param name="element"></param>
        public static void SetFocus(this IInputElement element)
        {
            element.Focus();
            Keyboard.Focus(element);
        }

        /// <summary>
        /// Sets focus on a child element once the parent is loaded.
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="element"></param>
        public static void FocusOnLoad(this FrameworkElement parent, IInputElement element)
        {
            parent.Loaded += delegate
            {
                element.SetFocus();
            };
        }

        /// <summary>
        /// Project collection items as selectable objects.
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<SelectableItem<T>> AsSelectable<T>(this IEnumerable<T> collection)
        {
            return collection.Select(e => new SelectableItem<T>(e));
        }

        /// <summary>
        /// Project collection items as selectable objects.
        /// </summary>
        public static async Task<IEnumerable<SelectableItem<T>>> AsSelectable<T>(this Task<IEnumerable<T>> collection)
        {
            return (await collection).AsSelectable();
        }


        /// <summary>
        /// Creates an <see cref="AsyncObservableCollectionView{T}"/> and binds the repository changes to the collection
        /// </summary>
        public static async Task<AsyncObservableCollectionView<T>> ToAsyncObservableCollectionView<T, TKey>(this ICRUDRepository<T, TKey> repo,FilterHandler<T> filter)
            where T : IUniqueObject<TKey>
        {
            var items = await repo.GetItems();
            var collection = AsyncObservableCollectionView<T>.Create(filter, items);
            collection.BindToRepositoryChanges(repo);
            return collection;
        }

        /// <summary>
        /// Project collection items as selectable objects.
        /// </summary>
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

        /// <summary>
        /// Project collection items as selectable objects.
        /// </summary>
        public static async Task<IEnumerable<SelectableItem<T>>> AsSelectable<T>(this Task<IEnumerable<T>> collection, EventHandler<T> onItemSelected, EventHandler<T> onItemUnselected)
        {
            return (await collection).AsSelectable(onItemSelected, onItemUnselected);
        }

        /// <summary>
        /// Returns the selected items in a SelectableItems collection.
        /// </summary>
        public static IEnumerable<T> GetSelected<T>(this IEnumerable<SelectableItem<T>> collection)
        {
            return collection.Where(e => e.IsSelected).Select(e => e.Item);
        }

        /// <summary>
        /// Sets all the selectable items in a collection to not selected.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        public static void UnSelectAll<T>(this IEnumerable<SelectableItem<T>> collection)
        {
            collection.ForEach(e => e.IsSelected = false);
        }

        /// <summary>
        /// Returns is the keys of all selected objects.
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        /// <returns></returns>
        public static List<TKey> GetSelectedKeys<TKey, T>(this IEnumerable<SelectableItem<T>> collection) where T : IUniqueObject<TKey>
        {
            return collection.Where(e => e.IsSelected).Select(e => e.Item.GetKey).ToList();
        }

        /// <inheritdoc cref="IReactiveCommand.ObserveProperty"/>
        public static IReactiveCommand<TObserved> ObserveProperty<TObserved>(this IReactiveCommand<TObserved> command, Expression<Func<TObserved, object>> property)
        {
            var propertyName = ExpressionHelper.GetMemberName(property);
            var type = ExpressionHelper.GetMemberType(property);
            command.ObserveProperty(propertyName, Validators.GetValidator(type));
            return command;
        }
        /// <inheritdoc cref="IReactiveCommand.ObserveProperty"/>
        public static IReactiveCommand<TObserved> ObserveProperty<TObserved, TProperty>(this IReactiveCommand<TObserved> command, Expression<Func<TObserved, TProperty>> propertyName, Validator<TProperty> validator)
        {
            return command.ObserveProperty(ExpressionHelper.GetMemberName(propertyName), validator);
        }
        /// <inheritdoc cref="IReactiveCommand.ObserveProperty"/>
        public static IReactiveCommand ObserveProperty<TObserved>(this IReactiveCommand<TObserved> command, Expression<Func<TObserved, object>> propertyName, Validator validator)
        {
            return command.ObserveProperty(ExpressionHelper.GetMemberName(propertyName), validator);
        }

        /// <summary>
        /// Initializes a ViewModel if it's not initialized.
        /// </summary>
        /// <param name="vm"></param>
        /// <returns></returns>
        public static async Task EnsureInitialization(this IViewModel vm)
        {
            if (!vm.IsInitialized)
                await vm.Initialize();
        }

        /// <summary>
        /// Creates an <see cref="AsyncObservableCollectionView{T}"/> from a collection.
        /// </summary>
        public static AsyncObservableCollectionView<T> ToAsyncObservableCollectionView<T>(this IEnumerable<T> collection, FilterHandler<T> filterPredicate)
        {
            return AsyncObservableCollectionView<T>.Create(filterPredicate, collection);
        }

        /// <summary>
        /// Creates an <see cref="AsyncObservableCollectionView{T}"/> from a collection.
        /// </summary>
        public static async Task<AsyncObservableCollectionView<T>> ToAsyncObservableCollectionView<T>(this Task<IEnumerable<T>> collection, FilterHandler<T> filterPredicate)
        {
            return AsyncObservableCollectionView<T>.Create(filterPredicate, await collection);
        }

        /// <summary>
        /// Create a new dialog instance and then calls the show method of the instance.
        /// </summary>
        /// <param name="dialogs">The dialog service</param>
        /// <param name="title">The title of the dialog.</param>
        /// <param name="content">The content of the dialog.</param>
        /// <param name="options">Dialog options.</param>
        /// <param name="actions">The actions of the dialog.</param>
        public static IDialog ShowDialog(this IDialogService dialogs, string title, UIElement content, DialogOptions options, params ActionCommandBase[] actions)
        {
            var dialog = dialogs.CreateDialog(title, content, options, actions);
            dialog.Show();
            return dialog;
        }

        /// <summary>
        /// Create a new dialog instance and then calls the show method of the instance.
        /// </summary>
        /// <param name="dialogs">The dialog service</param>
        /// <param name="content">The content of the dialog.</param>
        /// <param name="options">Dialog options.</param>
        /// <param name="actions">The actions of the dialog.</param>
        public static IDialog ShowDialog(this IDialogService dialogs, UIElement content, DialogOptions options, params ActionCommandBase[] actions)
        {
            var dialog = dialogs.CreateDialog(content, options, actions);
            dialog.Show();
            return dialog;
        }

        /// <summary>
        /// Create a new dialog instance and then calls the show method of the instance.
        /// </summary>
        /// <param name="dialogs">The dialog service</param>
        /// <param name="title">The title of the dialog.</param>
        /// <param name="content">The content of the dialog.</param>
        /// <param name="actions">The actions of the dialog.</param>
        public static IDialog ShowDialog(this IDialogService dialogs, string title, UIElement content, params ActionCommandBase[] actions)
        {
            var dialog = dialogs.CreateDialog(title, content, actions);
            dialog.Show();
            return dialog;
        }

        /// <summary>
        /// Create a new dialog instance and then calls the show method of the instance.
        /// </summary>
        /// <param name="dialogs">The dialog service</param>
        /// <param name="content">The content of the dialog.</param>
        /// <param name="actions">The actions of the dialog.</param>
        public static IDialog ShowDialog(this IDialogService dialogs, UIElement content, params ActionCommandBase[] actions)
        {
            var dialog = dialogs.CreateDialog(content, actions);
            dialog.Show();
            return dialog;
        }

        /// <summary>
        /// Create a new dialog instance and then calls the show method of the instance.
        /// </summary>
        /// <param name="dialogs">The dialog service</param>
        /// <param name="title">The title of the dialog.</param>
        /// <param name="editor">The content of the dialog.</param>
        /// <param name="options">Dialog options.</param>
        public static IDialog ShowDialog(this IDialogService dialogs, string title, IEditorViewModel editor, DialogOptions options)
        {
            var dialog = dialogs.CreateDialog(title, editor, options);
            dialog.Show();
            return dialog;
        }

        /// <summary>
        /// Create a new dialog instance and then calls the show method of the instance.
        /// </summary>
        /// <param name="dialogs">The dialog service</param>
        /// <param name="editor">The content of the dialog.</param>
        /// <param name="options">Dialog options.</param>
        public static IDialog ShowDialog(this IDialogService dialogs, IEditorViewModel editor, DialogOptions options)
        {
            var dialog = dialogs.CreateDialog(editor, options);
            dialog.Show();
            return dialog;
        }

        /// <summary>
        /// Create a new dialog instance and then calls the show method of the instance.
        /// </summary>
        /// <param name="dialogs">The dialog service</param>
        /// <param name="title">The title of the dialog.</param>
        /// <param name="editor">The content of the dialog.</param>
        public static IDialog ShowDialog(this IDialogService dialogs, string title, IEditorViewModel editor)
        {
            var dialog = dialogs.CreateDialog(title, editor);
            dialog.Show();
            return dialog;
        }

        /// <summary>
        /// Create a new dialog instance and then calls the show method of the instance.
        /// </summary>
        /// <param name="dialogs">The dialog service</param>
        /// <param name="editor">The content of the dialog.</param>
        public static IDialog ShowDialog(this IDialogService dialogs, IEditorViewModel editor)
        {
            var dialog = dialogs.CreateDialog(editor);
            dialog.Show();
            return dialog;
        }

        /// <summary>
        /// Create a confirmation dialog.
        /// </summary>
        /// <param name="dialogs">The dialog service</param>
        /// <param name="message">The dialog message.</param>
        /// <param name="yesAction">An action that will be executed when yes button is pressed</param>
        /// <param name="noAction">An action that will be executed when no button is pressed</param>
        /// <returns></returns>
        public static IDialog ShowConfirmation(this IDialogService dialogs, string message, Action yesAction, Action noAction = null)
        {
            var dialog = dialogs.CreateConfirmation(message, yesAction, noAction);
            dialog.Show();
            return dialog;
        }

        /// <summary>
        /// Create a new dialog instance and then calls the show method of the instance.
        /// </summary>
        /// <param name="dialogs">The dialog service</param>
        /// <param name="presentable">The title of the dialog.</param>
        public static IDialog ShowDialog(this IDialogService dialogs, IPresentable presentable)
        {
            var dialog = dialogs.CreateDialog(presentable);
            dialog.Show();
            return dialog;
        }

        /// <summary>
        /// Create a new dialog instance and then calls the show method of the instance.
        /// </summary>
        /// <param name="dialogs">The dialog service</param>
        /// <param name="presentable">The content of the dialog.</param>
        /// <param name="options">Dialog options.</param>
        public static IDialog ShowDialog(this IDialogService dialogs, IPresentable presentable, DialogOptions options)
        {
            var dialog = dialogs.CreateDialog(presentable, options);
            dialog.Show();
            return dialog;
        }

        /// <summary>
        /// Create a new dialog instance and then calls the show method of the instance.
        /// </summary>
        /// <param name="dialogs">The dialog service</param>
        /// <param name="presentable">The content of the dialog.</param>
        /// <param name="options">Dialog options.</param>
        /// <param name="actions">The actions of the dialog.</param>
        public static IDialog ShowDialog(this IDialogService dialogs, IPresentable presentable, DialogOptions options, params ActionCommandBase[] actions)
        {
            var dialog = dialogs.CreateDialog(presentable, options, actions);
            dialog.Show();
            return dialog;
        }

        /// <summary>
        /// Create a new dialog instance and then calls the show method of the instance.
        /// </summary>
        /// <param name="dialogs">The dialog service</param>
        /// <param name="title">The title of the dialog.</param>
        /// <param name="presentable">The content of the dialog.</param>
        public static IDialog ShowDialog(this IDialogService dialogs, string title, IPresentable presentable)
        {
            var dialog = dialogs.CreateDialog(title, presentable);
            dialog.Show();
            return dialog;
        }

        /// <summary>
        /// Create a new dialog instance and then calls the show method of the instance.
        /// </summary>
        /// <param name="dialogs">The dialog service</param>
        /// <param name="title">The title of the dialog.</param>
        /// <param name="presentable">The content of the dialog.</param>
        /// <param name="options">Dialog options.</param>
        public static IDialog ShowDialog(this IDialogService dialogs, string title, IPresentable presentable, DialogOptions options)
        {
            var dialog = dialogs.CreateDialog(title, presentable, options);
            dialog.Show();
            return dialog;
        }

        /// <summary>
        /// Create a new dialog instance and then calls the show method of the instance.
        /// </summary>
        /// <param name="dialogs">The dialog service</param>
        /// <param name="title">The title of the dialog.</param>
        /// <param name="presentable">The content of the dialog.</param>
        /// <param name="options">Dialog options.</param>
        /// <param name="actions">The actions of the dialog.</param>
        public static IDialog ShowDialog(this IDialogService dialogs, string title, IPresentable presentable, DialogOptions options, params ActionCommandBase[] actions)
        {
            var dialog = dialogs.CreateDialog(title, presentable, options, actions);
            dialog.Show();
            return dialog;
        }
    }
}