using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using Coddee.WPF;
using Coddee.WPF.Commands;
using HR.Clients.WPF.Components.Managements;

namespace HR.Clients.WPF.Components
{
    public class ManagementMainViewModel : ViewModelBase<ManagementMainView>
    {
        private List<IManagementViewModel> _managements;
        public List<IManagementViewModel> Managements
        {
            get { return _managements; }
            set { SetProperty(ref _managements, value); }
        }

        private ICommand _selectManagementCommand;
        public ICommand SelectManagementCommand
        {
            get { return _selectManagementCommand ?? (_selectManagementCommand = new RelayCommand<IManagementViewModel>(SelectManagement)); }
            set { SetProperty(ref _selectManagementCommand, value); }
        }

        private IManagementViewModel _currentViewModel;
        public IManagementViewModel CurrentViewModel
        {
            get { return _currentViewModel; }
            set { SetProperty(ref _currentViewModel, value); }
        }

        protected override async Task OnInitialization()
        {
            await base.OnInitialization();
            Managements = new List<IManagementViewModel>
            {
                CreateViewModel<CountriesViewModel>(),
                CreateViewModel<CitiesViewModel>(),
                CreateViewModel<JobsViewModel>(),
            };
        }

        private void SelectManagement(IManagementViewModel obj)
        {
            if (!obj.IsInitialized)
                obj.Initialize();
            CurrentViewModel = obj;
        }
    }
}