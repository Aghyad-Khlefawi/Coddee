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
    public class EmployeeEditorViewModel:ViewModelBase<EmployeeEditorView>
    {
        private Employee _editedItem;

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

        private AsyncObservableCollection<Company> _Companies;
        public AsyncObservableCollection<Company> Companies
        {
            get { return _Companies; }
            set { SetProperty(ref this._Companies, value); }
        }

        public override async Task Initialize()
        {
            await base.Initialize();
            Companies = AsyncObservableCollection<Company>.Create(await Resolve<ICompanyRepository>().GetItems());
        }

        public void Clear()
        {
            FirstName = null;
            LastName = null;
            Companies.SelectedItem = null;
            _editedItem = null;
        }


        public void Add(Company companiesSelectedItem)
        {
            Companies.SelectedItem = Companies.FirstOrDefault(e => e.ID == companiesSelectedItem.ID);
            OperationType = OperationType.Add;
            Resolve<IDialogService>().ShowEditorDialog(GetView(), Save, Cancel);
        }
        public void Edit(Employee item)
        {
            _editedItem = item;
            OperationType = OperationType.Edit;
            FirstName = item.FirstName;
            LastName = item.LastName;
            Companies.SelectedItem = Companies.FirstOrDefault(e => e.ID == item.CompanyID);
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
            if (Companies.SelectedItem == null)
            {
                ToastError("The Company field is required");
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
                        CompanyID = Companies.SelectedItem.ID
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
                        CompanyID = Companies.SelectedItem.ID
                    });
            }
            OnSave?.Invoke(OperationType, res);
            Clear();
        }
    }
}
