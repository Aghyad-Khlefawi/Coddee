// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using Coddee;
using Coddee.Collections;
using Coddee.Services;
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
        public CompaniesViewModel()
        {
            EditCompanyCommand = CreateReactiveCommand(this, EditCompany)
                .ObserveProperty(e => e.SelectedEmployee);
        }
        private CompanyEditorViewModel _companyEditor;
        private EmployeeEditorViewModelBase _employeeEditor;

        private AsyncObservableDictionaryView<Guid, Company> _companies;
        public AsyncObservableDictionaryView<Guid, Company> Companies
        {
            get { return _companies; }
            set { SetProperty(ref this._companies, value); }
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
        public ICommand AddCompanyCommand => new RelayCommand(AddCompany);
        public ICommand EditCompanyCommand;
        public ICommand DeleteCompanyCommand => new RelayCommand(DeleteCompany);

        public ICommand AddEmployeeCommand => new RelayCommand(AddEmployee);
        public ICommand EditEmployeeCommand => new RelayCommand(EditEmployee);
        public ICommand DeleteEmployeeCommand => new RelayCommand(DeleteEmployee);

        private void DeleteEmployee()
        {
            Resolve<IDialogService>()
                .ShowConfirmation($"Are you sure you want to delete '{Employees.SelectedItem.FullName}'?",
                                  async () =>
                                  {
                                      await Resolve<IEmployeeRepository>().DeleteItem(Employees.SelectedItem);
                                      Employees.Remove(Employees.SelectedItem);
                                  });
        }

        private void EditEmployee()
        {
            _employeeEditor.Edit(Employees.SelectedItem, Companies.SelectedItem);
        }

        private void AddEmployee()
        {
            _employeeEditor.Add(Companies.SelectedItem);
        }
        private void DeleteCompany()
        {
            Resolve<IDialogService>()
                .ShowConfirmation($"Are you sure you want to delete '{Companies.SelectedItem.Name}'?",
                                  async () =>
                                  {
                                      await Resolve<ICompanyRepository>().DeleteItem(Companies.SelectedItem);
                                      Companies.Remove(Companies.SelectedItem);
                                  });
        }

        private void EditCompany()
        {
            _companyEditor.Edit(Companies.SelectedItem);
            Resolve<IDialogService>().ShowEditorDialog(_companyEditor);
        }

        private void AddCompany()
        {
            _companyEditor.Add();
            Resolve<IDialogService>().ShowEditorDialog(_companyEditor);
        }

        protected override async Task OnInitialization()
        {
            var companyRepo = Resolve<ICompanyRepository>();

            Companies = AsyncObservableDictionaryView<Guid, Company>.Create(CompanySearch, await companyRepo.GetDetailedItems());
            Employees = AsyncObservableCollectionView<Employee>.Create(EmployeeSearch);

            Companies.SelectedItemChanged += CompanySelected;

            _companyEditor = await InitializeViewModel<CompanyEditorViewModel>();
            _companyEditor.Saved += CompanySaved;

            _employeeEditor = await InitializeViewModel<EmployeeEditorViewModelBase>();
            _employeeEditor.Saved += EmployeeSaved;

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

        private async void CompanySelected(object sender, Company e)
        {
            if (e != null)
            {
                Employees.Clear();
                await Employees.FillAsync(Resolve<IEmployeeRepository>().GetEmployeesByCompany(e.ID));
            }
        }

        private void EmployeeSaved(object sernder, EditorSaveArgs<Employee> args)
        {
            if (args.Item.CompanyID == Companies.SelectedItem.ID)
            {
                Employees.Update(args.OperationType, args.Item);
            }
            else
                Employees.Remove(e => e.ID == args.Item.ID);
            ToastSuccess();
        }
        
    }
}