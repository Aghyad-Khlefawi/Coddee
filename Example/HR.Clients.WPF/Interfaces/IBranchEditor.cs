using Coddee.Mvvm;
using HR.Data.Models;

namespace HR.Clients.WPF.Interfaces
{
    public interface IBranchEditor : IEditorViewModel<Branch>
    {
        void Add(int companyId);

    }
}
