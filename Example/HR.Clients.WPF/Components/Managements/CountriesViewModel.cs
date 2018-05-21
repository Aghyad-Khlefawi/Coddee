using System.Threading.Tasks;
using System.Windows.Input;
using Coddee.Collections;
using Coddee.Commands;
using Coddee.WPF;
using Coddee.WPF.Commands;
using HR.Clients.WPF.Components.Managements;
using HR.Clients.WPF.Interfaces;
using HR.Data.Models;
using HR.Data.Repositories;

namespace HR.Clients.WPF.Components
{
    public class CountriesViewModel : ManagementViewModelBase<CountriesView, ICountryEditor, ICountryRepository, Country, int>
    {
        public override string Title
        {
            get { return "Countries"; }
        }

        protected override bool Filter(Country item, string term)
        {
            return item.Name.ToLower().Contains(term.ToLower());
        }

        protected override string GetItemDescription(Country item)
        {
            return item.Name;
        }
    }
}