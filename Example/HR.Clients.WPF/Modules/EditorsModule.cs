using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Coddee;
using HR.Clients.WPF.Components;
using HR.Clients.WPF.Components.Companies;
using HR.Clients.WPF.Interfaces;

namespace HR.Clients.WPF.Modules
{
    [Module(nameof(EditorsModule), ModuleInitializationTypes.Manual)]
    public class EditorsModule : IModule
    {
        public Task Initialize(IContainer container)
        {
            container.RegisterType<ICityEditor, CityEditorViewModel>();
            container.RegisterType<ICountryEditor, CountryEditorViewModel>();
            container.RegisterType<ICompanyEditor, CompanyEditorViewModel>();
            return Task.FromResult(true);
        }
    }
}
