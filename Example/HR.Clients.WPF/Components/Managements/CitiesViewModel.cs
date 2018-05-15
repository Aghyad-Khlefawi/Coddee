using System.Threading.Tasks;
using System.Windows.Input;
using Coddee.Collections;
using Coddee.Commands;
using Coddee.Data;
using Coddee.WPF;
using Coddee.WPF.Commands;
using HR.Clients.WPF.Interfaces;
using HR.Data.Models;
using HR.Data.Repositories;

namespace HR.Clients.WPF.Components
{
    public class CitiesViewModel : ViewModelBase<CitiesView>,IManagementViewModel
    {
        public string Title
        {
            get { return "Cities"; }
        }

        private ICityRepository _cityRepository;
        private ICityEditor _cityEditorViewModel;

        private AsyncObservableCollectionView<City> _cities;
        public AsyncObservableCollectionView<City> Cities
        {
            get { return _cities; }
            set { SetProperty(ref _cities, value); }
        }

        private City _selectedCity;
        public City SelectedCity
        {
            get { return _selectedCity; }
            set { SetProperty(ref _selectedCity, value); }
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
            get { return _editCommand ?? (_editCommand = CreateReactiveCommand(this, Edit).ObserveProperty(e => e.SelectedCity)); }
            set { SetProperty(ref _editCommand, value); }
        }

        private IReactiveCommand _deleteCommand;
        public IReactiveCommand DeleteCommand
        {
            get { return _deleteCommand ?? (_deleteCommand = CreateReactiveCommand(this, Delete).ObserveProperty(e => e.SelectedCity)); }
            set { SetProperty(ref _deleteCommand, value); }
        }
      

        protected override async Task OnInitialization()
        {
            await base.OnInitialization();
            _cityRepository = GetRepository<ICityRepository>();
            _cityEditorViewModel = CreateViewModel<ICityEditor>();
            _cityEditorViewModel.Saved += CityEditorSaved;

            Cities = await _cityRepository.GetItemsWithDetails().ToAsyncObservableCollectionView(Filter);
            Cities.BindToRepositoryChanges(_cityRepository);
        }

        private void CityEditorSaved(object sender, Coddee.EditorSaveArgs<City> e)
        {
            ToastSuccess();
        }

        private bool Filter(City item, string term)
        {
            return item.Name.ToLower().Contains(term.ToLower());
        }

        public void Delete()
        {
            _dialogService.CreateConfirmation(string.Format("Are you sure you want to delete the item '{0}'", SelectedCity.Name),
                                              async () =>
                                              {
                                                  await _cityRepository.DeleteItem(SelectedCity);
                                                  ToastSuccess();
                                              }).Show();
        }

        public async void Edit()
        {
            await _cityEditorViewModel.Initialize();
            _cityEditorViewModel.Edit(SelectedCity);
            _cityEditorViewModel.Show();
        }

        private async void Add()
        {
            await _cityEditorViewModel.Initialize();
            _cityEditorViewModel.Add();
            _cityEditorViewModel.Show();
        }
    }
}