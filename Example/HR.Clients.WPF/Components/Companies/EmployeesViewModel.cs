using System;
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
        private IEmployeeJobEditor _employeeJobEditor;

        private AsyncObservableCollection<EmployeeJob> _employeeJobs;
        public AsyncObservableCollection<EmployeeJob> EmployeeJobs
        {
            get { return _employeeJobs; }
            set { SetProperty(ref _employeeJobs, value); }
        }

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
            set
            {
                SetProperty(ref this._selectedEmployee, value);
                OnEmployeeSelected(value);
            }
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

        private IReactiveCommand _addJobCommand;

        public IReactiveCommand AddJobCommand
        {
            get { return _addJobCommand ?? (_addJobCommand = CreateReactiveCommand(this, AddJob).ObserveProperty(e => e.SelectedEmployee)); }
            set { SetProperty(ref _addJobCommand, value); }
        }

        protected override void OnDesignMode()
        {
            base.OnDesignMode();
            EmployeeJobs = new AsyncObservableCollection<EmployeeJob>
            {
                new EmployeeJob
                {
                    BranchName = "Branch1",
                    CompanyName = "Samsongs",
                    DepartmentTitle = "Development",
                    EmployeeFirstName = "Aghyad",
                    EmployeeLastName = "Khlefawi",
                    JobTitle = "Developer",
                    StartDate = DateTime.Now
                },
                new EmployeeJob
                {
                    BranchName = "Branch1",
                    CompanyName = "Samsongs",
                    DepartmentTitle = "Development",
                    EmployeeFirstName = "Aghyad",
                    EmployeeLastName = "Khlefawi",
                    JobTitle = "Developer",
                    StartDate = DateTime.Now,
                    EndDate = DateTime.Now.AddYears(1)
                }
            };
        }

        public async void AddJob()
        {
            await ToggleBusyAsync(_employeeJobEditor.Initialize());
            _employeeJobEditor.AddFromEmployee(SelectedEmployee.Id);
            _employeeJobEditor.Show();
        }

        protected override async Task OnInitialization()
        {
            await base.OnInitialization();
            _employeeEditor = CreateViewModel<IEmployeeEditor>();
            _employeeJobEditor = CreateViewModel<IEmployeeJobEditor>();
            _employeeRepository = GetRepository<IEmployeeRepository>();

            EmployeeList = await _employeeRepository.GetItemsWithDetailes().ToAsyncObservableCollection();
            EmployeeList.BindToRepositoryChangesAsync(_employeeRepository,
                                                      async e => await _employeeRepository.GetItemWithDetailes(e.Id),
                                                      e => EmployeeList.First(e.Id));
        }

        private async void OnEmployeeSelected(Employee value)
        {
            async Task LoadEmployees()
            {
                EmployeeJobs = await _employeeRepository.GetEmployeeJobsByEmployee(value.Id)
                                                        .ToAsyncObservableCollection();
            }

            await ToggleBusyAsync(LoadEmployees());
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
                                              })
                          .Show();
        }
    }
}