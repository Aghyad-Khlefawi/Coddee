using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Coddee.Data;
using Coddee.Services;

namespace Coddee.WPF
{

    public abstract class EditorViewModelBase<TEditor, TView, TModel> : ViewModelBase<TView>,
        IEditorViewModel<TView, TModel>
        where TView : UIElement, new()
        where TModel : new()
        where TEditor : EditorViewModelBase<TEditor, TView, TModel>

    {
        private const string _eventsSource = "EditorBase";

        protected EditorViewModelBase()
        {
            Saved += OnSave;
            Canceled += OnCanceled;
        }
        
        public event EventHandler<EditorSaveArgs<TModel>> Saved;
        public event EventHandler<EditorSaveArgs<TModel>> Canceled;

        private OperationType _operationType;
        public OperationType OperationType
        {
            get { return _operationType; }
            set { SetProperty(ref this._operationType, value); }
        }

        private TModel _editedItem;
        public TModel EditedItem
        {
            get { return _editedItem; }
            set { SetProperty(ref this._editedItem, value); }
        }


        private bool _fillingValues;
        public bool FillingValues
        {
            get { return _fillingValues; }
            set { SetProperty(ref this._fillingValues, value); }
        }

        public virtual void Add()
        {
            OperationType = OperationType.Add;
            Clear();
            OnAdd();
        }

        public virtual void Clear()
        {
            EditedItem = new TModel();
        }

        public virtual void Edit(TModel item)
        {
            FillingValues = true;
            OperationType = OperationType.Edit;
            Clear();
            EditedItem = _mapper.Map<TModel>(item);
            OnEdit(item);
            FillingValues = false;
        }

        protected override async Task OnInitialization()
        {
            await base.OnInitialization();
            _mapper.RegisterMap<TModel, TModel>();
            _mapper.RegisterTwoWayMap<TEditor, TModel>();
        }

        protected virtual void OnAdd()
        {
        }

        protected virtual void OnEdit(TModel item)
        {
            MapEditedItemToEditor(item);
        }

        public virtual void MapEditedItemToEditor(TModel item)
        {
            _mapper.MapInstance(item, (TEditor)this);
        }

        public virtual void MapEditorToEditedItem(TModel item)
        {
            _mapper.MapInstance((TEditor)this, item);
        }

        public void Cancel()
        {
            Canceled?.Invoke(this, new EditorSaveArgs<TModel>(OperationType, EditedItem));
        }

        public virtual void PreSave()
        {
            MapEditorToEditedItem(EditedItem);
        }

        public virtual async Task<bool> Save()
        {
            try
            {
                IsBusy = true;
                var errors = Validate();
                if (errors != null && errors.Any())
                {
                    ShowErrors(errors);
                    IsBusy = false;
                    return false;
                }

                PreSave();
                Saved?.Invoke(this, new EditorSaveArgs<TModel>(OperationType, await SaveItem()));
                IsBusy = false;
                return true;
            }
            catch (Exception ex)
            {
                _logger?.Log(_eventsSource, ex);
                IsBusy = false;
                throw;
            }
        }

        protected virtual Task<TModel> SaveItem()
        {
            return Task.FromResult(EditedItem);
        }

        protected virtual void ShowErrors(IEnumerable<string> errors)
        {
            var errorBuilder = new StringBuilder();
            foreach (var error in errors)
            {
                errorBuilder.Append(error);
                errorBuilder.Append(Environment.NewLine);
            }
            var errorMessage = errorBuilder.ToString(0, errorBuilder.Length - Environment.NewLine.Length);
            _toast.ShowToast(errorMessage, ToastType.Error);
        }

        public virtual void OnSave(object sender, EditorSaveArgs<TModel> e)
        {
        }

        public virtual void OnCanceled(object sender, EditorSaveArgs<TModel> e)
        {
        }
        public virtual void Show()
        {
            _dialogService.ShowEditorDialog(this);
        }
    }

    public abstract class EditorViewModelBase<TEditor, TView, TRepository, TModel,
        TKey> : EditorViewModelBase<TEditor, TView, TModel>
        where TView : UIElement, new()
        where TModel : class, IUniqueObject<TKey>, new()
        where TRepository : class, ICRUDRepository<TModel, TKey>
        where TEditor : EditorViewModelBase<TEditor, TView, TModel>
    {
        protected TRepository _repository;

        protected override async Task OnInitialization()
        {
            await base.OnInitialization();
            _repository = GetRepository<TRepository>();
        }

        protected override async Task<TModel> SaveItem()
        {
            return await _repository.Update(OperationType, EditedItem);
        }


    }

}