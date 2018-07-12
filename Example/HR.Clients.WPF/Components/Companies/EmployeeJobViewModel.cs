using System;
using Coddee.Commands;
using Coddee.WPF;
using HR.Data.Models;
using HR.Data.Repositories;

namespace HR.Clients.WPF.Components.Companies
{
    public class EmployeeJobViewModel : ViewModelBase<EmployeeJobView>
    {
        private EmployeeJob _employeeJob;
        public EmployeeJob EmployeeJob
        {
            get { return _employeeJob; }
            set { SetProperty(ref _employeeJob, value); }
        }

        private IReactiveCommand _deleteCommand;
        public IReactiveCommand DeleteCommand
        {
            get { return _deleteCommand ?? (_deleteCommand = CreateReactiveCommand(Delete)); }
            set { SetProperty(ref _deleteCommand, value); }
        }
        protected override void OnDesignMode()
        {
            base.OnDesignMode();
            EmployeeJob = new EmployeeJob
            {
                BranchName = "Branch1",
                CompanyName = "Samsongs",
                DepartmentTitle = "Development",
                EmployeeFirstName = "Aghyad",
                EmployeeLastName = "Khlefawi",
                JobTitle = "Developer",
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddYears(1)
            };
        }

        public void Delete()
        {
            _dialogService.ShowConfirmation("Are you sure you want to delete this employee job.",
                                            async () =>
                                            {
                                                try
                                                {
                                                    await GetRepository<IEmployeeRepository>().DeleteEmployeeJob(EmployeeJob);
                                                }
                                                catch (Exception e)
                                                {
                                                    ToastError("Something went wrong while deleting the job.");
                                                    LogError(e);
                                                }
                                            });
        }

        public void SetEmployeeJob(EmployeeJob job)
        {
            EmployeeJob = job;
        }
    }
}