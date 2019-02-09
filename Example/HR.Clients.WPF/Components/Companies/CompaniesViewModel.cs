using System;
using System.Threading.Tasks;
using Coddee.Collections;
using Coddee.Data;
using Coddee.WPF;
using HR.Data.Models;
using HR.Data.Repositories;
using Coddee;
using Coddee.Commands;
using HR.Clients.WPF.Interfaces;

namespace HR.Clients.WPF.Components
{
    public class CompaniesViewModel : ViewModelBase<CompaniesView>
    {
        private ICompanyEditor _companyEditor;
        private IBranchEditor _branchEditor;
        private ICompanyRepository _companyRepository;

        private IBranchViewer _branchViewer;
        public IBranchViewer BranchViewer
        {
            get { return _branchViewer; }
            set { SetProperty(ref _branchViewer, value); }
        }

        private AsyncObservableCollection<Company> _companies;
        public AsyncObservableCollection<Company> Companies
        {
            get { return _companies; }
            set { SetProperty(ref _companies, value); }
        }

        private Company _selectedCompany;
        public Company SelectedCompany
        {
            get { return _selectedCompany; }
            set
            {
                SetProperty(ref _selectedCompany, value);
                OnCompanySelected(value);
            }
        }


        private IReactiveCommand _addCompanyCommand;
        public IReactiveCommand AddCompanyCommand
        {
            get { return _addCompanyCommand ?? (_addCompanyCommand = CreateReactiveCommand(AddCompany)); }
            set { SetProperty(ref _addCompanyCommand, value); }
        }

        private IReactiveCommand _editCompanyCommand;
        public IReactiveCommand EditCompanyCommand
        {
            get { return _editCompanyCommand ?? (_editCompanyCommand = CreateReactiveCommand(this, Edit).ObserveProperty(e => e.SelectedCompany)); }
            set { SetProperty(ref _editCompanyCommand, value); }
        }

        private IReactiveCommand _deleteCompanyCommand;
        public IReactiveCommand DeleteCompanyCommand
        {
            get { return _deleteCompanyCommand ?? (_deleteCompanyCommand = CreateReactiveCommand(this, Delete).ObserveProperty(e => e.SelectedCompany)); }
            set { SetProperty(ref _deleteCompanyCommand, value); }
        }

        private IReactiveCommand _createBranchCommand;
        public IReactiveCommand CreateBranchCommand
        {
            get { return _createBranchCommand ?? (_createBranchCommand = CreateReactiveCommand(this,CreateBranch).ObserveProperty(e=>e.SelectedCompany)); }
            set { SetProperty(ref _createBranchCommand, value); }
        }

        public async void CreateBranch()
        {
            await _branchEditor.Initialize();
            _branchEditor.Add(SelectedCompany.Id);
            _branchEditor.Show();
        }

        public async void AddCompany()
        {
            await _companyEditor.Initialize();
            _companyEditor.Add();
            _companyEditor.Show();
        }

        protected override void OnDesignMode()
        {
            base.OnDesignMode();
            Companies = new AsyncObservableCollection<Company>
            {
               
                new Company
                {
                    Name="Microsoft"
                },
                new Company
                {
                    Name="Apple"
                },
                new Company
                {
                    Name="Google"
                },
                new Company
                {
                    Name="Amazon"
                }
            };
        }

        protected override async Task OnInitialization()
        {
            await base.OnInitialization();
            _companyEditor = CreateViewModel<ICompanyEditor>();
            _companyEditor.Saved += CompanyEditorSaved;

            _branchEditor = CreateViewModel<IBranchEditor>();
            _companyRepository = GetRepository<ICompanyRepository>();
            
            async Task LoadCompanies()
            {
                Companies = await _companyRepository.GetItemsWithDetails(DateTime.Now).ToAsyncObservableCollection();
                Companies.BindToRepositoryChanges(_companyRepository);
            }

            async Task InitializeBranchViewer()
            {
                BranchViewer = await InitializeViewModel<IBranchViewer>();
            }

            try
            {
                await Task.WhenAll(LoadCompanies(), InitializeBranchViewer());
            }
            catch (Exception e)
            {
                throw;
            }
        }

        private void CompanyEditorSaved(object sender, EditorSaveArgs<Company> e)
        {
            ToastSuccess();
        }

        public void Delete()
        {
            _dialogService.CreateConfirmation(string.Format("Are you sure you want to delete the item '{0}'", SelectedCompany.Name),
                                              async () =>
                                              {
                                                  await _companyRepository.DeleteItem(SelectedCompany);
                                                  ToastSuccess();
                                              }).Show();
        }

        public async void Edit()
        {
            await _companyEditor.Initialize();
            _companyEditor.Edit(SelectedCompany);
            _companyEditor.Show();
        }


        private async void OnCompanySelected(Company value)
        {
            await _branchViewer.SetCompany(value.Id);
        }
    }
}