// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Coddee.Collections;
using Coddee.Services.Dialogs;
using Coddee.WPF.Commands;

namespace Coddee.WPF.Services.Dialogs
{
    public class DialogContainerViewModel : ViewModelBase<DialogContainerView>, IDialog
    {
        public DialogContainerViewModel()
        {
            Commands = AsyncObservableCollection<ActionCommandWrapper>.Create();
        }

        public event EventHandler Closed;
        public event EventHandler<DialogState> StateChanged;

        private DialogOptions _dialogOptions;

        private AsyncObservableCollection<ActionCommandWrapper> _commands;
        public AsyncObservableCollection<ActionCommandWrapper> Commands
        {
            get { return _commands; }
            set { SetProperty(ref _commands, value); }
        }

        private string _title;
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        private int _zIndex;
        public int ZIndex
        {
            get { return _zIndex; }
            set { SetProperty(ref _zIndex, value); }
        }

        private DialogState _state;
        public DialogState State
        {
            get { return _state; }
            set { SetProperty(ref _state, value); }
        }

        private VerticalAlignment _contentVerticalAlignment;
        public VerticalAlignment ContentVerticalAlignment
        {
            get { return _contentVerticalAlignment; }
            set { SetProperty(ref _contentVerticalAlignment, value); }
        }

        private HorizontalAlignment _contentHorizontalAlignment;
        public HorizontalAlignment ContentHorizontalAlignment
        {
            get { return _contentHorizontalAlignment; }
            set { SetProperty(ref _contentHorizontalAlignment, value); }
        }

        private VerticalAlignment _verticalAlignment;
        public VerticalAlignment VerticalAlignment
        {
            get { return _verticalAlignment; }
            set { SetProperty(ref _verticalAlignment, value); }
        }

        private HorizontalAlignment _horizontalAlignment;
        public HorizontalAlignment HorizontalAlignment
        {
            get { return _horizontalAlignment; }
            set { SetProperty(ref _horizontalAlignment, value); }
        }

        private UIElement _content;
        public UIElement Content
        {
            get { return _content; }
            set { SetProperty(ref _content, value); }
        }

        private bool _canMinimize;
        public bool CanMinimize
        {
            get { return _canMinimize; }
            set { SetProperty(ref _canMinimize, value); }
        }
        private IReactiveCommand _minimizeCommand;
        public IReactiveCommand MinimizeCommand
        {
            get { return _minimizeCommand ?? (_minimizeCommand = CreateReactiveCommand(Minimize)); }
            set { SetProperty(ref _minimizeCommand, value); }
        }

        private IReactiveCommand _showCommand;
        public IReactiveCommand ShowCommand
        {
            get { return _showCommand ?? (_showCommand = CreateReactiveCommand(Show)); }
            set { SetProperty(ref _showCommand, value); }
        }

        private IReactiveCommand _closeCommand;
        public IReactiveCommand CloseCommand
        {
            get { return _closeCommand ?? (_closeCommand = CreateReactiveCommand(Close)); }
            set { SetProperty(ref _closeCommand, value); }
        }

        public void Minimize()
        {
            SetState(DialogState.Minimized);
        }


        public void SetCommands(params ActionCommandBase[] commands)
        {
            Commands.ClearAndFill(commands.Select(e =>
            {
                if (e is CloseActionCommand closeAction)
                    return new ActionCommandWrapper(closeAction, this);
                return new ActionCommandWrapper(e, this);
            }));
        }

        public void SetState(DialogState newState)
        {
            State = newState;
            StateChanged?.Invoke(this, newState);
        }

        public void SetContent(UIElement content)
        {
            Content = content;
        }

        protected override void OnDesignMode()
        {
            base.OnDesignMode();
            SetDialogOptions(DialogOptions.Default);
            Commands = new AsyncObservableCollection<ActionCommandWrapper>
            {
                new ActionCommandWrapper{Title = "Save",HorizontalPosition = Dock.Right},
                new ActionCommandWrapper{Title = "Next",HorizontalPosition = Dock.Right},
                new ActionCommandWrapper{Title = "Cancel",HorizontalPosition = Dock.Left},
                new ActionCommandWrapper{Title = "Back",HorizontalPosition = Dock.Left},
            };
        }

        public void Show()
        {
            SetState(DialogState.Active);
        }

        public void Close()
        {
            Closed?.Invoke(this, EventArgs.Empty);
        }

        public void SetDialogOptions(DialogOptions options)
        {
            _dialogOptions = options;
            State = options.InitialState;
            ContentVerticalAlignment = options.ContentVerticalAlignment;
            ContentHorizontalAlignment = options.ContentHorizontalAlignment;
            VerticalAlignment = options.VerticalAlignment;
            HorizontalAlignment = options.HorizontalAlignment;
            CanMinimize = options.CanMinimize;
        }
    }
}