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
using Coddee;
using Coddee.Commands;
using HR.Clients.WPF.Interfaces;

namespace HR.Clients.WPF.Components
{
    public class SelectableCompany : SelectableItem<Company>
    {
        public SelectableCompany(Company item, bool isSelected = false) : base(item, isSelected)
        {
        }
    }
    public class CompaniesViewModel : ViewModelBase<CompaniesView>
    {
        private ICompanyEditor _companyEditor;
        private SelectableCompany _lastSelected;
        private ICompanyRepository _companyRepository;

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
                ToastInformation($"Selected {value.Name}");
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
                //new SelectableCompany(new Company
                //{
                //    Name="Microsoft"
                //}),
                //new SelectableCompany(new Company
                //{
                //    Name="Apple"
                //}),
                //new SelectableCompany(new Company
                //{
                //    Name="Google"
                //},true),
                //new SelectableCompany(new Company
                //{
                //    Name="Amazon"
                //})
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
            _companyEditor.Saved += companyEditorSaved;

            _companyRepository = GetRepository<ICompanyRepository>();
            Companies = await _companyRepository.ToAsyncObservableCollection();
            //Companies = (await _companyRepository.GetItems()).Select(CreateSelectableCompany).ToAsyncObservableCollection();
            //Companies.BindToRepositoryChanges(_companyRepository, CreateSelectableCompany, e => Companies.First(c => c.Item.Id.Equals(e.Id)));
        }

        private SelectableCompany CreateSelectableCompany(Company sourceitem)
        {
            var item = new SelectableCompany(sourceitem);
            item.Selected += (s, e) =>
            {
                if (_lastSelected != null)
                    _lastSelected.IsSelected = false;

                _lastSelected = (SelectableCompany)s;
                SelectedCompany = e;
            };
            return item;
        }

        private void companyEditorSaved(object sender, EditorSaveArgs<Company> e)
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

    }
}