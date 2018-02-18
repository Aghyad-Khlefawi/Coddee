using System.Collections.Generic;
using Coddee.Xamarin.Common;

namespace Coddee.Xamarin.Services.ViewModelManager
{
    public class ViewModelInfo
    {
        public ViewModelInfo(IViewModel viewModel)
        {
            ViewModel = viewModel;
            ChildViewModels = new List<ViewModelInfo>();
        }

        public int ID { get; set; }
        public IViewModel ViewModel { get; set; }
        public List<ViewModelInfo> ChildViewModels { get; set; }
        public ViewModelInfo ParentViewModel { get; set; }
    }
}