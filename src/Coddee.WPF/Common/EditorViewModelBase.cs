// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using Coddee.Data;
using Coddee.Exceptions;
using Coddee.Mvvm;
using Coddee.Services;
using Coddee.Services.Dialogs;
using Coddee.Validation;

namespace Coddee.WPF
{
    /// <summary>
    /// A base class for editors ViewModels that provide the ability to add and edit a model
    /// </summary>
    /// <typeparam name="TEditor">The same type of the ViewModel inheriting this class</typeparam>
    /// <typeparam name="TView">The view type of the ViewModel</typeparam>
    /// <typeparam name="TModel">The type of the model to be added or edited.</typeparam>
    public abstract class EditorViewModelBase<TEditor, TView, TModel> : ViewModelBase<TView>,
        IEditorViewModel<TView, TModel>
        where TView : System.Windows.UIElement, new()
        where TModel : new()
        where TEditor : EditorViewModelBase<TEditor, TView, TModel>

    {
        private const string _eventsSource = "EditorBase";

        /// <summary>
        /// The fields marked with <see cref="EditorFieldAttribute"/>
        /// </summary>
        protected IEnumerable<EditorFieldInfo> _editorFields;

        /// <inheritdoc />
        protected EditorViewModelBase()
        {

            Saved += OnSave;
            Canceled += OnCanceled;
        }

        /// <summary>
        /// A dynamically generated action to clear the editor fields base on <see cref="EditorFieldAttribute"/>
        /// </summary>
        protected Action<TEditor> _clearAction;

        /// <inheritdoc />
        public event EventHandler<EditorSaveArgs<TModel>> Saved;
        /// <inheritdoc />
        public event EventHandler<EditorSaveArgs<TModel>> Canceled;

        /// <inheritdoc />
        public override ViewModelOptions DefaultViewModelOptions => ViewModelOptions.Editor;

        private OperationType _operationType;

        /// <summary>
        /// The type of the operation that is currently happening in the editor.
        /// </summary>
        public OperationType OperationType
        {
            get { return _operationType; }
            set { SetProperty(ref this._operationType, value); }
        }

        private TModel _editedItem;
        /// <inheritdoc />
        public TModel EditedItem
        {
            get { return _editedItem; }
            set { SetProperty(ref this._editedItem, value); }
        }

        private bool _isSaving;
        /// <summary>
        /// Indicates whether the editor is currently performing the save operation.
        /// </summary>
        public bool IsSaving
        {
            get { return _isSaving; }
            set
            {
                SetProperty(ref _isSaving, value);
                if (_dialog != null)
                {
                    var saveCommand = _dialog.Commands.FirstOrDefault(e => e.Tag == ActionCommandTags.SaveCommand);
                    if (saveCommand != null)
                        saveCommand.SetCanExecute(!value);
                }
            }
        }

        private bool _fillingValues;

        /// <summary>
        /// Indicates whether the editor is currently filling the fields from the edited object.
        /// </summary>
        public bool FillingValues
        {
            get { return _fillingValues; }
            set { SetProperty(ref this._fillingValues, value); }
        }

        private string _title;
        /// <inheritdoc />
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref this._title, value); }
        }

        private string _fullTitle;
        /// <inheritdoc />
        public string FullTitle
        {
            get { return _fullTitle; }
            set { SetProperty(ref this._fullTitle, value); }
        }

        /// <inheritdoc />
        public virtual void Add()
        {
            OperationType = OperationType.Add;
            Clear();
            OnAdd();
            FullTitle = _localization["AddTemplate"].Replace("$Name$", _localization[Title]);
        }

        /// <inheritdoc />
        public virtual void Clear()
        {
            EditedItem = new TModel();
            _clearAction?.Invoke((TEditor)this);
        }

        /// <inheritdoc />
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

        /// <inheritdoc />
        protected override async Task OnInitialization()
        {
            GetEditorFields();
            await base.OnInitialization();
            _mapper.RegisterMap<TModel, TModel>();
            _mapper.RegisterTwoWayMap<TEditor, TModel>();
            GenerateClearFunction();
        }

        /// <inheritdoc />
        protected override void SetValidationRules(List<IValidationRule> validationRules)
        {
            base.SetValidationRules(validationRules);
            foreach (var editorFieldInfo in _editorFields.Where(e => e.Attribute.IsRequired))
            {
                var property = Expression.Property(Expression.Constant(this), editorFieldInfo.Property.Name);
                var lambda = Expression.Lambda<Func<object>>(property).Compile();
                var rule = new ValidationRule(ValidationType.Error, Validators.GetValidator(editorFieldInfo.Property.PropertyType), editorFieldInfo.Property.Name, lambda);
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

        /// <summary>
        /// Executed when the <see cref="Add"/> method is called
        /// </summary>
        protected virtual void OnAdd()
        {
        }
        /// <summary>
        /// Executed when the <see cref="Edit"/> method is called
        /// </summary>
        /// <param name="item">The edited item.</param>
        protected virtual async Task OnEdit(TModel item)
        {
            await MapEditedItemToEditor(item);
        }

        /// <summary>
        /// Map the values from the edited item to the editor fields.
        /// </summary>
        protected virtual Task MapEditedItemToEditor(TModel item)
        {
            _mapper.MapInstance(item, (TEditor)this);
            return completedTask;
        }
        /// <summary>
        /// Map the values from the  editor fields to the edited item .
        /// </summary>
        /// <param name="item"></param>
        protected virtual Task MapEditorToEditedItem(TModel item)
        {
            _mapper.MapInstance((TEditor)this, item);
            return completedTask;
        }

        /// <inheritdoc />
        public void Cancel()
        {
            Canceled?.Invoke(this, new EditorSaveArgs<TModel>(OperationType, EditedItem));
        }

        /// <summary>
        /// Called perform the edited item is sent to the repository.
        /// </summary>
        public virtual async Task PreSave()
        {
            await MapEditorToEditedItem(EditedItem);
        }

        /// <inheritdoc />
        public virtual async Task Save()
        {
            try
            {
                IsBusy = true;
                IsSaving = true;
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

                await PreSave();
                var result = await SaveItem();
                Saved?.Invoke(this, new EditorSaveArgs<TModel>(OperationType, result));
                IsBusy = false;
                IsSaving = false;
            }
            catch (ValidationException)
            {
                IsBusy = false;
                IsSaving = false;
                throw;
            }
            catch (Exception ex)
            {
                _logger?.Log(_eventsSource, ex);
                IsBusy = false;
                IsSaving = false;
                throw;
            }
        }

        /// <summary>
        /// Performs the save to the data store.
        /// </summary>
        /// <returns></returns>
        protected virtual Task<TModel> SaveItem()
        {
            return Task.FromResult(EditedItem);
        }

        /// <summary>
        /// Display the errors to the user.
        /// </summary>
        /// <param name="res">The last validation result</param>
        protected virtual void ShowErrors(ValidationResult res)
        {
            res.Errors.ForEach(e => _toast.ShowToast(e, ToastType.Error));
            res.Warnings.ForEach(e => _toast.ShowToast(e, ToastType.Warning));
        }

        /// <summary>
        /// Called after the <see cref="Save"/> method is called.
        /// </summary>
        public virtual void OnSave(object sender, EditorSaveArgs<TModel> e)
        {
        }

        /// <summary>
        /// Called after the <see cref="Cancel"/> method is called.
        /// </summary>
        public virtual void OnCanceled(object sender, EditorSaveArgs<TModel> e)
        {
        }

        private IDialog _dialog;
        /// <inheritdoc />
        public virtual void Show()
        {
            if (_dialog == null)
                _dialog = _dialogService.CreateDialog(Title, (IEditorViewModel)this);
            _dialog.Show();
        }
    }

    /// <summary>
    /// A base class for editors ViewModels that provide the ability to add and edit a model
    /// </summary>
    /// <typeparam name="TEditor">The same type of the ViewModel inheriting this class</typeparam>
    /// <typeparam name="TView">The view type of the ViewModel</typeparam>
    /// <typeparam name="TModel">The type of the model to be added or edited.</typeparam>
    /// <typeparam name="TRepository">The <see cref="IRepository"/> that the editor will send the item to.</typeparam>
    /// <typeparam name="TKey">The key type of the model.</typeparam>
    public abstract class EditorViewModelBase<TEditor, TView, TRepository, TModel, TKey> : EditorViewModelBase<TEditor, TView, TModel>
        where TView : System.Windows.UIElement, new()
        where TModel : class, IUniqueObject<TKey>, new()
        where TRepository : class, ICRUDRepository<TModel, TKey>
        where TEditor : EditorViewModelBase<TEditor, TView, TModel>
    {
        /// <summary>
        /// The model repository.
        /// </summary>
        protected TRepository _repository;

        /// <inheritdoc />
        protected override async Task OnInitialization()
        {
            await base.OnInitialization();
            _repository = GetRepository<TRepository>();
        }

        /// <inheritdoc />
        protected override async Task<TModel> SaveItem()
        {
            return await _repository.Update(OperationType, EditedItem);
        }
    }


}