using System.Threading.Tasks;
using Coddee;
using Coddee.Modules;
using HR.Clients.WPF.Components;
using HR.Clients.WPF.Components.Companies;
using HR.Clients.WPF.Components.Managements;
using HR.Clients.WPF.Interfaces;

namespace HR.Clients.WPF.Modules
{
    [Module(nameof(HRModule), ModuleInitializationTypes.Manual, nameof(ViewModelManagerModule))]
    public class HRModule : IModule
    {
        public Task Initialize(IContainer container)
        {
            container.RegisterType<ICityEditor, CityEditorViewModel>();
            container.RegisterType<ICountryEditor, CountryEditorViewModel>();
            container.RegisterType<ICompanyEditor, CompanyEditorViewModel>();
            container.RegisterType<IBranchEditor, BranchEditorViewModel>();
            container.RegisterType<IJobEditor, JobEditorViewModel>();
            container.RegisterType<IDepartmentEditor, DepartmentEditorViewModel>();

            container.RegisterType<IEmployeeEditor, EmployeeEditorViewModel>();
            container.RegisterType<IBranchViewer, BranchViewerViewModel>();
            container.RegisterType<IEmployeeJobEditor, EmployeeJobEditorViewModel>();
            return Task.FromResult(true);
        }
    }
}
