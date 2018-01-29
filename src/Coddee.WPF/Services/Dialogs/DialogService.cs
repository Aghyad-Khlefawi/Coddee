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
using Coddee.WPF;
using Coddee.WPF.Services.Dialogs;

namespace Coddee.Services.Dialogs
{
    public class DialogService : ViewModelBase<DialogServiceView>, IDialogService
    {
        private int _maxZindex;

        private AsyncObservableCollection<IDialog> _activeDialogs;
        public AsyncObservableCollection<IDialog> ActiveDialogs
        {
            get { return _activeDialogs; }
            set { SetProperty(ref _activeDialogs, value); }
        }


        private AsyncObservableCollection<IDialog> _minimizedDialogs;
        public AsyncObservableCollection<IDialog> MinimizedDialogs
        {
            get { return _minimizedDialogs; }
            set { SetProperty(ref _minimizedDialogs, value); }
        }

        public event EventHandler<IDialog> DialogStateChanged;
        public event EventHandler<IDialog> DialogClosed;

        public IDialog CreateDialog(string title, UIElement content, DialogOptions options, params ActionCommandBase[] commands)
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
        public IDialog CreateDialog(UIElement content, DialogOptions options, params ActionCommandBase[] commands)
        {
            return CreateDialog(null, content, options, commands);
        }

        public IDialog CreateDialog(string title, UIElement content, params ActionCommandBase[] actions)
        {
            return CreateDialog(title, content, DialogOptions.Default, actions);
        }
        public IDialog CreateDialog(UIElement content, params ActionCommandBase[] actions)
        {
            return CreateDialog(null, content, actions);
        }

        public IDialog CreateDialog(string title, IEditorViewModel editor, DialogOptions options)
        {
            var dialog = CreateDialog(title,
                                editor.GetView(),
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
                                                      catch (ValidationException e)
                                                      {
                                                          return false;
                                                      }
                                                  }));
            return dialog;
        }
        public IDialog CreateDialog(IEditorViewModel editor, DialogOptions options)
        {
            return CreateDialog(editor.FullTitle, editor, options);
        }

        public IDialog CreateDialog(string title, IEditorViewModel editor)
        {
            return CreateDialog(title, editor, DialogOptions.Default);
        }
        public IDialog CreateDialog(IEditorViewModel editor)
        {
            return CreateDialog(editor.FullTitle, editor);
        }

        public IDialog CreateDialog(IPresentable presentable)
        {
            return CreateDialog(null, presentable, DialogOptions.Default);
        }
        public IDialog CreateDialog(IPresentable presentable, DialogOptions options)
        {
            return CreateDialog(null, presentable, options);
        }
        public IDialog CreateDialog(IPresentable presentable, DialogOptions options, params ActionCommandBase[] actions)
        {
            return CreateDialog(null, presentable, options, actions);
        }

        public IDialog CreateDialog(string title, IPresentable presentable)
        {
            return CreateDialog(title, presentable.GetView(), DialogOptions.Default);
        }
        public IDialog CreateDialog(string title, IPresentable presentable, DialogOptions options)
        {
            return CreateDialog(title, presentable.GetView(), options);
        }
        public IDialog CreateDialog(string title, IPresentable presentable, DialogOptions options, params ActionCommandBase[] actions)
        {
            return CreateDialog(title, presentable.GetView(), options, actions);
        }

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

        public void Initialize(Region dialogRegion)
        {
            dialogRegion.View(this);
            ActiveDialogs = AsyncObservableCollection<IDialog>.Create();
            MinimizedDialogs = AsyncObservableCollection<IDialog>.Create();
        }

        public IEnumerable<IDialog> GetActiveDialogs()
        {
            return ActiveDialogs.ToList();
        }

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