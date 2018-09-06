using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Coddee.Collections;
using Coddee.Commands;
using Coddee.Xamarin.Forms;
using HR.Data.Models;
using HR.Data.Repositories;
using Xamarin.Forms;

namespace HR.Clients.Components
{
    public class BranchesViewModel : ViewModelBase<BranchesPage>
    {
        private Company _company;
        private EmployeesViewModel _employeesViewModel; 

        private AsyncObservableCollection<Branch> _branches;
        public AsyncObservableCollection<Branch> Branches
        {
            get { return _branches; }
            set { SetProperty(ref _branches, value); }
        }

        private IReactiveCommand _selectBranchCommand;
        public IReactiveCommand SelectBranchCommand
        {
            get { return _selectBranchCommand ?? (_selectBranchCommand = CreateReactiveCommand<BranchesViewModel, Branch>(this, SelectBranch)); }
            set { SetProperty(ref _selectBranchCommand, value); }
        }

        public async void SelectBranch(Branch branch)
        {
            if (branch == null)
                return;
            NavigationPush(_employeesViewModel.GetDefaultView());

            await _employeesViewModel.Initialize();
            await _employeesViewModel.SetBranch(branch);
            Branches.SelectedItem = null;
        }
        protected override async Task OnInitialization()
        {
            await base.OnInitialization();
            Branches = AsyncObservableCollection<Branch>.Create();
            _employeesViewModel = CreateViewModel<EmployeesViewModel>();
        }

        public async Task SetCompany(Company company)
        {
            _company = company;
            await ToggleBusyAsync(Branches.ClearAndFillAsync(GetRepository<IBranchRepository>().GetItemsWithDetailsByCompany(company.Id)));
        }
    }
}
