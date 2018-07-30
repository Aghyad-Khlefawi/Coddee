using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Coddee;
using Coddee.Collections;
using Coddee.Commands;
using Coddee.Data;
using Coddee.Xamarin.Forms;
using HR.Data.Models;
using HR.Data.Repositories;
using Xamarin.Forms;

namespace HR.Clients.Components
{
    public class MainPageViewModel : ViewModelBase<MainPage>
    {
        private BranchesViewModel _branchesViewModel;

        private AsyncObservableCollection<Company> _companies;
        public AsyncObservableCollection<Company> Companies
        {
            get { return _companies; }
            set { SetProperty(ref _companies, value); }
        }

        private IReactiveCommand _selectCompanyCommand;
        public IReactiveCommand SelectCompanyCommand
        {
            get { return _selectCompanyCommand ?? (_selectCompanyCommand = CreateReactiveCommand<MainPageViewModel, Company>(this, SelectCompany)); }
            set { SetProperty(ref _selectCompanyCommand, value); }
        }

        public async void SelectCompany(Company company)
        {
            if (company == null)
                return;
            Device.BeginInvokeOnMainThread(() => { Navigation.PushAsync(_branchesViewModel.GetDefaultView()); });

            await _branchesViewModel.Initialize();
            await _branchesViewModel.SetCompany(company);
            Companies.SelectedItem = null;
        }
        protected override async Task OnInitialization()
        {
            await base.OnInitialization();
            _branchesViewModel = CreateViewModel<BranchesViewModel>();

            Companies = await GetRepository<ICompanyRepository>().GetItemsWithDetails().ToAsyncObservableCollection();
        }
    }
}
