using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Coddee.Collections;
using Coddee.Data;
using Coddee.Xamarin.Forms;
using HR.Data.Models;
using HR.Data.Repositories;

namespace HR.Clients.Components
{
    public class MainPageViewModel : ViewModelBase<MainPage>
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
            try
            {
                Companies = await GetRepository<ICompanyRepository>().ToAsyncObservableCollection();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}
