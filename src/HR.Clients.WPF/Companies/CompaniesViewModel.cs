// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System.Threading.Tasks;
using System.Windows.Input;
using Coddee;
using Coddee.WPF;
using Coddee.WPF.Collections;
using Coddee.WPF.Commands;
using Coddee.WPF.Modules.Dialogs;
using HR.Clients.WPF.Companies.Editors;
using HR.Data.Models;
using HR.Data.Repositories;

namespace HR.Clients.WPF.Companies
{
    public class CompaniesViewModel : ViewModelBase<CompaniesView>
    {
        private CompanyEditorViewModel _companyEditor;
        private EmployeeEditorViewModel _employeeEditor;

        private AsyncObservableCollectionView<Company> _companies;
        public AsyncObservableCollectionView<Company> Companies
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

        public ICommand AddCompanyCommand => new RelayCommand(AddCompany);
        public ICommand EditCompanyCommand => new RelayCommand(EditCompany);
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
            _employeeEditor.Edit(Employees.SelectedItem);
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
        }

        private void AddCompany()
        {
            _companyEditor.Add();
        }

        public override async Task Initialize()
        {
            await base.Initialize();
            var companyRepo = Resolve<ICompanyRepository>();

            Companies = AsyncObservableCollectionView<Company>.Create(CompanySearch,
                                                                      await companyRepo.GetItems());

            Employees = AsyncObservableCollectionView<Employee>.Create(EmployeeSearch);

            Companies.SelectedItemChanged += CompanySelected;

            _companyEditor = Resolve<CompanyEditorViewModel>();
            await _companyEditor.Initialize();
            _companyEditor.OnSave += CompanySaved;

            _employeeEditor = Resolve<EmployeeEditorViewModel>();
            await _employeeEditor.Initialize();
            _employeeEditor.OnSave += EmployeeSaved;
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

        private void CompanySaved(OperationType op, Company company)
        {
            Companies.Update(op,company);
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

        private void EmployeeSaved(OperationType op, Employee employee)
        {
            if (employee.CompanyID == Companies.SelectedItem.ID)
            {
                Employees.Update(op, employee);
            }
            else
                Employees.Remove(e=>e.ID == employee.ID);
            ToastSuccess();
        }
        
    }
}