using System.Linq;
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
    public class BranchEditorViewModel : EditorViewModelBase<BranchEditorViewModel, BranchEditorView, IBranchRepository, Branch, int>, IBranchEditor
    {
        private ICityRepository _cityRepository;

        private string _name;
        [EditorField]
        public string Name
        {
            get { return _name; }
            set { SetProperty(ref _name, value); }
        }


        private AsyncObservableCollection<Company> _companyList;
        public AsyncObservableCollection<Company> CompanyList
        {
            get { return _companyList; }
            set { SetProperty(ref _companyList, value); }
        }

        private Company _selectedCompany;
        [EditorField]
        public Company SelectedCompany
        {
            get { return _selectedCompany; }
            set { SetProperty(ref _selectedCompany, value); }
        }

        private AsyncObservableCollection<Country> _countryList;
        public AsyncObservableCollection<Country> CountryList
        {
            get { return _countryList; }
            set { SetProperty(ref this._countryList, value); }
        }

        private Country _selectedCountry;
        [EditorField]
        public Country SelectedCountry
        {
            get { return _selectedCountry; }
            set
            {
                SetProperty(ref this._selectedCountry, value);
                if (!FillingValues && value != null)
                {
                    async void Select() => await OnCountrySelected(value);
                    Select();
                }
            }
        }


        private AsyncObservableCollection<City> _cityList;
        public AsyncObservableCollection<City> CityList
        {
            get { return _cityList; }
            set { SetProperty(ref this._cityList, value); }
        }

        private City _selectedCity;
        [EditorField]
        public City SelectedCity
        {
            get { return _selectedCity; }
            set { SetProperty(ref this._selectedCity, value); }
        }

        protected override async Task MapEditedItemToEditor(Branch item)
        {
            SelectedCountry = CountryList.FirstOrDefault(e => e.Id == item.CountryId);
            await OnCountrySelected(SelectedCountry);
            SelectedCity = CityList.FirstOrDefault(e => e.Id == item.CityId);
            SelectedCompany = CompanyList.FirstOrDefault(e => e.Id == item.CompanyId);
            await base.MapEditedItemToEditor(item);
        }

        protected override Task MapEditorToEditedItem(Branch item)
        {
            item.CityId = SelectedCity.Id;
            item.CompanyId = SelectedCompany.Id;
            return base.MapEditorToEditedItem(item);
        }

        public void Add(int companyId)
        {
            base.Add();
            SelectedCompany = CompanyList.FirstOrDefault(e => e.Id == companyId);
        }

        protected override async Task OnInitialization()
        {
            await base.OnInitialization();
            _cityRepository = GetRepository<ICityRepository>();

            CompanyList = await GetRepository<ICompanyRepository>().ToAsyncObservableCollection();
            CountryList = await GetRepository<ICountryRepository>().ToAsyncObservableCollection();
        }


        private async Task OnCountrySelected(Country value)
        {
            CityList = await _cityRepository.GetItemsByCountry(value.Id).ToAsyncObservableCollection();
            CityList.BindToRepositoryChanges(_cityRepository, e => e.CountryId == value.Id);
        }
    }
}