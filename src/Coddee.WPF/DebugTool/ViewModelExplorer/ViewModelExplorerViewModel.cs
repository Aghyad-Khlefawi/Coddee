using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Coddee.Collections;
using Coddee.Services;
using Coddee.Services.ViewModelManager;
using Coddee.WPF.Commands;

namespace Coddee.WPF.DebugTool
{
    public class ViewModelExplorerViewModel : ViewModelBase<ViewModelExplorerView>
    {
        public ViewModelExplorerViewModel()
        {
            
        }
        public ViewModelExplorerViewModel(IViewModelsManager vmManager, PropertyMainpulatorViewModel mainpulator)
        {
            _vmManager = vmManager;
            _mainpulator = mainpulator;
        }


        private PropertyMainpulatorViewModel _mainpulator;

        private UIElement _viewModelManipulatorContent;
        public UIElement ViewModelManipulatorContent
        {
            get { return _viewModelManipulatorContent; }
            set { SetProperty(ref this._viewModelManipulatorContent, value); }
        }

        private AsyncObservableCollection<ViewModelNavigationItem> _navigationStack;
        public AsyncObservableCollection<ViewModelNavigationItem> NavigationStack
        {
            get { return _navigationStack; }
            set { SetProperty(ref this._navigationStack, value); }
        }

        private AsyncObservableCollection<ViewModelNavigationItem> _viewModels;
        public AsyncObservableCollection<ViewModelNavigationItem> ViewModels
        {
            get { return _viewModels; }
            set { SetProperty(ref this._viewModels, value); }
        }

        private ViewModelInfo _currentViewModel;
        public ViewModelInfo CurrentViewModel
        {
            get { return _currentViewModel; }
            set { SetProperty(ref this._currentViewModel, value); }
        }

        protected override void OnDesignMode()
        {
            base.OnDesignMode();
            NavigationStack = new AsyncObservableCollection<ViewModelNavigationItem>();
            ViewModels = new AsyncObservableCollection<ViewModelNavigationItem>();

            var shellVM = new ViewModelNavigationItem(new ViewModelInfo(new ViewModelMock("ShellViewModel") { IsSelected = true }));
            var mainVM = new ViewModelNavigationItem(new ViewModelInfo(new ViewModelMock("MainViewModel")));
            shellVM.ViewModelInfo.ChildViewModels.Add(mainVM.ViewModelInfo);
            mainVM.ViewModelInfo.ParentViewModel = shellVM.ViewModelInfo;

            _mainpulator = new PropertyMainpulatorViewModel { Properties = new AsyncObservableCollection<IViewModelProperty>() };
            _viewModelManipulatorContent = new PropertyMainpulatorView { DataContext = _mainpulator };
            SetAsCurrentVM(shellVM.ViewModelInfo);
        }

        protected override async Task OnInitialization()
        {
            await base.OnInitialization();
            NavigationStack = AsyncObservableCollection<ViewModelNavigationItem>.Create();
            ViewModels = AsyncObservableCollection<ViewModelNavigationItem>.Create();


            await _mainpulator.Initialize();
            ViewModelManipulatorContent = _mainpulator.GetView();
        }

        public ICommand LoadCommand => new RelayCommand(Load);

        public void Load()
        {
            IsBusy = true;
            ViewModels.Clear();
            NavigationStack.Clear();

            var root = CreateNavigationItem(_vmManager.RootViewModel);
            SetAsCurrentVM(root.ViewModelInfo);
            IsBusy = false;
        }

        private ViewModelNavigationItem CreateNavigationItem(ViewModelInfo root)
        {
            var nav = new ViewModelNavigationItem(root);
            nav.OnNavigate += (sender, args) => SetAsCurrentVM(args.ViewModelInfo);
            return nav;
        }

        private void SetAsCurrentVM(ViewModelInfo arg)
        {
            ViewModels.Clear();
            NavigationStack.Clear();

            CurrentViewModel = arg;
            ViewModels.Fill(arg.ChildViewModels.Select(CreateNavigationItem));

            var item = arg;
            while (item != null)
            {
                NavigationStack.Insert(0, CreateNavigationItem(item));
                item = item.ParentViewModel;
            }
            _mainpulator.SetViewModel(arg);
        }
    }
}