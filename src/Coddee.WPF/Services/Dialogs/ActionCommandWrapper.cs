using System.Windows.Controls;
using System.Windows.Input;
using Coddee.Exceptions;
using Coddee.Services.Dialogs;
using Coddee.WPF.Commands;

namespace Coddee.WPF.Services.Dialogs
{
    /// <summary>
    /// A bindable wrapper for <see cref="ActionCommandBase"/>
    /// </summary>
    public class ActionCommandWrapper : BindableBase
    {
        internal ActionCommandWrapper()
        {
            CanExecute = true;
        }

        /// <param name="action">The wrapped action command</param>
        /// <param name="dialog">The dialog containing the action</param>
        public ActionCommandWrapper(ActionCommandBase action, IDialog dialog)
        {
            if (action is ActionCommand ac)
            {
                if (ac.Action != null)
                    Command = new RelayCommand(ac.Action);
            }
            else if (action is AsyncActionCommand asc)
            {
                if (asc.Action != null)
                    Command = new RelayCommand(async () =>
                    {
                        try
                        {
                            var res = await asc.Action();
                            if (res)
                                dialog.Close();
                        }
                        catch (ValidationException)
                        {
                        }
                    });
            }
            Tag = action.Tag;
            Title = action.Title;
            HorizontalPosition = action.HorizontalPosition == Coddee.HorizontalPosition.Left ? Dock.Left : Dock.Right;
            action.CanExecuteChanged += ActionCanExecuteChanged;
            CanExecute = action.CanExecute;
        }

        /// <param name="action">The wrapped action command</param>
        /// <param name="dialog">The dialog containing the action</param>
        public ActionCommandWrapper(CloseActionCommand action, IDialog dialog)
        {
            Command = new RelayCommand(() =>
            {
                try
                {
                    action.Action?.Invoke();
                    dialog.Close();
                }
                catch (ValidationException)
                {
                }
            });
            Tag = action.Tag;
            Title = action.Title;
            HorizontalPosition = action.HorizontalPosition == Coddee.HorizontalPosition.Left ? Dock.Left : Dock.Right;
            action.CanExecuteChanged += ActionCanExecuteChanged;
            CanExecute = action.CanExecute;
        }


        ///<inheritdoc cref="ActionCommand"/>
        public string Tag { get; set; }
        ///<inheritdoc cref="ActionCommand"/>
        public string Title { get; set; }
        ///<inheritdoc cref="ActionCommand"/>
        public ICommand Command { get; }
        ///<inheritdoc cref="ActionCommand"/>
        public Dock HorizontalPosition { get; set; }

        private bool _canExecute;
        
        ///<inheritdoc cref="ActionCommand"/>
        public bool CanExecute
        {
            get { return _canExecute; }
            set { SetProperty(ref _canExecute, value); }
        }

        private void ActionCanExecuteChanged(object sender, bool e)
        {
            CanExecute = e;
        }
    }
}