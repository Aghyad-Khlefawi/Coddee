using System.Threading.Tasks;
using Coddee;
using Coddee.Collections;
using Coddee.Data;
using Coddee.WPF;
using HR.Clients.WPF.Interfaces;
using HR.Data.Models;
using HR.Data.Repositories;

namespace HR.Clients.WPF.Components.Companies
{
    public class BranchViewerViewModel : ViewModelBase<BranchViewerView>,IBranchViewer
    {
        private IBranchRepository _branchRepository;

        private AsyncObservableCollection<Branch> _companyBranches;
        public AsyncObservableCollection<Branch> CompanyBranches
        {
            get { return _companyBranches; }
            set { SetProperty(ref _companyBranches, value); }
        }

        protected override void OnDesignMode()
        {
            base.OnDesignMode();
            CompanyBranches = new AsyncObservableCollection<Branch>
            {
                new Branch
                {
                    Name = "Branch 1",
                    CompanyName = "Company",
                    CityName = "City",
                    CountryName = "Country",
                    EmployeeCount = 12
                }
            };
        }

        protected override async Task OnInitialization()
        {
            await base.OnInitialization();
            _branchRepository = GetRepository<IBranchRepository>();
        }

        public async Task SetCompany(int companyId)
        {
            async Task LoadBranches()
            {
                CompanyBranches = await _branchRepository.GetItemsWithDetailsByCompany(companyId).ToAsyncObservableCollection();
                CompanyBranches.BindToRepositoryChanges(_branchRepository, e => e.CompanyId == companyId);
            }

            await ToggleBusyAsync(LoadBranches());
        }
    }
}