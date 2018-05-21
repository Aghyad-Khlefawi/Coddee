using System.Threading.Tasks;
using System.Windows.Input;
using Coddee;
using Coddee.Collections;
using Coddee.Commands;
using Coddee.Data;
using Coddee.WPF;
using Coddee.WPF.Commands;
using HR.Clients.WPF.Interfaces;
using HR.Data.Models;
using HR.Data.Repositories;

namespace HR.Clients.WPF.Components.Companies
{
    public class EmployeesViewModel : ViewModelBase<EmployeesView>
    {
        private IEmployeeRepository _employeeRepository;
        private IEmployeeEditor _employeeEditor;




        private AsyncObservableCollection<Employee> _employeeList;
        public AsyncObservableCollection<Employee> EmployeeList
        {
            get { return _employeeList; }
            set { SetProperty(ref this._employeeList, value); }
        }

        private Employee _selectedEmployee;
        public Employee SelectedEmployee
        {
            get { return _selectedEmployee; }
            set { SetProperty(ref this._selectedEmployee, value); }
        }

        private ICommand _addCommand;
        public ICommand AddCommand
        {
            get { return _addCommand ?? (_addCommand = new RelayCommand(Add)); }
            set { SetProperty(ref _addCommand, value); }
        }

        private IReactiveCommand _editCommand;
        public IReactiveCommand EditCommand
        {
            get { return _editCommand ?? (_editCommand = CreateReactiveCommand(this, Edit).ObserveProperty(e => e.SelectedEmployee)); }
            set { SetProperty(ref _editCommand, value); }
        }

        private IReactiveCommand _deleteCommand;
        public IReactiveCommand DeleteCommand
        {
            get { return _deleteCommand ?? (_deleteCommand = CreateReactiveCommand(this, Delete).ObserveProperty(e => e.SelectedEmployee)); }
            set { SetProperty(ref _deleteCommand, value); }
        }



        protected override async Task OnInitialization()
        {
            await base.OnInitialization();
            _employeeEditor = CreateViewModel<IEmployeeEditor>();

            _employeeRepository = GetRepository<IEmployeeRepository>();
            EmployeeList = await _employeeRepository.GetItems().ToAsyncObservableCollection();
            EmployeeList.BindToRepositoryChanges(_employeeRepository);
        }

        public async void Edit()
        {
            await _employeeEditor.Initialize();
            _employeeEditor.Edit(SelectedEmployee);
            _employeeEditor.Show();
        }

        private async void Add()
        {
            await _employeeEditor.Initialize();
            _employeeEditor.Add();
            _employeeEditor.Show();
        }
        public void Delete()
        {
            _dialogService.CreateConfirmation(string.Format("Are you sure you want to delete the item '{0}'", SelectedEmployee.FullName),
                                              async () =>
                                              {
                                                  await _employeeRepository.DeleteItem(SelectedEmployee);
                                                  ToastSuccess();
                                              }).Show();
        }

    }
}