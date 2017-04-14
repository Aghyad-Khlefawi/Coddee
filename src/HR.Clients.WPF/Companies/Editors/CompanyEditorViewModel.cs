// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using System.Linq;
using System.Threading.Tasks;
using Coddee;
using Coddee.Collections;
using Coddee.WPF;
using Coddee.WPF.Modules.Dialogs;
using HR.Data.Models;
using HR.Data.Repositories;

namespace HR.Clients.WPF.Companies.Editors
{
    public class CompanyEditorViewModel : ViewModelBase<CompanyEditorView>
    {
        private Company _editedItem;

        public event Action<OperationType, Company> OnSave;

        private OperationType _operationType;
        public OperationType OperationType
        {
            get { return _operationType; }
            set { SetProperty(ref this._operationType, value); }
        }

        private string _name;
        public string Name
        {
            get { return _name; }
            set { SetProperty(ref this._name, value); }
        }

        private AsyncObservableCollection<State> _states;
        public AsyncObservableCollection<State> States
        {
            get { return _states; }
            set { SetProperty(ref this._states, value); }
        }

        public override async Task Initialize()
        {
            await base.Initialize();
            States = AsyncObservableCollection<State>.Create(await Resolve<IStateRepository>().GetItems());
        }

        public void Clear()
        {
            Name = null;
            States.SelectedItem = null;
            _editedItem = null;
        }


        public void Add()
        {
            OperationType = OperationType.Add;
            Resolve<IDialogService>().ShowEditorDialog(GetView(), Save, Cancel);
        }
        public void Edit(Company item)
        {
            _editedItem = item;
            OperationType = OperationType.Edit;
            Name = item.Name;
            States.SelectedItem = States.FirstOrDefault(e => e.ID == item.StateID);
            Resolve<IDialogService>().ShowEditorDialog(GetView(), Save, Cancel);
        }
        private void Cancel()
        {
            Clear();
        }

        private async void Save()
        {
            if (string.IsNullOrEmpty(Name))
            {
                ToastError("The name field is required");
                return;
            }
            if (States.SelectedItem == null)
            {
                ToastError("The state field is required");
                return;
            }
            Company res;
            if (OperationType == OperationType.Add)
            {
                res = await Resolve<ICompanyRepository>()
                    .InsertItem(new Company
                    {
                        Name = Name,
                        StateID = States.SelectedItem.ID
                    });
            }
            else
            {
                res = await Resolve<ICompanyRepository>()
                    .UpdateItem(new Company
                    {
                        ID = _editedItem.ID,
                        Name = Name,
                        StateID = States.SelectedItem.ID
                    });
            }
            OnSave?.Invoke(OperationType, res);
            Clear();
        }
    }
}