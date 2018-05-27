using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Coddee.Validation;
using Coddee.WPF;
using HR.Clients.WPF.Interfaces;
using HR.Data.Models;
using HR.Data.Repositories;

namespace HR.Clients.WPF.Components.Managements
{
    public class DepartmentEditorViewModel : EditorViewModelBase<DepartmentEditorViewModel, DepartmentEditorView, IDepartmentRepository, Department, int>, IDepartmentEditor
    {
        public DepartmentEditorViewModel()
        {
            Title = "Department";
        }

        private string _name;
        [EditorField]
        public string Name
        {
            get { return _name; }
            set { SetProperty(ref _name, value); }
        }

        protected override async Task MapEditedItemToEditor(Department item)
        {
            await base.MapEditedItemToEditor(item);
            this.Name = item.Title;
        }

        protected override async Task MapEditorToEditedItem(Department item)
        {
            await base.MapEditorToEditedItem(item);
            item.Title = Name;
        }
        protected override void SetValidationRules(List<IValidationRule> validationRules)
        {
            base.SetValidationRules(validationRules);
            validationRules.Add(ValidationRule.CreateErrorRule(() => Name));
        }
    }
}