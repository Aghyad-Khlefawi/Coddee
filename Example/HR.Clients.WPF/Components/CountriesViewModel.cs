using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Coddee.Collections;
using Coddee.Data;
using Coddee.WPF;
using Coddee.WPF.Commands;
using HR.Data.Models;
using HR.Data.Repositories;

namespace HR.Clients.WPF.Components
{
    public class CountriesViewModel : ViewModelBase<CountriesView>,IManagementViewModel
    {
        public CountriesViewModel()
        {
            
        }

        public string Title
        {
            get { return "Countries"; }
        }

        private CountryEditorViewModel _countryEditor;
        private ICountryRepository _countryRepository;



        private AsyncObservableCollectionView<Country> _countries;
        public AsyncObservableCollectionView<Country> Countries
        {
            get { return _countries; }
            set { SetProperty(ref _countries, value); }
        }

        private Country _selectedCountry;
        public Country SelectedCountry
        {
            get { return _selectedCountry; }
            set { SetProperty(ref _selectedCountry, value); }
        }

        private ICommand _addCommand;
        public ICommand AddCommand
        {
            get { return _addCommand ?? (_addCommand = new RelayCommand(Add)); }
            set { SetProperty(ref _addCommand, value); }
        }

        private IReactiveCommand _editCommand;
        public IReactiveCommand EditCommand
        {
            get { return _editCommand ?? (_editCommand = CreateReactiveCommand(this, Edit).ObserveProperty(e => e.SelectedCountry)); }
            set { SetProperty(ref _editCommand, value); }
        }

        private IReactiveCommand _deleteCommand;
        public IReactiveCommand DeleteCommand
        {
            get { return _deleteCommand ?? (_deleteCommand = CreateReactiveCommand(this, Delete).ObserveProperty(e => e.SelectedCountry)); }
            set { SetProperty(ref _deleteCommand, value); }
        }

        protected override async Task OnInitialization()
        {
            await base.OnInitialization();
            _countryRepository = GetRepository<ICountryRepository>();
            _countryEditor = CreateViewModel<CountryEditorViewModel>();
            _countryEditor.Saved += CountryEditorSaved;
            Countries = await _countryRepository.ToAsyncObservableCollectionView(filter);
        }

        private void CountryEditorSaved(object sender, Coddee.EditorSaveArgs<Country> e)
        {
            ToastSuccess();
        }

        private bool filter(Country item, string term)
        {
            return item.Name.ToLower().Contains(term.ToLower());
        }

        public void Delete()
        {
            _dialogService.CreateConfirmation(string.Format("Are you sure you want to delete the item '{0}'", SelectedCountry.Name),
                                              async () =>
                                              {
                                                  await _countryRepository.DeleteItem(SelectedCountry);
                                                  ToastSuccess();
                                              }).Show();
        }

        public async void Edit()
        {
            await _countryEditor.Initialize();
            _countryEditor.Edit(SelectedCountry);
            _countryEditor.Show();
        }

        private async void Add()
        {
            await _countryEditor.Initialize();
            _countryEditor.Add();
            _countryEditor.Show();
        }

    }
}