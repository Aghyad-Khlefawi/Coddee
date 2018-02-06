// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using Coddee.Data;
using Coddee.Exceptions;
using Coddee.Services;
using Coddee.Services.Dialogs;
using Coddee.Validation;
using Expression = System.Linq.Expressions.Expression;

namespace Coddee.WPF
{

    public abstract class EditorViewModelBase<TEditor, TView, TModel> : ViewModelBase<TView>,
        IEditorViewModel<TView, TModel>
        where TView : System.Windows.UIElement, new()
        where TModel : new()
        where TEditor : EditorViewModelBase<TEditor, TView, TModel>

    {
        private const string _eventsSource = "EditorBase";
        protected IEnumerable<EditorFieldInfo> _editorFields;
        protected EditorViewModelBase()
        {
            Saved += OnSave;
            Canceled += OnCanceled;
        }

        protected Action<TEditor> _clearAction;

        public event EventHandler<EditorSaveArgs<TModel>> Saved;
        public event EventHandler<EditorSaveArgs<TModel>> Canceled;

        public override ViewModelOptions DefaultViewModelOptions => ViewModelOptions.Editor;

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

        private string _title;
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref this._title, value); }
        }

        private string _fullTitle;
        public string FullTitle
        {
            get { return _fullTitle; }
            set { SetProperty(ref this._fullTitle, value); }
        }

        public virtual void Add()
        {
            OperationType = OperationType.Add;
            Clear();
            OnAdd();
            FullTitle = _localization["AddTemplate"].Replace("$Name$", _localization[Title]);
        }

        public virtual void Clear()
        {
            EditedItem = new TModel();
            _clearAction?.Invoke((TEditor)this);
        }

        public virtual void Edit(TModel item)
        {
            FillingValues = true;
            OperationType = OperationType.Edit;
            Clear();
            FullTitle = _localization["EditTemplate"].Replace("$Name$", _localization[Title]);
            EditedItem = _mapper.Map<TModel>(item);
            OnEdit(item);
            FillingValues = false;
        }

        protected override async Task OnInitialization()
        {
            GetEditorFields();
            await base.OnInitialization();
            _mapper.RegisterMap<TModel, TModel>();
            _mapper.RegisterTwoWayMap<TEditor, TModel>();
            GenerateClearFunction();
        }

        protected override void SetValidationRules(List<IValidationRule> validationRules)
        {
            base.SetValidationRules(validationRules);
            foreach (var editorFieldInfo in _editorFields.Where(e=>e.Attribute.IsRequired))
            {
                var property = Expression.Property(Expression.Constant(this), editorFieldInfo.Property.Name);
                var lambda = Expression.Lambda<Func<object>>(property).Compile();
                var rule = new ValidationRule(ValidationType.Error,Validators.GetValidator(editorFieldInfo.Property.PropertyType),editorFieldInfo.Property.Name,lambda);
                validationRules.Add(rule);
            }
        }

        private void GetEditorFields()
        {
            _editorFields = typeof(TEditor).GetProperties().Where(e => e.IsDefined(typeof(EditorFieldAttribute), true)).Select(e => new EditorFieldInfo
            {
                Attribute = e.GetCustomAttribute<EditorFieldAttribute>(),
                Property = e
            });
        }

        private void GenerateClearFunction()
        {
            if (_editorFields.Any())
            {
                var param = Expression.Parameter(typeof(TEditor), "e");
                var expressions = new List<Expression<Action<TEditor>>>(_editorFields.Count());
                foreach (var fielInfo in _editorFields)
                {
                    Expression<Action<TEditor>> clearExpression = null;
                    switch (fielInfo.Property.GetCustomAttribute<EditorFieldAttribute>().ClearAction)
                    {
                        case ClearAction.Default:
                            // Generate default assign expression ( e=> e.Property = default(T) )
                            clearExpression = Expression.Lambda<Action<TEditor>>(Expression.Assign(Expression.Property(param, fielInfo.Property.Name), Expression.Default(fielInfo.Property.PropertyType)), param);
                            break;
                        case ClearAction.Clear:
                            // Generate call expressions ( e=>e.Clear() )
                            var method = fielInfo.Property.DeclaringType.GetMethod(nameof(ICollection<object>.Clear));
                            if (method != null)
                            {
                                clearExpression = Expression.Lambda<Action<TEditor>>(Expression.Call(Expression.Property(param, fielInfo.Property.Name), method), param);
                            }
                            break;
                    }
                    if (clearExpression != null)
                        expressions.Add(clearExpression);

                }
                // Generate block expression to call clear function for each property
                // (e=> e.Property1 = default(T);
                //      e.Property2.Clear();
                // )
                var exp = Expression.Lambda<Action<TEditor>>(Expression.Block(expressions.Select(ex => Expression.Invoke(ex, param))), param);

                //Compile the expression to a lambda 
                _clearAction = exp.Compile();
            }
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

        public virtual async Task Save()
        {
            try
            {
                IsBusy = true;

                //Validation
                var validationResult = Validate();
                if (validationResult != null && !validationResult.IsValid)
                {
                    if (ViewModelOptions.ShowErrors)
                    {
                        ShowErrors(validationResult);
                    }
                    IsBusy = false;
                    throw new ValidationException(validationResult);
                }

                PreSave();
                var result = await SaveItem();
                Saved?.Invoke(this, new EditorSaveArgs<TModel>(OperationType, result));
                IsBusy = false;
            }
            catch (ValidationException)
            {
                throw;
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

        protected virtual void ShowErrors(ValidationResult res)
        {
            res.Errors.ForEach(e => _toast.ShowToast(e, ToastType.Error));
            res.Warnings.ForEach(e => _toast.ShowToast(e, ToastType.Warning));
        }

        public virtual void OnSave(object sender, EditorSaveArgs<TModel> e)
        {
        }

        public virtual void OnCanceled(object sender, EditorSaveArgs<TModel> e)
        {
        }

        private IDialog _dialog;
        public virtual void Show()
        {
            if (_dialog == null)
                _dialog = _dialogService.CreateDialog(Title, this);
            _dialog.Show();
        }
    }

    public abstract class EditorViewModelBase<TEditor, TView, TRepository, TModel, TKey> : EditorViewModelBase<TEditor, TView, TModel>
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