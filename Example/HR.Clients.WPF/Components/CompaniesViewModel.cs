using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Coddee.Collections;
using Coddee.Data;
using Coddee.WPF;
using HR.Data.Models;
using HR.Data.Repositories;

namespace HR.Clients.WPF.Components
{
    public class CompaniesViewModel : ViewModelBase<CompaniesView>
    {
        private AsyncObservableCollection<Company> _companies;
        public AsyncObservableCollection<Company> Companies
        {
            get { return _companies; }
            set { SetProperty(ref _companies, value); }
        }

        protected override async Task OnInitialization()
        {
            await base.OnInitialization();
            var companyRepository = GetRepository<ICompanyRepository>();
            Companies = await companyRepository.ToAsyncObservableCollection();
        }
    }
}