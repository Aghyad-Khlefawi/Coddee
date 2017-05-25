using System;
using System.Linq;
using System.Threading.Tasks;
using Coddee;
using Coddee.Collections;
using Coddee.Data;
using Coddee.WPF;
using Coddee.WPF.Modules.Dialogs;
using HR.Data.Models;
using HR.Data.Repositories;

namespace HR.Clients.WPF.Companies.Editors
{
    public class EmployeeEditorViewModel : EditorViewModel<EmployeeEditorViewModel,EmployeeEditorView, IEmployeeRepository, Employee, Guid>
    {
        private Company _selectedCompany;

        public void Add(Company companiesSelectedItem)
        {
            _selectedCompany = companiesSelectedItem;
            base.Add();
        }

        public void Edit(Employee item, Company companiesSelectedItem)
        {
            _selectedCompany = companiesSelectedItem;
            base.Edit(item);
        }

        public override void PreSave()
        {
            EditedItem.CompanyID = _selectedCompany.ID;
            EditedItem.CompanyName = _selectedCompany.Name;
            EditedItem.StateName = _selectedCompany.StateName;
            base.PreSave();
        }
    }
}