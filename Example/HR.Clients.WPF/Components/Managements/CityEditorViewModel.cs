using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Coddee.Collections;
using Coddee.Data;
using Coddee.Validation;
using Coddee.WPF;
using HR.Data.Models;
using HR.Data.Repositories;

namespace HR.Clients.WPF.Components
{
    public class CityEditorViewModel : EditorViewModelBase<CityEditorViewModel, CityEditorView, ICityRepository, City, int>
    {
        public CityEditorViewModel()
        {
            Title = "City";
        }

        private string _name;
        [EditorField]
        public string Name
        {
            get { return _name; }
            set { SetProperty(ref _name, value); }
        }

        private AsyncObservableCollection<Country> _countries;
        public AsyncObservableCollection<Country> Countries
        {
            get { return _countries; }
            set { SetProperty(ref _countries, value); }
        }

        private Country _selectedCountry;
        [EditorField]
        public Country SelectedCountry
        {
            get { return _selectedCountry; }
            set { SetProperty(ref _selectedCountry, value); }
        }

        protected override async Task OnInitialization()
        {
            await base.OnInitialization();
            Countries = await GetRepository<ICountryRepository>().ToAsyncObservableCollection();
        }

        protected override void MapEditedItemToEditor(City item)
        {
            base.MapEditedItemToEditor(item);
            SelectedCountry = Countries.FirstOrDefault(e => e.Id == item.CountryId);
        }

        protected override void MapEditorToEditedItem(City item)
        {
            base.MapEditorToEditedItem(item);
            item.CountryId = SelectedCountry.Id;
            item.CountryName = SelectedCountry.Name;
        }

        protected override void SetValidationRules(List<IValidationRule> validationRules)
        {
            base.SetValidationRules(validationRules);
            validationRules.Add(ValidationRule.CreateErrorRule(() => Name));
            validationRules.Add(ValidationRule.CreateErrorRule(() => SelectedCountry));
        }
    }
}