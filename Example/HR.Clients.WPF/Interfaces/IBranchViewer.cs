using System.Threading.Tasks;
using Coddee.Mvvm;

namespace HR.Clients.WPF.Interfaces
{
    public interface IBranchViewer:IPresentableViewModel
    {
        Task SetCompany(int companyId);
    }
}
