using System.Collections.Generic;
using Coddee.Validation;
using Coddee.WPF;
using HR.Clients.WPF.Interfaces;
using HR.Data.Models;
using HR.Data.Repositories;

namespace HR.Clients.WPF.Components.Companies
{
    public class EmployeeEditorViewModel : EditorViewModelBase<EmployeeEditorViewModel, EmployeeEditorView,IEmployeeRepository, Employee, int>, IEmployeeEditor
    {
        public EmployeeEditorViewModel()
        {
            Title = "Employee";
        }

        private string _firstName;
        [EditorField]
        public string FirstName
        {
            get { return _firstName; }
            set { SetProperty(ref _firstName, value); }
        }

        private string _lastName;
        [EditorField]
        public string LastName
        {
            get { return _lastName; }
            set { SetProperty(ref _lastName, value); }
        }

        protected override void SetValidationRules(List<IValidationRule> validationRules)
        {
            base.SetValidationRules(validationRules);
            validationRules.Add(ValidationRule.CreateErrorRule(() => FirstName));
            validationRules.Add(ValidationRule.CreateErrorRule(() => LastName));
        }
    }
}