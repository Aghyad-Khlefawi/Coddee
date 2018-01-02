using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Coddee.Collections;
using Coddee.WPF.Commands;

namespace Coddee.WPF.Services.Dialogs
{
    public class DialogsExplorerViewModel : ViewModelBase<DialogsExplorerView>, IDialogExplorer
    {
        public DialogsExplorerViewModel()
        {

        }

        private IReactiveCommand _showCommand;
        public IReactiveCommand ShowCommand
        {
            get { return _showCommand ?? (_showCommand = CreateReactiveCommand<DialogsExplorerViewModel, DialogContainerViewModel>(this, Show)); }
            set { SetProperty(ref _showCommand, value); }
        }

        public void Show(DialogContainerViewModel dialog)
        {
            dialog.Show();
            IsOpen = false;
        }

        private AsyncObservableCollection<DialogContainerViewModel> _dialogs;
        public AsyncObservableCollection<DialogContainerViewModel> Dialogs
        {
            get { return _dialogs; }
            set { SetProperty(ref _dialogs, value); }
        }

        private IReactiveCommand _toggleCommand;
        public IReactiveCommand ToggleCommand
        {
            get { return _toggleCommand ?? (_toggleCommand = CreateReactiveCommand(Toggle)); }
            set { SetProperty(ref _toggleCommand, value); }
        }

        private double _horizontalOffset;
        public double HorizontalOffset
        {
            get { return _horizontalOffset; }
            set { SetProperty(ref _horizontalOffset, value); }
        }

        private double _verticalOffset;
        public double VerticalOffset
        {
            get { return _verticalOffset; }
            set { SetProperty(ref _verticalOffset, value); }
        }

        private double _width;
        public double Width
        {
            get { return _width; }
            set { SetProperty(ref _width, value); }
        }

        private bool _isOpen;
        public bool IsOpen
        {
            get { return _isOpen; }
            set { SetProperty(ref _isOpen, value); }
        }

        private int _dialogsCount;
        public int DialogsCount
        {
            get { return _dialogsCount; }
            set { SetProperty(ref _dialogsCount, value); }
        }

        protected override void OnDesignMode()
        {
            base.OnDesignMode();
            Dialogs = new AsyncObservableCollection<DialogContainerViewModel>
            {
                new DialogContainerViewModel
                {
                    Title = "Dialog1",
                    Content = new Border{Background = new SolidColorBrush(Colors.Green)}
                },
                new DialogContainerViewModel
                {
                    Title = "Dialog2",
                    Content = new Border{Background = new SolidColorBrush(Colors.Red)}
                },
                new DialogContainerViewModel
                {
                    Title = "Dialog3",
                    Content = new Border{Background = new SolidColorBrush(Colors.Beige)}
                },
                new DialogContainerViewModel
                {
                    Title = "Dialog4",
                    Content = new Border{Background = new SolidColorBrush(Colors.Blue)}
                },
            };
        }

        public void Toggle()
        {
            IsOpen = !IsOpen;
        }

        protected override async Task OnInitialization()
        {
            await base.OnInitialization();
            Dialogs = AsyncObservableCollection<DialogContainerViewModel>.Create(_dialogService.GetMinimizedDialogs().Cast<DialogContainerViewModel>());
            DialogsCount = Dialogs.Count;
            Dialogs.CollectionChanged += delegate
            {
                DialogsCount = Dialogs.Count;
            };
            _dialogService.DialogStateChanged += DialogServiceDialogStateChanged;
            _dialogService.DialogClosed += DialogClosed;

            var shell = (Window)Resolve<IShell>();
            SetPopupOffset(shell);
            ExecuteOnUIContext(() =>
            {
                shell.SizeChanged += delegate
                {
                    SetPopupOffset(shell);
                };
                shell.StateChanged += delegate
                {
                    SetPopupOffset(shell);
                };
                shell.LocationChanged += delegate
                {
                    SetPopupOffset(shell);
                };
            });
        }

        private void SetPopupOffset(Window shell)
        {
            ExecuteOnUIContext(() =>
            {
                HorizontalOffset = shell.Left;
                VerticalOffset = shell.Top + 35;
                Width = shell.ActualWidth;
            });
        }

        private void DialogClosed(object sender, Coddee.Services.Dialogs.IDialog e)
        {
            var dialog = e as DialogContainerViewModel;
            Dialogs.RemoveIfExists(dialog);
        }

        private void DialogServiceDialogStateChanged(object sender, Coddee.Services.Dialogs.IDialog e)
        {
            var dialog = e as DialogContainerViewModel;
            switch (e.State)
            {
                case DialogState.Active:
                    Dialogs.RemoveIfExists(dialog);
                    break;
                case DialogState.Minimized:
                    if (!Dialogs.Contains(dialog))
                        Dialogs.Add(dialog);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}