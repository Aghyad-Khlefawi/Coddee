using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Coddee.Validation;
using Coddee.WPF;
using HR.Data.Models;
using HR.Data.Repositories;

namespace HR.Clients.WPF.Components
{
    public class CountryEditorViewModel : EditorViewModelBase<CountryEditorViewModel, CountryEditorView, ICountryRepository, Country, int>
    {
        public CountryEditorViewModel()
        {
            Title = "Country";
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
            validationRules.Add(ValidationRule.CreateErrorRule(() => Name));
        }
    }
}