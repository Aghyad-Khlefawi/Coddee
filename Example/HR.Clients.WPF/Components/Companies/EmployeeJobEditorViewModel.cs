using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Coddee;
using Coddee.Collections;
using Coddee.Validation;
using Coddee.WPF;
using HR.Clients.WPF.Interfaces;
using HR.Data.Models;
using HR.Data.Repositories;

namespace HR.Clients.WPF.Components.Companies
{
    public class EmployeeJobEditorViewModel : EditorViewModelBase<EmployeeJobEditorViewModel, EmployeeJobEditorView, EmployeeJob>, IEmployeeJobEditor
    {
        private IBranchRepository _branchRepository;

        private AsyncObservableCollection<Employee> _employeeList;
        public AsyncObservableCollection<Employee> EmployeeList
        {
            get { return _employeeList; }
            set { SetProperty(ref this._employeeList, value); }
        }

        private Employee _selectedEmployee;
        [EditorField]
        public Employee SelectedEmployee
        {
            get { return _selectedEmployee; }
            set { SetProperty(ref this._selectedEmployee, value); }
        }

        private AsyncObservableCollection<Job> _jobList;
        public AsyncObservableCollection<Job> JobList
        {
            get { return _jobList; }
            set { SetProperty(ref this._jobList, value); }
        }

        private Job _selectedJob;
        [EditorField]
        public Job SelectedJob
        {
            get { return _selectedJob; }
            set { SetProperty(ref this._selectedJob, value); }
        }

        private AsyncObservableCollection<Branch> _branchList;
        public AsyncObservableCollection<Branch> BranchList
        {
            get { return _branchList; }
            set { SetProperty(ref this._branchList, value); }
        }

        private Branch _selectedBranch;
        [EditorField]
        public Branch SelectedBranch
        {
            get { return _selectedBranch; }
            set { SetProperty(ref this._selectedBranch, value); }
        }

        private AsyncObservableCollection<Department> _departmentList;
        public AsyncObservableCollection<Department> DepartmentList
        {
            get { return _departmentList; }
            set { SetProperty(ref this._departmentList, value); }
        }

        private Department _selectedDepartment;
        [EditorField]
        public Department SelectedDepartment
        {
            get { return _selectedDepartment; }
            set { SetProperty(ref this._selectedDepartment, value); }
        }

        private DateTime _startDate;
        [EditorField]
        public DateTime StartDate
        {
            get { return _startDate; }
            set { SetProperty(ref _startDate, value); }
        }

        private DateTime? _endDate;
        [EditorField]
        public DateTime? EndDate
        {
            get { return _endDate; }
            set { SetProperty(ref _endDate, value); }
        }

        private AsyncObservableCollection<Company> _companyList;
        public AsyncObservableCollection<Company> CompanyList
        {
            get { return _companyList; }
            set { SetProperty(ref this._companyList, value); }
        }

        private Company _selectedCompany;
        [EditorField]
        public Company SelectedCompany
        {
            get { return _selectedCompany; }
            set
            {
                SetProperty(ref this._selectedCompany, value);
                OnCompanySelected(value);
            }
        }



        private bool _canChangeEmployee;
        public bool CanChangeEmployee
        {
            get { return _canChangeEmployee; }
            set { SetProperty(ref _canChangeEmployee, value); }
        }

        private bool _canChangeBranch;
        public bool CanChangeBranch
        {
            get { return _canChangeBranch; }
            set { SetProperty(ref _canChangeBranch, value); }
        }

        protected override async Task OnInitialization()
        {
            await base.OnInitialization();
            _branchRepository = GetRepository<IBranchRepository>();


            await Task.WhenAll(
                               GetRepository<IEmployeeRepository>().GetItems().ToAsyncObservableCollection().ContinueWith(res => EmployeeList = res.Result),
                               GetRepository<IDepartmentRepository>().GetItems().ToAsyncObservableCollection().ContinueWith(res => DepartmentList = res.Result),
                               GetRepository<IJobRepository>().GetItems().ToAsyncObservableCollection().ContinueWith(res => JobList = res.Result),
                               GetRepository<ICompanyRepository>().GetItems().ToAsyncObservableCollection().ContinueWith(res => CompanyList = res.Result)
                               );
        }


        protected override async Task MapEditorToEditedItem(EmployeeJob item)
        {
            await base.MapEditorToEditedItem(item);
            item.BranchName = this.SelectedBranch.Name;
            item.BranchId = this.SelectedBranch.Id;

            item.DepartmentTitle = this.SelectedDepartment.Title;
            item.DepartmentId = this.SelectedDepartment.Id;

            item.JobTitle = this.SelectedJob.Title;
            item.JobId = this.SelectedJob.Id;

            item.EmployeeFirstName = this.SelectedEmployee.FirstName;
            item.EmployeeLastName = this.SelectedEmployee.LastName;
            item.EmployeeId = this.SelectedEmployee.Id;
        }

        protected override async Task MapEditedItemToEditor(EmployeeJob item)
        {
            await base.MapEditedItemToEditor(item);

            SelectedEmployee = EmployeeList.First(e => e.Id == item.EmployeeId);
            SelectedJob = JobList.First(e => e.Id == item.JobId);
            SelectedDepartment = DepartmentList.First(e => e.Id == item.DepartmentId);
            SelectedCompany = CompanyList.First(e => e.Id == item.CompanyId);
            BranchList = (await _branchRepository.GetItemsWithDetailsByCompany(item.CompanyId)).ToAsyncObservableCollection();
            SelectedBranch = BranchList.First(e => e.Id == item.BranchId);
        }

        public void AddFromEmployee(int employeeId)
        {
            base.Add();
            CanChangeEmployee = false;
            CanChangeBranch = true;
            SelectedEmployee = EmployeeList.First(e => e.Id == employeeId);
        }

        public override void Clear()
        {
            base.Clear();
            StartDate = DateTime.Today;
        }

        protected override void SetValidationRules(List<IValidationRule> validationRules)
        {
            base.SetValidationRules(validationRules);
            validationRules.Add(ValidationRule.CreateErrorRule(() => SelectedEmployee));
            validationRules.Add(ValidationRule.CreateErrorRule(() => SelectedBranch));
            validationRules.Add(ValidationRule.CreateErrorRule(() => SelectedDepartment));
            validationRules.Add(ValidationRule.CreateErrorRule(() => SelectedJob));
        }

        protected override Task<EmployeeJob> SaveItem()
        {
            return GetRepository<IEmployeeRepository>().InsertEmployeeJob(EditedItem);
        }

        private async void OnCompanySelected(Company value)
        {
            if (FillingValues || value == null)
                return;

            BranchList = (await _branchRepository.GetItemsWithDetailsByCompany(value.Id)).ToAsyncObservableCollection();
        }
    }
}