using System.Threading.Tasks;
using System.Windows.Input;
using Coddee.Collections;
using Coddee.Commands;
using Coddee.Data;
using Coddee.WPF;
using Coddee.WPF.Commands;
using HR.Clients.WPF.Components.Managements;
using HR.Clients.WPF.Interfaces;
using HR.Data.Models;
using HR.Data.Repositories;

namespace HR.Clients.WPF.Components
{
    public class CitiesViewModel : ManagementViewModelBase<CitiesView,ICityEditor,ICityRepository,City,int>
    {
        public override string Title
        {
            get { return "Cities"; }
        }
        
        protected override bool Filter(City item, string term)
        {
            return item.Name.ToLower().Contains(term.ToLower());
        }

        protected override string GetItemDescription(City item)
        {
            return item.Name;
        }
    }
}