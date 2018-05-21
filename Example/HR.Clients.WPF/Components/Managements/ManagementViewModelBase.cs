using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Coddee;
using Coddee.Collections;
using Coddee.Commands;
using Coddee.Data;
using Coddee.Mvvm;
using Coddee.WPF;
using Coddee.WPF.Commands;

namespace HR.Clients.WPF.Components.Managements
{
    public abstract class ManagementViewModelBase<TView, TEditor, TRepository, TModel, TKey> : ViewModelBase<TView>, IManagementViewModel
        where TView : UIElement, new()
        where TEditor : IEditorViewModel<TModel>
        where TRepository : ICRUDRepository<TModel, TKey>
        where TModel : IUniqueObject<TKey>, new()
    {
        private TEditor _editor;
        private TRepository _repository;

        private AsyncObservableCollectionView<TModel> _itemList;
        public AsyncObservableCollectionView<TModel> ItemList
        {
            get { return _itemList; }
            set { SetProperty(ref this._itemList, value); }
        }

        private TModel _selectedItem;
        public TModel SelectedItem
        {
            get { return _selectedItem; }
            set { SetProperty(ref this._selectedItem, value); }
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
            get { return _editCommand ?? (_editCommand = CreateReactiveCommand(this, Edit).ObserveProperty(e => e.SelectedItem)); }
            set { SetProperty(ref _editCommand, value); }
        }

        private IReactiveCommand _deleteCommand;
        public IReactiveCommand DeleteCommand
        {
            get { return _deleteCommand ?? (_deleteCommand = CreateReactiveCommand(this, Delete).ObserveProperty(e => e.SelectedItem)); }
            set { SetProperty(ref _deleteCommand, value); }
        }

        protected override async Task OnInitialization()
        {
            await base.OnInitialization();
            _repository = GetRepository<TRepository>();
            _editor = CreateViewModel<TEditor>();
            _editor.Saved += EditorSaved            ;
            ItemList = await GetItems().ToAsyncObservableCollectionView(Filter);
            ItemList.BindToRepositoryChanges(_repository);
        }

        protected abstract bool Filter(TModel item, string term);

        private void EditorSaved(object sender, EditorSaveArgs<TModel> e)
        {
            ToastSuccess();
        }

        protected virtual Task<IEnumerable<TModel>> GetItems()
        {
            return _repository.GetItems();
        }

        public void Delete()
        {
            _dialogService.CreateConfirmation(string.Format("Are you sure you want to delete the item '{0}'", GetItemDescription(SelectedItem)),
                                              async () =>
                                              {
                                                  await _repository.DeleteItem(SelectedItem);
                                                  ToastSuccess();
                                              }).Show();
        }

        protected virtual string GetItemDescription(TModel item)
        {
            return item.ToString();
        }

        public async void Edit()
        {
            await _editor.Initialize();
            _editor.Edit(SelectedItem);
            _editor.Show();
        }

        private async void Add()
        {
            await _editor.Initialize();
            _editor.Add();
            _editor.Show();
        }

        public abstract string Title { get; }
    }
}