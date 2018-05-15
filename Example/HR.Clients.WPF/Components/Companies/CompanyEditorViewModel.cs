using System.Collections.Generic;
using Coddee.Validation;
using Coddee.WPF;
using HR.Clients.WPF.Interfaces;
using HR.Data.Models;
using HR.Data.Repositories;

namespace HR.Clients.WPF.Components.Companies
{
    public class CompanyEditorViewModel : EditorViewModelBase<CompanyEditorViewModel, CompanyEditorView,ICompanyRepository,Company,int>,ICompanyEditor
    {
        public CompanyEditorViewModel()
        {
            Title = "Company";
        }

        private string _name;
        [EditorField]
        public string Name
        {
            get { return _name; }
            set { SetProperty(ref _name, value); }
        }

        protected override void SetValidationRules(List<IValidationRule> validationRules)
        {
            base.SetValidationRules(validationRules);
            validationRules.Add(ValidationRule.CreateErrorRule(()=>Name));
        }
    }
}