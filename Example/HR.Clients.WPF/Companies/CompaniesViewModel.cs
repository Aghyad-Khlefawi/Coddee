// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using System.Threading.Tasks;
using Coddee;
using Coddee.Collections;
using Coddee.WPF;
using Coddee.WPF.Collections;
using Coddee.WPF.Commands;
using HR.Clients.WPF.Companies.Editors;
using HR.Data.Models;
using HR.Data.Repositories;

namespace HR.Clients.WPF.Companies
{
    public class CompaniesViewModel : ViewModelBase<CompaniesView>
    {
        private CompanyEditorViewModel _companyEditor;
        private EmployeeEditorViewModelBase _employeeEditor;

        private AsyncObservableDictionaryView<Guid, Company> _companies;
        public AsyncObservableDictionaryView<Guid, Company> Companies
        {
            get { return _companies; }
            set { SetProperty(ref this._companies, value); }
        }

        private Company _selectedCompany;
        public Company SelectedCompany
        {
            get { return _selectedCompany; }
            set
            {
                SetProperty(ref _selectedCompany, value);
                CompanySelected(value);
            }
        }

        private AsyncObservableCollectionView<Employee> _employees;
        public AsyncObservableCollectionView<Employee> Employees
        {
            get { return _employees; }
            set { SetProperty(ref this._employees, value); }
        }
        private Employee _selectedEmployee;
        public Employee SelectedEmployee
        {
            get { return _selectedEmployee; }
            set { SetProperty(ref this._selectedEmployee, value); }
        }

        private IReactiveCommand _addCompanyCommand;
        public IReactiveCommand AddCompanyCommand
        {
            get { return _addCompanyCommand ?? (_addCompanyCommand = CreateReactiveCommand(AddCompany)); }
            set { SetProperty(ref _addCompanyCommand, value); }
        }

        private IReactiveCommand _editCompanyCommand;
        public IReactiveCommand EditCompanyCommand
        {
            get
            {
                return _editCompanyCommand ?? (_editCompanyCommand = CreateReactiveCommand(this, EditCompany)
                                                   .ObserveProperty(e => e.SelectedCompany));
            }
            set { SetProperty(ref _editCompanyCommand, value); }
        }

        private IReactiveCommand _deleteCompanyCommand;
        public IReactiveCommand DeleteCompanyCommand
        {
            get
            {
                return _deleteCompanyCommand ?? (_deleteCompanyCommand = CreateReactiveCommand(this, DeleteCompany)
                                                     .ObserveProperty(e => e.SelectedCompany));
            }
            set { SetProperty(ref _deleteCompanyCommand, value); }
        }


        private IReactiveCommand _addEmployeeCommand;
        public IReactiveCommand AddEmployeeCommand
        {
            get { return _addEmployeeCommand ?? (_addEmployeeCommand = CreateReactiveCommand(AddEmployee)); }
            set { SetProperty(ref _addEmployeeCommand, value); }
        }

        private IReactiveCommand _editEmployeeCommand;
        public IReactiveCommand EditEmployeeCommand
        {
            get { return _editEmployeeCommand ?? (_editEmployeeCommand = CreateReactiveCommand(EditEmployee)); }
            set { SetProperty(ref _editEmployeeCommand, value); }
        }

        private IReactiveCommand _deleteEmployeeCommand;
        public IReactiveCommand DeleteEmployeeCommand
        {
            get { return _deleteEmployeeCommand ?? (_deleteEmployeeCommand = CreateReactiveCommand(DeleteEmployee)); }
            set { SetProperty(ref _deleteEmployeeCommand, value); }
        }


        private void DeleteEmployee()
        {
            //TODO update
            _dialogService
                .ShowConfirmation($"Are you sure you want to delete '{Employees.SelectedItem.FullName}'?",
                                  async () =>
                                  {
                                      await Resolve<IEmployeeRepository>().DeleteItem(Employees.SelectedItem);
                                      Employees.Remove(Employees.SelectedItem);
                                  });
        }

        private void EditEmployee()
        {
            _employeeEditor.Edit(Employees.SelectedItem, SelectedCompany);
            _employeeEditor.Show();
        }

        private void AddEmployee()
        {
            _employeeEditor.Add(SelectedCompany);
            _employeeEditor.Show();
        }
        private void DeleteCompany()
        {
            _dialogService.CreateConfirmation($"Are you sure you want to delete '{SelectedCompany.Name}'?",
                                  async () =>
                                  {
                                      await Resolve<ICompanyRepository>().DeleteItem(SelectedCompany);
                                      Companies.Remove(SelectedCompany);
                                  }).Show();
        }

        private void EditCompany()
        {
            _companyEditor.Edit(SelectedCompany);
            _companyEditor.Show();
        }

        private void AddCompany()
        {
            _companyEditor.Add();
            _companyEditor.Show();
        }

        protected override async Task OnInitialization()
        {
            var companyRepo = Resolve<ICompanyRepository>();

            Companies = AsyncObservableDictionaryView<Guid, Company>.Create(CompanySearch, await companyRepo.GetDetailedItems(Guid.Empty,DateTime.Now));
            Employees = AsyncObservableCollectionView<Employee>.Create(EmployeeSearch);
            
            _companyEditor = CreateViewModel<CompanyEditorViewModel>();
            _companyEditor.Saved += CompanySaved;

            _employeeEditor = CreateViewModel<EmployeeEditorViewModelBase>();
            _employeeEditor.Saved += EmployeeSaved;

            await InitializeChildViewModels();

        }

        private bool EmployeeSearch(Employee item, string search)
        {
            return item.FirstName.ToLower().Contains(search) || item.LastName.ToLower().Contains(search) || item.CompanyName.ToLower()
                       .Contains(search) || item.StateName.ToLower().Contains(search);
        }

        private bool CompanySearch(Company item, string search)
        {
            return item.Name.ToLower().Contains(search) ||
                   item.StateName.ToLower().Contains(search);
        }

        private void CompanySaved(object sernder, EditorSaveArgs<Company> args)
        {
            Companies.Update(args.OperationType, args.Item);
            ToastSuccess();
        }

        private async void CompanySelected(Company e)
        {
            if (e != null)
            {
                Employees.Clear();
                await Employees.FillAsync(Resolve<IEmployeeRepository>().GetEmployeesByCompany(e.ID));
            }
        }

        private void EmployeeSaved(object sernder, EditorSaveArgs<Employee> args)
        {
            if (args.Item.CompanyID == SelectedCompany.ID)
            {
                Employees.Update(args.OperationType, args.Item);
            }
            else
                Employees.Remove(e => e.ID == args.Item.ID);
            ToastSuccess();
        }

    }
}