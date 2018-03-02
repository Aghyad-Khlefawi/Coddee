// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using Coddee.Collections;
using Coddee.Exceptions;
using Coddee.Mvvm;
using Coddee.WPF;
using Coddee.WPF.Services.Dialogs;

namespace Coddee.Services.Dialogs
{
    /// <inheritdoc cref="IDialogService"/>
    public class DialogService : ViewModelBase<DialogServiceView>, IDialogService
    {
        private int _maxZindex;

        private AsyncObservableCollection<IDialog> _activeDialogs;

        /// <summary>
        /// A collection containing the currently active dialogs.
        /// </summary>
        public AsyncObservableCollection<IDialog> ActiveDialogs
        {
            get { return _activeDialogs; }
            set { SetProperty(ref _activeDialogs, value); }
        }


        private AsyncObservableCollection<IDialog> _minimizedDialogs;
        /// <summary>
        /// A collection containing the currently minimized dialogs.
        /// </summary>
        public AsyncObservableCollection<IDialog> MinimizedDialogs
        {
            get { return _minimizedDialogs; }
            set { SetProperty(ref _minimizedDialogs, value); }
        }

        /// <inheritdoc />
        public event EventHandler<IDialog> DialogStateChanged;
        /// <inheritdoc />
        public event EventHandler<IDialog> DialogClosed;

        /// <inheritdoc />
        public IDialog CreateDialog(string title, object content, DialogOptions options, params ActionCommandBase[] commands)
        {
            var container = CreateViewModel<DialogContainerViewModel>();
            container.ZIndex = _maxZindex;
            container.SetDialogOptions(options);
            container.SetCommands(commands);
            container.SetContent(content);
            container.StateChanged += OnDialogStateChanged;
            container.Title = title;
            container.Closed += ContainerClosed;
            Interlocked.Increment(ref _maxZindex);
            return container;
        }
        /// <inheritdoc />
        public IDialog CreateDialog(object content, DialogOptions options, params ActionCommandBase[] commands)
        {
            return CreateDialog(null, content, options, commands);
        }

        /// <inheritdoc />
        public IDialog CreateDialog(string title, object content, params ActionCommandBase[] actions)
        {
            return CreateDialog(title, content, DialogOptions.Default, actions);
        }
        /// <inheritdoc />
        public IDialog CreateDialog(object content, params ActionCommandBase[] actions)
        {
            return CreateDialog(null, content, actions);
        }

        /// <inheritdoc />
        public IDialog CreateDialog(string title, IEditorViewModel editor, DialogOptions options)
        {
            var dialog = CreateDialog(title,
                                (UIElement)editor.GetView(),
                                options,
                                new CloseActionCommand(_localization.GetValue("Cancel")),
                                new AsyncActionCommand(_localization.GetValue("Save"),
                                                  async () =>
                                                  {
                                                      try
                                                      {
                                                          await editor.Save();
                                                          return true;
                                                      }
                                                      catch (ValidationException)
                                                      {
                                                          return false;
                                                      }
                                                  })
                                { Tag = ActionCommandTags.SaveCommand });
            return dialog;
        }
        /// <inheritdoc />
        public IDialog CreateDialog(IEditorViewModel editor, DialogOptions options)
        {
            return CreateDialog(editor.FullTitle, editor, options);
        }

        /// <inheritdoc />
        public IDialog CreateDialog(string title, IEditorViewModel editor)
        {
            return CreateDialog(title, editor, DialogOptions.Default);
        }
        /// <inheritdoc />
        public IDialog CreateDialog(IEditorViewModel editor)
        {
            return CreateDialog(editor.FullTitle, editor);
        }

        /// <inheritdoc />
        public IDialog CreateDialog(IPresentable presentable)
        {
            return CreateDialog(null, presentable, DialogOptions.Default);
        }
        /// <inheritdoc />
        public IDialog CreateDialog(IPresentable presentable, DialogOptions options)
        {
            return CreateDialog(null, presentable, options);
        }
        /// <inheritdoc />
        public IDialog CreateDialog(IPresentable presentable, DialogOptions options, params ActionCommandBase[] actions)
        {
            return CreateDialog(null, presentable, options, actions);
        }

        /// <inheritdoc />
        public IDialog CreateDialog(string title, IPresentable presentable)
        {
            return CreateDialog(title, presentable.GetView(), DialogOptions.Default);
        }
        /// <inheritdoc />
        public IDialog CreateDialog(string title, IPresentable presentable, DialogOptions options)
        {
            return CreateDialog(title, presentable.GetView(), options);
        }

        /// <inheritdoc />
        public IDialog CreateDialog(string title, IPresentable presentable, DialogOptions options, params ActionCommandBase[] actions)
        {
            return CreateDialog(title, presentable.GetView(), options, actions);
        }

        /// <inheritdoc />
        public IDialog CreateConfirmation(string message, Action yesAction, Action noAction = null)
        {
            return CreateDialog(_localization["Confirmation"],
                                new TextBlock
                                {
                                    TextAlignment = TextAlignment.Center,
                                    Text = message
                                },
                                new DialogOptions
                                {
                                    HorizontalAlignment = HorizontalAlignment.Center,
                                    VerticalAlignment = VerticalAlignment.Center,
                                    ContentVerticalAlignment = VerticalAlignment.Center,
                                    ContentHorizontalAlignment = HorizontalAlignment.Center,
                                    CanMinimize = false,
                                    InitialState = DialogState.Active
                                },
                                new CloseActionCommand(_localization["No"], noAction),
                                new CloseActionCommand(_localization["Yes"], yesAction));
        }

        private void ContainerClosed(object sender, EventArgs e)
        {
            if (sender is IDialog dialog)
            {
                MinimizedDialogs.RemoveIfExists(dialog);
                ActiveDialogs.RemoveIfExists(dialog);
                DialogClosed?.Invoke(this, dialog);
            }
        }

        /// <inheritdoc />
        public void Initialize(Region dialogRegion)
        {
            dialogRegion.View(this);
            ActiveDialogs = AsyncObservableCollection<IDialog>.Create();
            MinimizedDialogs = AsyncObservableCollection<IDialog>.Create();
        }

        /// <inheritdoc />
        public IEnumerable<IDialog> GetActiveDialogs()
        {
            return ActiveDialogs.ToList();
        }

        /// <inheritdoc />
        public IEnumerable<IDialog> GetMinimizedDialogs()
        {
            return MinimizedDialogs.ToList();
        }

        private void OnDialogStateChanged(object sender, DialogState e)
        {
            if (sender is IDialog dialog)
            {

                switch (e)
                {
                    case DialogState.Active:
                        if (ActiveDialogs.Contains(dialog))
                            return;
                        MinimizedDialogs.RemoveIfExists(dialog);
                        ActiveDialogs.Add(dialog);
                        break;
                    case DialogState.Minimized:
                        if (MinimizedDialogs.Contains(dialog))
                            return;
                        ActiveDialogs.RemoveIfExists(dialog);
                        MinimizedDialogs.Add(dialog);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(e), e, null);
                }
                DialogStateChanged?.Invoke(this, dialog);
            }
        }
    }
}