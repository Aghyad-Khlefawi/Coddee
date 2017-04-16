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
    public class EmployeeEditorViewModel : ViewModelBase<EmployeeEditorView>
    {
        private Employee _editedItem;
        private Company _selectedCompany;

        public event Action<OperationType, Employee> OnSave;

        private OperationType _operationType;
        public OperationType OperationType
        {
            get { return _operationType; }
            set { SetProperty(ref this._operationType, value); }
        }

        private string _firstName;
        public string FirstName
        {
            get { return _firstName; }
            set { SetProperty(ref this._firstName, value); }
        }

        private string _lastName;
        public string LastName
        {
            get { return _lastName; }
            set { SetProperty(ref this._lastName, value); }
        }


        public override async Task Initialize()
        {
            await base.Initialize();
        }

        public void Clear()
        {
            FirstName = null;
            LastName = null;
            _editedItem = null;
        }


        public void Add(Company companiesSelectedItem)
        {
            _selectedCompany = companiesSelectedItem;
            OperationType = OperationType.Add;
            Resolve<IDialogService>().ShowEditorDialog(GetView(), Save, Cancel);
        }

        public void Edit(Employee item, Company companiesSelectedItem)
        {
            _selectedCompany = companiesSelectedItem;
            _editedItem = item;
            OperationType = OperationType.Edit;
            FirstName = item.FirstName;
            LastName = item.LastName;
            Resolve<IDialogService>().ShowEditorDialog(GetView(), Save, Cancel);
        }

        private void Cancel()
        {
            Clear();
        }

        private async void Save()
        {
            if (string.IsNullOrEmpty(FirstName) || string.IsNullOrEmpty(LastName))
            {
                ToastError("First and last name fields are required");
                return;
            }
            Employee res;
            if (OperationType == OperationType.Add)
            {
                res = await Resolve<IEmployeeRepository>()
                    .InsertItem(new Employee
                    {
                        FirstName = FirstName,
                        LastName = LastName,
                        CompanyID = _selectedCompany.ID,
                        CompanyName = _selectedCompany.Name,
                        StateName = _selectedCompany.StateName
                    });
            }
            else
            {
                res = await Resolve<IEmployeeRepository>()
                    .UpdateItem(new Employee
                    {
                        ID = _editedItem.ID,
                        FirstName = FirstName,
                        LastName = LastName,
                        CompanyID = _selectedCompany.ID,
                        CompanyName = _selectedCompany.Name,
                        StateName = _selectedCompany.StateName
                    });
            }
            OnSave?.Invoke(OperationType, res);
            Clear();
        }
    }
}