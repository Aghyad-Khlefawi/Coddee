// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Coddee.Collections;
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

        public async Task<IDialog> CreateDialog(string title, UIElement content, DialogOptions options, params ActionCommand[] commands)
        {
            var container = CreateViewModel<DialogContainerViewModel>();
            container.ZIndex = _maxZindex;
            await container.Initialize();
            container.SetDialogOptions(options);
            container.SetCommands(commands);
            container.SetContent(content);
            container.StateChanged += OnDialogStateChanged;
            container.Title = title;
            container.Closed += ContainerClosed;
            Interlocked.Increment(ref _maxZindex);
            return container;
        }

        public Task<IDialog> CreateDialog(string title, UIElement content, params ActionCommand[] actions)
        {
            return CreateDialog(title, content, DialogOptions.Default, actions);
        }

        public async Task<IDialog> CreateDialog(string title, IEditorViewModel editor, DialogOptions options)
        {
            var dialog = await CreateDialog(title,
                                editor.GetView(),
                                options,
                                new CloseActionCommand(_localization.GetValue("Save"), async () => { await editor.Save(); }),
                                new CloseActionCommand(_localization.GetValue("Cancel")));
            return dialog;
        }

        public Task<IDialog> CreateDialog(string title, IEditorViewModel editor)
        {
            return CreateDialog(title, editor, DialogOptions.Default);
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